using System;
using System.Collections.Generic;
using Networking.Client;
using Networking.Host;
using Networking.Shared;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace UI
{
    public class LobbyMenu : NetworkBehaviour
    {
        [SerializeField] private LobbyPlayerObject lobbyPlayerPrefab;
        [SerializeField] private Transform lobbyPlayerObjectParent;
        [SerializeField] private TMP_Text joinCodeText;
        [SerializeField] private TMP_Text lobbyNameText;
        [SerializeField] private GameObject startButton;
        
        private void Start()
        {
            startButton.SetActive(false);
            RefreshList(LobbyManager.Instance.ActiveLobby);
            joinCodeText.text = LobbyManager.Instance.JoinCode;
            lobbyNameText.text = LobbyManager.Instance.LobbyName;
            
            LobbyManager.OnLobbyUpdate += RefreshList;
        }

        private void RefreshList(Lobby lobby)
        {
            if (lobby.Players.Count > 1 && LobbyManager.Instance.IsHost)
            {
                startButton.SetActive(true);
            }
            
            foreach (Transform child in lobbyPlayerObjectParent)   
            {
                Destroy(child.gameObject);
            }
            
            foreach (Player player in lobby.Players)
            {
                Debug.Log(player.Data["PlayerName"].Value);
                LobbyPlayerObject playerObject = Instantiate(lobbyPlayerPrefab, lobbyPlayerObjectParent);
                playerObject.Initialize(this,player.Data["PlayerName"].Value, player.Id, LobbyManager.Instance.IsHost);
            }
        }

        public void StartGame()
        {
            LobbyManager.Instance.StartGame();
        }
        

        public async void LeaveLobby()
        {
            if (LobbyManager.Instance.IsHost)
            {
                await LobbyManager.Instance.DestroyCurrentLobby();
            }
            else
            {
                await LobbyManager.Instance.LeaveCurrentLobby();
            }
        }
    }
}