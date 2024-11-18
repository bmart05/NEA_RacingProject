using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Core.Position;
using Unity.Netcode;
using UnityEditor.Rendering;
using UnityEngine;

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

        public NetworkVariable<int> NumPlayers { get; private set; } = new NetworkVariable<int>();
        public bool HasGameStarted { get; private set; }
        public bool HasGameFinished { get; private set; }
        
        private int _playersLoadedIn;
        private int _playersFinishedCountdown;
        
        private void Start()
        {
            if (IsHost)
            {
                InitializeGame();
            }
            else
            {
                InitializeGame();
            }
        }

        private void SpawnAllPlayers()
        {
            var connectedClients = NetworkManager.Singleton.ConnectedClients;
            int playerIndex = 0;
            foreach (var relayClientId in connectedClients.Keys)
            {
                Transform startingPosition = RaceManager.Instance.GetStartingPosition(playerIndex);
                Debug.Log(startingPosition.position);
                CarPlayer playerObject = Instantiate(playerPrefab, startingPosition.position,startingPosition.rotation);
                playerObject.NetworkObject.SpawnWithOwnership(relayClientId);
                RaceManager.Instance.InitializePlayer(playerObject);
                playerObject.SetCanMove(false);
                _playerObjects.Add(playerObject);
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
                GameStartedClientRpc();
            }
        }

        private IEnumerator StartCountdown()
        {
            Debug.Log("Started race countdown");
            yield return new WaitForSecondsRealtime(3f);
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
            HasGameStarted = true;
        }
        
        [ServerRpc]
        public void HandleFinishGameServerRpc()
        {
            HandleFinishGameClientRpc();
            Debug.Log("Destroying players");
            foreach (var player in _playerObjects)
            {
                Destroy(player.gameObject);
            }
        }

        [ClientRpc]
        private void HandleFinishGameClientRpc()
        {
            HasGameFinished = true;
            RaceUI.Instance.ShowFinishUI();
        }
    }
}