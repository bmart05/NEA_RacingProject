using System;
using System.Collections.Generic;
using Networking.Client;
using Networking.Shared;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace UI
{
    public class LobbiesList : MonoBehaviour
    {
        [SerializeField] private LobbyItem lobbyItemPrefab;
        [SerializeField] private Transform lobbyItemParent;
        
        private bool _isJoining;
        private bool _isRefreshing;

        private void OnEnable()
        {
            RefreshList();
        }

        public async void RefreshList()
        {
            if (_isRefreshing)
            {
                return;
            }

            _isRefreshing = true;
            //remove all previous lobby instances created
            foreach (Transform child in lobbyItemParent)
            {
                Destroy(child.gameObject);
            }

            var results = await LobbyManager.Instance.GetActiveLobbiesAsync();

            foreach (Lobby lobby in results)
            {
                LobbyItem item = Instantiate(lobbyItemPrefab,lobbyItemParent);
                item.Initialize(this, lobby);
            }
  
            _isRefreshing = false;
        }

        public async void JoinAsync(Lobby lobby)
        {
            await LobbyManager.Instance.JoinLobbyByIdAsync(PlayerPrefs.GetString(NameInput.PlayerNameKey,"Anonymous Player"),lobby.Id);
        }
    }
}