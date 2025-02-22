using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Core.Position;
using UI;
using Unity.Netcode;

using UnityEngine;
using UnityEngine.SceneManagement;

namespace Core.Game
{
    public class GameManager : NetworkBehaviour
    {
        private static GameManager _instance;
    
        public static GameManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }
    
                _instance = FindObjectOfType<GameManager>();
                if (_instance == null)
                {
                    return null;
                }
    
                return _instance;
            }
        }
    
        
        [Header("References")] 
        [SerializeField] private CarPlayer playerPrefab; //this should change for each player but is fine for now
    
        [SerializeField] private List<CarPlayer> _playerObjects;
    
        [field: SerializeField] public CarPlayer localPlayerObject;
        [SerializeField] private AudioSource musicSource;
        public NetworkVariable<int> NumPlayers { get; private set; } = new NetworkVariable<int>();
        public NetworkVariable<bool> HasGameStarted{ get; private set; } = new NetworkVariable<bool>();
        public NetworkVariable<bool> HasGameFinished { get; private set; } = new NetworkVariable<bool>();
        
        private int _playersLoadedIn;
        private int _playersFinishedCountdown;
        private int _playersFinishedFinishCountdown;
    
        public override void OnNetworkSpawn()
        {
            InitializeGame();
        }
    
        private void SpawnAllPlayers()
        {
            var connectedClients = NetworkManager.Singleton.ConnectedClients;
            int playerIndex = 0;
            foreach (var relayClientId in connectedClients.Keys)
            {
                Transform startingPosition = RaceManager.Instance.GetStartingPosition(playerIndex);
                CarPlayer playerObject = Instantiate(playerPrefab, startingPosition.position,startingPosition.rotation);
                playerObject.NetworkObject.SpawnWithOwnership(relayClientId);
                RaceManager.Instance.InitializePlayer(playerObject);
                _playerObjects.Add(playerObject);
    
                if (relayClientId == NetworkManager.Singleton.LocalClientId)
                {
                    localPlayerObject = playerObject;
                }
                playerIndex++;
            }
            Debug.Log("Spawned all players");
            StartCountdownClientRpc(); //start countdown on all clients once they have spawned
        }
    
        private void InitializeGame()
        {
            
            if(IsHost)
            {
                NumPlayers.Value = NetworkManager.Singleton.ConnectedClients.Count;
            }
            OnPlayerStartedGameServerRpc();
        }
    
        [ClientRpc]
        private void StartCountdownClientRpc()
        {
            StartCoroutine(StartCountdown());
        }
    
        [ServerRpc(RequireOwnership = false)]
        private void FinishCountdownServerRpc()
        {
            _playersFinishedCountdown++;
            if (_playersFinishedCountdown >= NumPlayers.Value) //all clients must finish the countdown before the game starts to prevent players
                                                         //from finishing at different times on different clients
            {
                Debug.Log("Go!");
                Countdown.Instance.ShowGo();
                GameStartedClientRpc();
            }
        }
    
        private IEnumerator StartCountdown()
        {
            Debug.Log("Started race countdown");
            Countdown.Instance.ShowThree();
            yield return new WaitForSecondsRealtime(1f);
            Countdown.Instance.ShowTwo();
            yield return new WaitForSecondsRealtime(1f);
            Countdown.Instance.ShowOne();
            yield return new WaitForSecondsRealtime(1f);
            FinishCountdownServerRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        private void OnPlayerStartedGameServerRpc()
        {
            _playersLoadedIn++;
            if (_playersLoadedIn >= NumPlayers.Value)
            {
                SpawnAllPlayers();
            }
        }
    
        [ClientRpc]
        private void GameStartedClientRpc()
        {
            Debug.Log("Game has started");
            if (!IsHost)
            {
                _playerObjects = FindObjectsOfType<CarPlayer>().ToList();
                foreach (var player in  _playerObjects)
                {
                    if (player.OwnerClientId == NetworkManager.Singleton.LocalClientId)
                    {
                        localPlayerObject = player;
                    }
                }
            }
            else
            {
                HasGameStarted.Value = true;
            }
            foreach (var player in _playerObjects)
            {
                if (!IsHost)
                {
                    RaceManager.Instance.InitializePlayer(player);
                }
                player.SetCanMove(true);
            }
            RaceManager.Instance.SetStartingTime();
            musicSource.Play();
        }

        [ServerRpc]
        public void StartFinishCountdownServerRpc()
        {
            StartFinishCountdownClientRpc();
        }
        
        [ServerRpc(RequireOwnership = false)]
        public void HandleFinishCountdownEndServerRpc()
        {
            _playersFinishedFinishCountdown++;
            if (_playersFinishedFinishCountdown >= NumPlayers.Value)
            {
                HandleFinishGameServerRpc();
            }
        }
        [ClientRpc]
        public void StartFinishCountdownClientRpc()
        {
            StartCoroutine(StartFinishCountdown());
        }
        private IEnumerator StartFinishCountdown()
        {
            Debug.Log("Started countdown to finish");
            yield return new WaitForSecondsRealtime(10f);
            if (HasGameFinished.Value == false)
            {
                HandleFinishCountdownEndServerRpc();
            }
        }
        
        
        
        [ServerRpc]
        public void HandleFinishGameServerRpc()
        {
            HasGameFinished.Value = true;
            HandleFinishGameClientRpc();
        }
    
        [ClientRpc]
        private void HandleFinishGameClientRpc()
        {
            //check if there are any players that havent finished
            var players = FindObjectsByType<CarPlayer>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                RaceManager.Instance.FinishPlayerDnf(player);
            }
            RaceUI.Instance.ShowFinishUI();
            LeaderboardManager.SetNewScore(SceneManager.GetActiveScene().name,RaceManager.Instance.LocalFinishTime);
        }
    }
}