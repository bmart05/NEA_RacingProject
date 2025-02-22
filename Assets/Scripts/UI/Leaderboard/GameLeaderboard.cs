using System;
using System.Linq;
using Core.Position;
using Unity.Netcode;
using UnityEngine;

namespace UI
{
    public class GameLeaderboard : MonoBehaviour
    {
        [SerializeField] private LeaderboardItem leaderboardItemPrefab;
        [SerializeField] private Transform leaderboardItemParent;
        

        private void Start()
        {
            UpdateLeaderboard();
        }

        private void UpdateLeaderboard()
        {
            foreach (Transform child in leaderboardItemParent)
            {
                Destroy(child.gameObject);
            }

            int position = 1;
            foreach (var playerPosition in RaceManager.Instance.FinishingPositions)
            {
                LeaderboardItem item = Instantiate(leaderboardItemPrefab, leaderboardItemParent);
                item.Initialize(playerPosition.Key,playerPosition.Value, position,(NetworkManager.Singleton.LocalClientId == playerPosition.Key));
                position++;
            }
            
        }
    }
}