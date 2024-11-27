using UnityEngine;

namespace UI
{
    [CreateAssetMenu(menuName = "New Map", fileName="New Map")]
    public class MapInfo : ScriptableObject
    {
        public string mapName;
        public string sceneName;
        public Sprite mapImage;
    }
}