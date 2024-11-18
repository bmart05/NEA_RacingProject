using System;
using System.Collections;
using Cinemachine;
using Core.Position;
using Core.Position.Checkpoints;
using Networking.Host;
using Networking.Shared;
using Unity.Collections;
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
        public bool hasFinished = false;

        public NetworkVariable<FixedString32Bytes> PlayerName { get; private set; } =
            new NetworkVariable<FixedString32Bytes>();

        public override void OnNetworkSpawn()
        {
            //RaceManager.Instance.InitializePlayer(this); //this fails to call as player is spawned before the game scene loads
            //this will fix itself once i put race logic in as then player spawning will be handled by the playermanager
            if (IsOwner)
            {
                //network player will have the highest priority camera so it will always be the one selected
                carCamera.Priority = ownerCamPriority; 
            }

            if (IsHost)
            {
                UserData userData =
                    HostSingleton.Instance.GameManager.NetworkServer.GetUserDataFromClientId(OwnerClientId);
                PlayerName.Value = userData.userName;
            }
        }

        private void Start()
        {
            position.playerName = PlayerName.Value.ToString();
        }

        private void Update()
        {
            position.racePosition = RaceManager.Instance.GetPosition(this);
            if (IsOwner) 
            {
                RaceUI.Instance.UpdatePositionText(position);
            }
        }

        public void SetCanMove(bool value)
        {
            carController.SetCanMove(value);
        }

        public void SetFinished()
        {
            hasFinished = true;
        }

        [ServerRpc(RequireOwnership = false)]
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