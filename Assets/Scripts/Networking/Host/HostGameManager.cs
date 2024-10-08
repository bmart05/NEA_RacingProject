using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Networking.Server;
using Networking.Shared;
using UI;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using Unity.Networking.Transport.Relay;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace Networking.Host
{
    public class HostGameManager
    {
        private const int MaxConnections = 16;

        private Allocation _allocation;
        private string _joinCode;
        private NetworkServer _networkServer;

        public async Task StartHostAsync()
        {
            try
            {
                _allocation = await Relay.Instance.CreateAllocationAsync(MaxConnections);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }
            
            try
            {
                _joinCode = await Relay.Instance.GetJoinCodeAsync(_allocation.AllocationId);
                Debug.Log(_joinCode);
            }
            catch (Exception e)
            {
                Debug.LogError(e);
                return;
            }

            UnityTransport transport = NetworkManager.Singleton.GetComponent<UnityTransport>();

            RelayServerData relayServerData = new RelayServerData(_allocation, "dtls");
            transport.SetRelayServerData(relayServerData);

            await LobbyManager.Instance.CreateLobbyAsync(PlayerPrefs.GetString(NameInput.PlayerNameKey, "Anonymous Player"), MaxConnections,_joinCode);

            _networkServer = new NetworkServer(NetworkManager.Singleton);
            
            UserData userData = new UserData()
            {
                userName = PlayerPrefs.GetString(NameInput.PlayerNameKey, "Anonymous Player"),
                userAuthId = AuthenticationService.Instance.PlayerId
            };
            string payload = JsonUtility.ToJson(userData);
            byte[] payloadByte = Encoding.UTF8.GetBytes(payload);
            NetworkManager.Singleton.NetworkConfig.ConnectionData = payloadByte; 

            NetworkManager.Singleton.StartHost();
            
            NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene", LoadSceneMode.Single);
        }
    }
}