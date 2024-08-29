using System;
using Core.Player;
using UnityEngine;

namespace Utilities
{
    public class StunOnContact : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarPlayer player))
            {
                player.StunPlayerServerRpc();
            }
        }
    }
}