﻿using System;
using System.Threading.Tasks;
using Networking.Client;
using Networking.Host;
using UnityEngine;

namespace Networking
{
    public class ApplicationController : MonoBehaviour
    {
        [SerializeField] private ClientSingleton clientPrefab;
        [SerializeField] private HostSingleton hostPrefab;
        
        private async void Start()
        {
            DontDestroyOnLoad(gameObject);
            await LaunchInMode(SystemInfo.graphicsDeviceType != UnityEngine.Rendering.GraphicsDeviceType.Null);
        }

        private async Task LaunchInMode(bool isDedicatedServer)
        {
            if (isDedicatedServer)
            {
                //run dedicated server code (probably wont need this) 
            }
            else // use listen server structure
            {
                ClientSingleton clientSingleton = Instantiate(clientPrefab);
                await clientSingleton.CreateClient();

                HostSingleton hostSingleton = Instantiate(hostPrefab);
                hostSingleton.CreateHost();
            }
        }
    }
}