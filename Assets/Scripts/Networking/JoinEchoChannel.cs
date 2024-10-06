using System;
using Unity.Services.Vivox;
using UnityEngine;

namespace Networking
{
    public class JoinEchoChannel : MonoBehaviour
    {
        private async void Start()
        {
            await VivoxService.Instance.InitializeAsync();
            await VivoxService.Instance.LoginAsync();
            await VivoxService.Instance.JoinEchoChannelAsync("ChannelName", ChatCapability.AudioOnly);
        }
    }
}