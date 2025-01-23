using System;
using System.Text;
using System.Threading.Tasks;
using Core.Cars;
using Networking.Shared;
using UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using Unity.Services.Vivox;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class ClientGameManager : IDisposable
    {
        private const string MenuSceneName = "MainMenu";
        private JoinAllocation _joinAllocation;
        private NetworkClient _networkClient;

        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();
            await VivoxService.Instance.InitializeAsync();

            _networkClient = new NetworkClient(NetworkManager.Singleton);
            
            AuthState authState = await AuthenticationWrapper.DoAuth();
            if (authState == AuthState.Authenticated)
            {
                return true;
            }
            
            return false; //failed to authenticate
        }

        public async Task StartClientAsync(string joinCode)
        {
            try
            {
                _joinAllocation = await Relay.Instance.JoinAllocationAsync(joinCode);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(_joinAllocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            UserData userData = new UserData()
            {
                userName = PlayerPrefs.GetString(NameInput.PlayerNameKey, "Anonymous Player"),
                userAuthId = AuthenticationService.Instance.PlayerId,
                carModelName = PlayerPrefs.GetString(ModelPickerUI.PlayerPrefsKey,"Race Car")
            };
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadByte = Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadByte;
            
            NetworkManager.Singleton.StartClient();
        }
        
        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }

        public void Dispose()
        {
            Shutdown();
        }

        public void Shutdown()
        {
            _networkClient?.Dispose();
            if (NetworkManager.Singleton.IsListening)
            {
                NetworkManager.Singleton.Shutdown();
            }
        }
    }
}