using System;
using System.Collections.Generic;
using Networking.Shared;
using Unity.Netcode;
using Unity.Services.Lobbies.Http;
using UnityEngine;

namespace Networking.Server
{
    public class NetworkServer : IDisposable
    {
        private NetworkManager _manager;

        private Dictionary<ulong, string> _clientIdToAuthId = new Dictionary<ulong, string>();
        private Dictionary<string, UserData> _authIdToUserData = new Dictionary<string, UserData>();

        public NetworkServer(NetworkManager manager)
        {
            _manager = manager;

            _manager.ConnectionApprovalCallback += ApprovalCheck;
            _manager.OnServerStarted += OnNetworkReady;
        }


        private void ApprovalCheck(NetworkManager.ConnectionApprovalRequest request, NetworkManager.ConnectionApprovalResponse response)
        {
            string payload = System.Text.Encoding.UTF8.GetString(request.Payload);
            UserData userData = JsonUtility.FromJson<UserData>(payload);

            _clientIdToAuthId[request.ClientNetworkId] = userData.userAuthId;
            _authIdToUserData[userData.userAuthId] = userData;

            response.Approved = true;
        }

        public UserData GetUserDataFromClientId(ulong clientId)
        {
            return _authIdToUserData[_clientIdToAuthId[clientId]];
        }
        private void OnNetworkReady()
        {
            _manager.OnClientDisconnectCallback += OnClientDisconnect;
        }

        private void OnClientDisconnect(ulong clientId)
        {
            if (_clientIdToAuthId.TryGetValue(clientId, out string authId))
            {
                _clientIdToAuthId.Remove(clientId);
                _authIdToUserData.Remove(authId);
            }
        }

        public void Dispose()
        {
            if (_manager != null)
            {
                _manager.ConnectionApprovalCallback -= ApprovalCheck;
                _manager.OnServerStarted -= OnNetworkReady;
                _manager.OnClientDisconnectCallback -= OnClientDisconnect;

                if (_manager.IsListening)
                {
                    _manager.Shutdown();
                }
            }
        }
    }
}