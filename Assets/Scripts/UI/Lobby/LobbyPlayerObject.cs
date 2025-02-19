using Networking.Shared;
using TMPro;
using Unity.Netcode;
using Unity.Services.Lobbies.Models;
using UnityEngine;
using UnityEngine.UI;


namespace UI
{
    public class LobbyPlayerObject : MonoBehaviour
    {
        [SerializeField] private TMP_Text lobbyNameText;
        [SerializeField] private Toggle muteButton;
        [SerializeField] private GameObject kickButton;

        private LobbyMenu _lobbyMenu;
        private string _playerId;

        public void Initialize(LobbyMenu lobbyMenu,string playerName, string playerId, bool isHost)
        {
            kickButton.SetActive(isHost);
            if (playerId == LobbyManager.Instance.PlayerId)
            {
                muteButton.gameObject.SetActive(false);
                kickButton.gameObject.SetActive(false);
            }

            lobbyNameText.text = playerName;
            _playerId = playerId;
        }

        public void ToggleMute()
        {
            if (muteButton.isOn)
            {
                VivoxManager.Instance.MutePlayerLocally(_playerId);
            }
            else
            {
                VivoxManager.Instance.UnmutePlayerLocally(_playerId);
            }
        }

        public async void Kick()
        {
            await LobbyManager.Instance.RemovePlayer(_playerId);
        }
    }
}