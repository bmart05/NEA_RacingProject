using Networking.Shared;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace UI
{
    public class LobbyPlayerObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text lobbyNameText;
        [SerializeField] private GameObject kickButton;

        private LobbyMenu _lobbyMenu;
        private string _playerId;

        public void Initialize(LobbyMenu lobbyMenu,string playerName, string playerId, bool isHost)
        {
            kickButton.SetActive(isHost);

            lobbyNameText.text = playerName;
            _playerId = playerId;
        }

        public async void Kick()
        {
            await LobbyManager.Instance.RemovePlayer(_playerId);
        }
    }
}