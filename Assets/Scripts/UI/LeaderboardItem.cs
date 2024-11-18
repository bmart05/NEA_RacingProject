using System;
using Core.Position;
using TMPro;
using UnityEngine;

namespace UI
{
    public class LeaderboardItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text racePositionText;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private TMP_Text trackTimeText;

        private ulong _clientId;
        private RacePosition _position;
        
        public void Initialize(ulong clientId, RacePosition position)
        {
            _clientId = clientId;
            _position = position;

            racePositionText.text = $"{position.racePosition.ToString()}.";
            playerNameText.text = _position.playerName;
            TimeSpan timeSpan = TimeSpan.FromSeconds(position.finishingTime);
            trackTimeText.text = $"{timeSpan.Minutes:00}:{timeSpan.Seconds:00}:{(timeSpan.Milliseconds/10):00}";
        }
    }
}