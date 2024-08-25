using System;
using Cinemachine;
using Core.Position;
using Core.Position.Checkpoints;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Player
{
    public class CarPlayer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera carCamera;

        [SerializeField] private Renderer carModelRenderer;

        [Header("Settings")] 
        [SerializeField] private int ownerCamPriority = 20;
        
        
        public RacePosition position;
        public override void OnNetworkSpawn()
        {
            RaceManager.Instance.InitializePlayer(this);
            if (IsOwner)
            {
                //network player will have the highest priority camera so it will always be the one selected
                carCamera.Priority = ownerCamPriority; 
            }
        }

        private void Update()
        {
            if (IsOwner) //no need to do this for other clients, as we don't need to know their position, just where we are
            {
                position.racePosition = RaceManager.Instance.GetPosition(this);
                RaceUI.Instance.UpdateText(position.racePosition);
            }
        }
    }
}