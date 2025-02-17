using System;
using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Items
{
    [CreateAssetMenu(menuName = "New Item", fileName = "New Item")]
    [Serializable]
    public class Item : ScriptableObject
    {
        public string name;
        public Sprite itemImage;
        public AudioClip itemSoundEffect;
        public int maxUses;
        public List<Tag> tags = new List<Tag>();

        public bool HasTag(string tagName)
        {
            return tags.Exists(t => t.name == tagName);
        }
        
        public bool HasTag(string tagName, out Tag tag)
        {
            tag = null;
            foreach (var t in tags)
            {
                if (t.name == tagName)
                {
                    tag = t;
                }
            }

            return (tag != null);
        }
        public bool HasTag(Tag tag)
        {
            return tags.Contains(tag);
        }
    }
}