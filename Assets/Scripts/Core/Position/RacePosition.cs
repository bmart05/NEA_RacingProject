using UnityEngine.Serialization;

namespace Core.Position
{
    [System.Serializable]
    public struct RacePosition
    {
        public int racePosition;
        public int lapNumber;
        public int checkpointNumber;
    }
}