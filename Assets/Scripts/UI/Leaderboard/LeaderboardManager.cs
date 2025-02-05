using Newtonsoft.Json;
using UnityEngine;
using Unity.Services.Leaderboards;

namespace UI
{
    public static class LeaderboardManager
    {
        private const string PlayerPrefsKey = "HighScore";

        public static void SetNewScore(string trackName,float time)
        {
            float highScore = PlayerPrefs.GetFloat($"{PlayerPrefsKey}_{trackName}", 0f);
            if (time > highScore)
            {
                Debug.Log("Setting new high score");
                PlayerPrefs.SetFloat($"{PlayerPrefsKey}_{trackName}",time); //update high score locally
                AddScoreToLeaderboard(trackName,time); //store high score in leaderboard
            }

        }
        
        public static async void AddScoreToLeaderboard(string trackName, float time)
        {
            var metadata = new LeaderboardMetadata { playerName = PlayerPrefs.GetString(NameInput.PlayerNameKey, "Anonymous Player") };
            AddPlayerScoreOptions options = new AddPlayerScoreOptions();
            options.Metadata = metadata;
            var playerEntry = await LeaderboardsService.Instance.AddPlayerScoreAsync(trackName, time, options);
            Debug.Log(JsonConvert.SerializeObject(playerEntry));
        }
    }
}