using UnityEngine;

namespace Core.Items
{
    [CreateAssetMenu(menuName = "New Tag", fileName = "New Tag")]
    public class Tag : ScriptableObject
    {
        public string name;
    }
}