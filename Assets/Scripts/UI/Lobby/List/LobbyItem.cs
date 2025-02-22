using TMPro;
using Unity.Services.Lobbies.Models;
using UnityEngine;

namespace UI
{
    public class LobbyItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text lobbyNameText;
        [SerializeField] private TMP_Text lobbyPlayersText;

        private Lobby _lobby;
        private LobbiesList _lobbiesList;

        public void Initialize(LobbiesList list, Lobby lobby)
        {
            lobbyNameText.text = lobby.Name;
            lobbyPlayersText.text = $"{lobby.Players.Count}/{lobby.MaxPlayers}";
            _lobbiesList = list;
            _lobby = lobby;
        }

        public void Join()
        {
            Debug.Log(_lobby.LobbyCode);
            Debug.Log(_lobby.Id);
            _lobbiesList.JoinAsync(_lobby);
        }
    }
}