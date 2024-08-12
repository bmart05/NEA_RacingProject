using Unity.Netcode;
using UnityEngine;

namespace Utilities
{
    public class OnlyShowForOwner : NetworkBehaviour
    {
        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                gameObject.SetActive(false);
            }
        }
    }
}