using System;
using System.Collections;
using Cinemachine;
using Core.Position;
using Core.Position.Checkpoints;
using Unity.Netcode;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;

namespace Core.Player
{
    public class CarPlayer : NetworkBehaviour
    {
        [Header("References")]
        [SerializeField] private CinemachineVirtualCamera carCamera;
        [SerializeField] private CarController carController;
        [SerializeField] private Animator carModelAnimator;

        [Header("Settings")] 
        [SerializeField] private int ownerCamPriority = 20;
        [SerializeField] private float stunTime = 1.5f;
        
        
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

        [ServerRpc]
        public void StunPlayerServerRpc()
        {
            //doesn't need to run on server right now
            StunPlayerClientRpc();
        }
        [ClientRpc]
        public void StunPlayerClientRpc()
        {
            StartCoroutine(StunPlayer());
        }

        private IEnumerator StunPlayer()
        {
            carController.SetCanMove(false);
            carModelAnimator.SetBool("isStunned", true);
            yield return new WaitForSeconds(stunTime);
            carModelAnimator.SetBool("isStunned", false);
            carController.SetCanMove(true);
        }
    }
}