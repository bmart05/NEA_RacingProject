using System;
using Core.Player;
using Unity.Netcode;
using UnityEngine;

namespace Utilities
{
    public class StunOnContact : MonoBehaviour
    {
        public ulong OwnerClientId;
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarPlayer player))
            {
                ulong hitClientId = player.OwnerClientId;
                if (hitClientId == OwnerClientId)
                {
                    return;
                }
                player.StunPlayerServerRpc();
            }
        }
    }
}