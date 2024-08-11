using System.Collections.Generic;
using UnityEngine;

namespace Core.Items
{
    [CreateAssetMenu(menuName = "New Item", fileName = "New Item")]
    public class Item : ScriptableObject
    {
        public string name;
        public int maxUses;
        public List<Tag> tags = new List<Tag>();

        public bool HasTag(string tagName)
        {
            return tags.Exists(t => t.name == tagName);
        }
        public bool HasTag(Tag tag)
        {
            return tags.Contains(tag);
        }
    }
}