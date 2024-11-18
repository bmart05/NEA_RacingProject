using System;
using Core.Position;
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

            foreach (var playerPosition in RaceManager.Instance.FinishingPositions)
            {
                //highlight it if it is yours
                LeaderboardItem item = Instantiate(leaderboardItemPrefab, leaderboardItemParent);
                item.Initialize(playerPosition.Key,playerPosition.Value);
            }
        }
    }
}