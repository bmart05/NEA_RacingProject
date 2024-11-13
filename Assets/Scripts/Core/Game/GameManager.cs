using System;
using System.Collections.Generic;
using System.Linq;
using Core.Player;
using Core.Position;
using Unity.Netcode;
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
        
        public int NumPlayers;
        public int _playersLoadedIn;
        
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
                _playerObjects.Add(playerObject);
                playerIndex++;
            }
            GameStartedClientRpc();
        }

        private void InitializeGame()
        {
            
            if(IsHost)
            {
                NumPlayers = NetworkManager.Singleton.ConnectedClients.Count;
            }
            OnPlayerStartedGameServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnPlayerStartedGameServerRpc()
        {
            _playersLoadedIn++;
            if (_playersLoadedIn >= NumPlayers)
            {
                SpawnAllPlayers();
            }
        }

        [ClientRpc]
        private void GameStartedClientRpc()
        {
            if (!IsHost)
            {
                _playerObjects = FindObjectsOfType<CarPlayer>().ToList();
                foreach (var player in _playerObjects)
                {
                    RaceManager.Instance.InitializePlayer(player);
                }
            }
        }
        
        public void HandleFinishGame()
        {
            foreach (var player in _playerObjects)
            {
                player.SetCanMove(false);
            }
            RaceUI.Instance.ShowFinishUI();
        }
    }
}