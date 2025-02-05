using System;
using Core.Position;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace UI
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text racePositionText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text trackTimeText;
        [SerializeField] private Image itemPanel;

        [SerializeField] private Color ownerColor;

        private ulong _clientId;
        private RacePosition _position;
        
        public void Initialize(ulong clientId, RacePosition position, int finishingPlace, bool isMine)
        {
            _clientId = clientId;
            _position = position;

            racePositionText.text = $"{finishingPlace.ToString()}.";
            playerNameText.text = _position.playerName;
            TimeSpan timeSpan = TimeSpan.FromSeconds(position.finishingTime);
            trackTimeText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{(timeSpan.Milliseconds/10):00}";

            if (isMine)
            {
                itemPanel.color = ownerColor;
            }
        }
        public void Initialize(int rank, string playerName, float time,bool isMine)
        {
            racePositionText.text = $"{rank.ToString()}.";
            playerNameText.text = playerName;
            TimeSpan timeSpan = TimeSpan.FromSeconds(time);
            trackTimeText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{(timeSpan.Milliseconds/10):00}";

            if (isMine)
            {
                itemPanel.color = ownerColor;
            }
        }
    }
}