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
        [Header("References")] 
        [SerializeField] private CarPlayer playerPrefab; //this should change for each player but is fine for now

        [SerializeField] private List<CarPlayer> _playerObjects;
        
        private int NumPlayers;
        private int _playersLoadedIn;
        
        private void Start()
        {
            if (IsHost)
            {
                SpawnAllPlayers();
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
            NumPlayers = connectedClients.Count;

            int playerIndex = 0;
            foreach (var relayClientId in connectedClients.Keys)
            {
                Transform startingPosition = RaceManager.Instance.GetStartingPosition(playerIndex);
                CarPlayer playerObject = Instantiate(playerPrefab, startingPosition.position,startingPosition.rotation);
                playerObject.NetworkObject.SpawnWithOwnership(relayClientId);
                RaceManager.Instance.InitializePlayer(playerObject);
                _playerObjects.Add(playerObject);
                playerIndex++;
            }
        }

        private void InitializeGame()
        {
            if (!IsHost)
            {
                _playerObjects = FindObjectsOfType<CarPlayer>().ToList();
                foreach (var player in _playerObjects)
                {
                    RaceManager.Instance.InitializePlayer(player);
                }
            }
            OnPlayerStartedGameServerRpc();
        }

        [ServerRpc(RequireOwnership = false)]
        private void OnPlayerStartedGameServerRpc()
        {
            _playersLoadedIn++;
            if (_playersLoadedIn >= NumPlayers)
            {
                GameStartedClientRpc();
            }
        }

        [ClientRpc]
        private void GameStartedClientRpc()
        {
            // foreach (var player in _playerObjects)
            // {
            //     player.SetCanMove(true);
            // }
        }
    }
}