using System;
using Networking.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace UI
{
    public class MapPanel : MonoBehaviour
    {
        [SerializeField] private MapInfo mapInfo;
        [SerializeField] private Image mapImage;
        [SerializeField] private TMP_Text mapNameText;

        private void Start()
        {
            mapImage.sprite = mapInfo.mapImage;
            mapNameText.text = mapInfo.mapName;
        }

        public void LoadMap()
        {
            SceneManager.LoadScene(mapInfo.sceneName);
        }

        public void VoteMap()
        {
            LobbyManager.Instance.SelectMap(mapInfo.sceneName);
        }
    }
}