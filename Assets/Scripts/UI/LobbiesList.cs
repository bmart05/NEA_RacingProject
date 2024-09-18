using System;
using System.Collections.Generic;
using Networking.Client;
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

                foreach (Transform child in lobbyItemParent)
                {
                    Destroy(child.gameObject);
                }

                foreach (Lobby lobby in response.Results)
                {
                    LobbyItem item = Instantiate(lobbyItemPrefab,lobbyItemParent);
                    item.Initialize(this, lobby);
                }
            }
            catch (LobbyServiceException e)
            {
                Console.WriteLine(e);
                throw;
            }
  
            _isRefreshing = false;
        }

        public async void JoinAsync(Lobby lobby)
        {
            if (_isJoining)
            {
                return;
            }
            
            _isJoining = true;
            try
            {
                Lobby joiningLobby = await Lobbies.Instance.JoinLobbyByIdAsync(lobby.Id);
                string joinCode = joiningLobby.Data["JoinCode"].Value;

                await ClientSingleton.Instance.GameManager.StartClientAsync(joinCode);
            }
            catch (LobbyServiceException e)
            {   
                Console.WriteLine(e);
                throw;
            }

            _isJoining = false;
        }
    }
}