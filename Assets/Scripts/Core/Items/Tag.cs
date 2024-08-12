using System;
using Unity.Netcode;
using UnityEngine;

namespace Core.Items
{
    [Serializable]
    [CreateAssetMenu(menuName = "New Tag", fileName = "New Tag")]
    public class Tag : ScriptableObject, INetworkSerializable
    {
        public string name;
        public void NetworkSerialize<T>(BufferSerializer<T> serializer) where T : IReaderWriter
        {
            serializer.SerializeValue(ref name);
        }
    }
}