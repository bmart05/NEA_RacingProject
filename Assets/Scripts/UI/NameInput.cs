using System;
using TMPro;
using UnityEngine;

namespace UI
{
    public class NameInput : MonoBehaviour
    {
        [SerializeField] private TMP_InputField playerNameField;
        
        public const string PlayerNameKey = "PlayerName";
        
        private void Start()
        {
            playerNameField.text = PlayerPrefs.GetString(PlayerNameKey);
        }

        public void HandleNameChanged()
        {
            PlayerPrefs.SetString(PlayerNameKey, playerNameField.text);
        }
    }
}