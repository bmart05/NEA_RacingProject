using System;
using Core.Player;
using TMPro;
using UnityEngine;

namespace Core.Position
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

        public void UpdateText(int racePosition)
        {
            positionText.text = racePosition.ToString(); 
        }

        public void UpdateLapNumber(int lapNumber)
        {
            lapNumberText.text = $"{lapNumber}/{RaceManager.Instance.NumLaps}";
        }
    }
}