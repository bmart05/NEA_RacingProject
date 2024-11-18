using Unity.Collections;
using UnityEngine.Serialization;

namespace Core.Position
{
    [System.Serializable]
    public struct RacePosition
    {
        public string playerName;
        public int racePosition;
        public int lapNumber;
        public int checkpointNumber;
        public float finishingTime;
    }
}