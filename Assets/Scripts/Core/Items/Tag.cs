using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Items
{
    [Serializable]
    [CreateAssetMenu(menuName = "New Tag", fileName = "New Tag")]
    public class Tag : ScriptableObject
    {
        public string name;
        public float value1;
        public float value2;
        public GameObject clientGameObject;
        public GameObject serverGameObject;
    }
}