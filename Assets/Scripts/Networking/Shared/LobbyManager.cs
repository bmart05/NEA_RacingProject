using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Networking.Client;
using Networking.Host;
using UI;
using Unity.Netcode;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Serialization;

namespace Networking.Shared
{
    public class LobbyManager : MonoBehaviour
    {
        private static LobbyManager _instance;

        public static LobbyManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<LobbyManager>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }
        
        public Lobby ActiveLobby { get; private set; }
        public string LobbyId { get; private set; }
        public List<Player> Players { get; private set; }
        public string PlayerId { get; private set; }
        public string JoinCode { get; private set; }
        public string LobbyName { get; private set; }

        private float _updatePlayersFrequency = 5f;
        private float _nextUpdatePlayersTime;
        public static event Action<Lobby> OnLobbyUpdate;

        private string _playerName;
        private bool _isJoining;
        public bool IsHost { get; private set; }
        public bool IsPrivate { get; private set; }


        private void Start()
        {
            DontDestroyOnLoad(gameObject);
            PlayerId = AuthenticationService.Instance.PlayerId;
        }

        private async void Update()
        {
            try
            {
                if (ActiveLobby != null && Time.realtimeSinceStartup >= _nextUpdatePlayersTime)
                {
                    await PeriodicUpdateLobby();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task CreateLobbyAsync(string playerName, int maxConnections, string joinCode)
        {
            try
            {
                CreateLobbyOptions lobbyOptions = new CreateLobbyOptions()
                {
                    IsPrivate = false,
                    Player = CreatePlayerData(),
                    Data = new Dictionary<string, DataObject>()
                    {
                        {
                            "JoinCode", new DataObject(
                                visibility: DataObject.VisibilityOptions.Member,
                                value: joinCode
                            )
                        }
                    }
                };

                LobbyName = $"{playerName}'s Lobby";
                ActiveLobby = await Lobbies.Instance.CreateLobbyAsync(LobbyName, maxConnections, lobbyOptions);
                LobbyId = ActiveLobby.Id;
                Players = ActiveLobby?.Players;
                JoinCode = joinCode;
                
                await VivoxManager.Instance.JoinGroupChannel(JoinCode);
                
                HostSingleton.Instance.StartCoroutine(HeartbeatLobby(15f));
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                throw;
            }

            IsHost = true;
            _playerName = playerName;
        }
        
        public async Task JoinLobbyAsync(string playerName, string lobbyId)
        {
            if (_isJoining)
            {
                return;
            }
            
            _isJoining = true;
            try
            {

                JoinLobbyByIdOptions options = new JoinLobbyByIdOptions()
                {
                    Player = CreatePlayerData()
                };
                
                ActiveLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobbyId,options);
                LobbyName = ActiveLobby.Name;
                LobbyId = ActiveLobby.Id;
                Players = ActiveLobby?.Players;
                string joinCode = ActiveLobby.Data["JoinCode"].Value;
                IsHost = false;
                _playerName = playerName;
                JoinCode = joinCode;

                await VivoxManager.Instance.JoinGroupChannel(JoinCode);

                await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
            }
            catch (LobbyServiceException e)
            {   
                Console.WriteLine(e);
                throw;
            }

            _isJoining = false;

           
        }

        public async Task<List<Lobby>> GetActiveLobbiesAsync()
        {
            try
            {
                QueryLobbiesOptions options = new QueryLobbiesOptions();
                options.Count = 25;
                options.Filters = new List<QueryFilter>()
                {
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.AvailableSlots,
                        op: QueryFilter.OpOptions.GT,
                        value: "0"
                    ),
                    new QueryFilter(
                        field: QueryFilter.FieldOptions.IsLocked,
                        op: QueryFilter.OpOptions.EQ,
                        value: "0"
                    )
                };

                QueryResponse response = await Lobbies.Instance.QueryLobbiesAsync(options);

                return response.Results;
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async void StartGame()
        {
            await LockLobby(); //stops players from joining while in a game
            
            NetworkManager.Singleton.SceneManager.LoadScene("GameScene",LoadSceneMode.Single); //update to involve map voting
        }

        public async Task DestroyCurrentLobby()
        {
            try
            {
                if (ActiveLobby != null && IsHost)
                {
                    await Lobbies.Instance.DeleteLobbyAsync(LobbyId);
                    
                    OnPlayerNotInLobby();
                }
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task LeaveCurrentLobby()
        {
            try
            {
                await RemovePlayer(PlayerId);
                
                OnPlayerNotInLobby();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async Task RemovePlayer(string playerId)
        {
            try
            {
                await Lobbies.Instance.RemovePlayerAsync(LobbyId,playerId);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        
        public async Task LockLobby()
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions()
                {
                    IsLocked = true
                };

                await Lobbies.Instance.UpdateLobbyAsync(LobbyId, options);
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task UnlockLobby()
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions()
                {
                    IsLocked = false
                };

                await Lobbies.Instance.UpdateLobbyAsync(LobbyId, options);
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        
        public async Task SetLobbyToPrivate()
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions()
                {
                    IsPrivate = true
                };

                await Lobbies.Instance.UpdateLobbyAsync(LobbyId, options);
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
        public async Task SetLobbyToPublic()
        {
            try
            {
                UpdateLobbyOptions options = new UpdateLobbyOptions()
                {
                    IsPrivate = false,
                };

                await Lobbies.Instance.UpdateLobbyAsync(LobbyId, options);
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        public async void OnPlayerNotInLobby()
        {
            if (ActiveLobby != null)
            {
                ActiveLobby = null;
                await VivoxManager.Instance.LeaveCurrentChannel();
                SceneManager.LoadScene("MainMenu");
            }
        }

        
        private async Task PeriodicUpdateLobby()
        {
            try
            {
                _nextUpdatePlayersTime = Time.realtimeSinceStartup + _updatePlayersFrequency;
                
                Lobby updatedLobby = await Lobbies.Instance.GetLobbyAsync(LobbyId);
                UpdateLobby(updatedLobby);
            }
            catch (LobbyServiceException e)
            {
                Debug.LogError(e);
                throw;
            }
        }
        
        // ReSharper disable Unity.PerformanceAnalysis
        private void UpdateLobby(Lobby newLobby)
        {
            
            if (ActiveLobby == null || newLobby == null)
            {
                return;
            }

            if (DidPlayersChange(ActiveLobby.Players, newLobby.Players))
            {
                Debug.Log("Updated lobby");
                ActiveLobby = newLobby;
                Players = newLobby.Players;
                IsPrivate = newLobby.IsPrivate;

                if (Players.Exists(player => player.Id == PlayerId))
                {
                    OnLobbyUpdate?.Invoke(newLobby);
                }
                else
                {
                    OnPlayerNotInLobby();
                }
            }
        }

        private bool DidPlayersChange(List<Player> oldPlayers, List<Player> newPlayers)
        {
            if (oldPlayers.Count != newPlayers.Count)
            {
                return true;
            }

            for (int i = 0; i < newPlayers.Count; i++)
            {
                if (oldPlayers[i].Id != newPlayers[i].Id)
                {
                    return true;
                }
            }

            return false;
        }
        
        private IEnumerator HeartbeatLobby(float waitTimeSeconds)
        {
            WaitForSecondsRealtime delay = new WaitForSecondsRealtime(waitTimeSeconds);
            while (true)
            {
                Lobbies.Instance.SendHeartbeatPingAsync(LobbyId);
                yield return delay;
            }
        }

        private Player CreatePlayerData()
        {
            Player player = new Player();
            player.Data = new Dictionary<string, PlayerDataObject>()
            {
                {
                    "PlayerName",
                    new PlayerDataObject(PlayerDataObject.VisibilityOptions.Member,
                        PlayerPrefs.GetString(NameInput.PlayerNameKey, "Anonymous Player"))
                }

            };
            return player;
        }
    }
}