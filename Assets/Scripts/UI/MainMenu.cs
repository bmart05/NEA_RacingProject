using Networking.Client;
using Networking.Host;
using Networking.Shared;
using TMPro;
using UnityEngine;

namespace UI
{
    public class MainMenu : MonoBehaviour
    {
        [SerializeField] private TMP_InputField joinCodeField;
        
        public async void StartHost()
        {
            await HostSingleton.Instance.GameManager.StartHostAsync();
        }

        public async void StartClient()
        {
            await LobbyManager.Instance.JoinLobbyByCodeAsync(PlayerPrefs.GetString(NameInput.PlayerNameKey,"Anonymous Player"),joinCodeField.text);
        }
    }
}