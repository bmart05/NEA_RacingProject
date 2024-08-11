using System.Collections.Generic;
using UnityEngine;
using Utilities;

namespace Core.Items
{
    public class GlobalItems : MonoBehaviour
    {
        private static GlobalItems _instance;

        public static GlobalItems Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<GlobalItems>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }
        
        public List<Item> items = new List<Item>();

        public Item GetRandomItem()
        {
            return items.PickRandom();
        }
    }
    
}