using System;
using System.Threading.Tasks;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Core;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class ClientGameManager
    {
        private const string MenuSceneName = "MainMenu";
        private JoinAllocation _joinAllocation;
        
        public async Task<bool> InitAsync()
        {
            await UnityServices.InitializeAsync();
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

            NetworkManager.Singleton.StartClient();
        }
        
        public void GoToMenu()
        {
            SceneManager.LoadScene(MenuSceneName);
        }
    }
}