using Cinemachine;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class CameraPrioritySetter : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera carCamera;

        [Header("Settings")] 
        [SerializeField] private int ownerCamPriority = 20;
        
        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                //network player will have the highest priority camera so it will always be the one selected
                carCamera.Priority = ownerCamPriority; 
            }
        }
    }
}