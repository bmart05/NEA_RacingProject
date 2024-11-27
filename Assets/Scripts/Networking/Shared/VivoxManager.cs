using System;
using System.Linq;
using System.Threading.Tasks;
using UI;
using Unity.Services.Vivox;
using UnityEngine;

namespace Networking.Shared
{
    public class VivoxManager : MonoBehaviour
    {
        private static VivoxManager _instance;
        
        public static VivoxManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<VivoxManager>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }

        private bool _isLoggedIn;
        private bool _isMuted;
        [SerializeField] private bool isEnabled;
        private string _currentChannelName;

        private void Start()
        {
            DontDestroyOnLoad(gameObject);
        }

        private async Task LoginAsync()
        {
            if (_isLoggedIn || !isEnabled)
            {
                return;
            }
            
            var loginOptions = new LoginOptions()
            {
                DisplayName = PlayerPrefs.GetString(NameInput.PlayerNameKey, "Anonymous Player"),
                EnableTTS = true
            };
            await VivoxService.Instance.LoginAsync(loginOptions);
            _isLoggedIn = true;

        }
         
        public async Task JoinGroupChannel(string channelName)
        {
            if (!_isLoggedIn || !isEnabled)
            {
                await LoginAsync();
            }

            _currentChannelName = channelName;
            await VivoxService.Instance.JoinGroupChannelAsync(channelName,ChatCapability.AudioOnly);
        }

        public async Task LeaveCurrentChannel()
        {
            await VivoxService.Instance.LeaveChannelAsync(_currentChannelName);
        }

        public void ToggleInput()
        {
            if (_isMuted)
            {
                Debug.Log($"Muted yourself");
                VivoxService.Instance.UnmuteInputDevice();
                _isMuted = false;
            }
            else
            {
                Debug.Log($"Unmuted yourself");
                VivoxService.Instance.MuteInputDevice();
                _isMuted = true;
            }
        }

        public void MutePlayerLocally(string playerId)
        {
            Debug.Log($"Muted {playerId}");
            VivoxService.Instance.ActiveChannels[_currentChannelName].First(participant => participant.PlayerId == playerId).MutePlayerLocally();
        }
        public void UnmutePlayerLocally(string playerId)
        {
            Debug.Log($"Unmuted {playerId}");
            VivoxService.Instance.ActiveChannels[_currentChannelName].First(participant => participant.PlayerId == playerId).UnmutePlayerLocally();
        }
    }
}