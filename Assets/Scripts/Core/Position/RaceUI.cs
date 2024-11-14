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
        [SerializeField] private GameObject gameUI;
        [SerializeField] private GameObject finishUI;

        public void UpdatePositionText(RacePosition position)
        {
            positionText.text = position.racePosition.ToString();
            lapNumberText.text = $"{(position.lapNumber+1).ToString()}/{RaceManager.Instance.NumLaps}";
        }

        public void ShowFinishUI()
        {
            gameUI.SetActive(false);
            finishUI.SetActive(true);
        }
    }
}