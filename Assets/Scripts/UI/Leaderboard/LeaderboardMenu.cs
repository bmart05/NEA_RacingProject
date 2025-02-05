using Newtonsoft.Json;
using TMPro;
using Unity.Netcode;
using Unity.Services.Leaderboards;
using Unity.Services.Leaderboards.Models;
using UnityEngine;

namespace UI
{
    public class LeaderboardMenu : MonoBehaviour
    {

        [Header("References")] 
        [SerializeField] private TMP_Text mapNameText;
        [SerializeField] private Transform leaderboardParent;
        [SerializeField] private LeaderboardItem leaderboardEntryPrefab;
        
        
        [SerializeField] private MapInfo currentMapInfo;
        public void SelectMap(MapInfo info)
        {
            currentMapInfo = info;
            mapNameText.text = info.mapName;
            UpdateTimes();
        }

        public async void UpdateTimes()
        {
            foreach (Transform child in leaderboardParent)
            {
                Destroy(child.gameObject);
            }

            string leaderboardId = currentMapInfo.sceneName; // the scene name will always be the leaderboard id
            LeaderboardScoresPage scoresResponse = await LeaderboardsService.Instance.GetScoresAsync(
                leaderboardId,
                new GetScoresOptions{ IncludeMetadata = true }
            );
            foreach (var result in scoresResponse.Results)
            {
                LeaderboardItem item = Instantiate(leaderboardEntryPrefab, leaderboardParent);
                Debug.Log(result.Metadata);
                LeaderboardMetadata metadata = JsonConvert.DeserializeObject<LeaderboardMetadata>(result.Metadata);
                Debug.Log(metadata.playerName);
                item.Initialize((result.Rank+1),metadata.playerName, (float)result.Score,false);
            }
        }
    }
}