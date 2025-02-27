using System;
using Core.Position;
using Networking.Shared;
using TMPro;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace UI
{
    public class RaceUI : MonoBehaviour
    {
        private static RaceUI _instance;
        
        public static RaceUI Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<RaceUI>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }
        
        [Header("References")] 
        [SerializeField] private TMP_Text positionText;
        [SerializeField] private TMP_Text lapNumberText;
        [SerializeField] private TMP_Text raceTimerText;
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject finishUI;

        public void UpdatePositionText(RacePosition position)
        {
            positionText.text = position.racePosition.ToString();
            lapNumberText.text = $"{(position.lapNumber).ToString()}/{RaceManager.Instance.NumLaps}";
            TimeSpan timeSpan = TimeSpan.FromSeconds(Time.realtimeSinceStartup - RaceManager.Instance.StartingTime);
            raceTimerText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{(timeSpan.Milliseconds / 10):00}";
        }

        public void ShowFinishUI()
        {
            gameUI.SetActive(false);
            finishUI.SetActive(true);
        }

        public void GoBackToLobby()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                NetworkManager.Singleton.SceneManager.LoadScene("LobbyScene",LoadSceneMode.Single);
            }
        }

        public async void LeaveLobby()
        {
            if (NetworkManager.Singleton.IsHost)
            {
                await LobbyManager.Instance.DestroyCurrentLobby();
            }
            else
            {
                await LobbyManager.Instance.LeaveCurrentLobby();
            }
        }
    }
}