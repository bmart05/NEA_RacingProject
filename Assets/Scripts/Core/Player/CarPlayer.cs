using Cinemachine;
using Core.Position;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class CarPlayer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera carCamera;

        public RacePosition Position { get; set; }

        [Header("Settings")] 
        [SerializeField] private int ownerCamPriority = 20;
        
        public override void OnNetworkSpawn()
        {
            Position.lapNumber = RaceManager.Instance.NumLaps;
            
            if (IsOwner)
            {
                //network player will have the highest priority camera so it will always be the one selected
                carCamera.Priority = ownerCamPriority; 
            }
        }
    }
}