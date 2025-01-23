using System;
using Networking.Shared;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Client
{
    public class NetworkClient : IDisposable
    {
        private NetworkManager _manager;

        private const string MenuSceneName = "MainMenu";

        public NetworkClient(NetworkManager manager)
        {
            _manager = manager;

            _manager.OnClientDisconnectCallback += OnClientDisconnect;
        }
        
        private void OnClientDisconnect(ulong clientId)
        {
            if (clientId != 0 && clientId != _manager.LocalClientId) //stops the host from disconnecting
            {
                return;
            }

            if (SceneManager.GetActiveScene().name != MenuSceneName)
            {
                SceneManager.LoadScene(MenuSceneName);
            }

            if (_manager.IsConnectedClient)
            {
                _manager.Shutdown();
            }
        }

        public void Dispose()
        {
            if (_manager != null)
            {
                _manager.OnClientDisconnectCallback -= OnClientDisconnect;
            }
        }
    }
}