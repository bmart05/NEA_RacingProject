using System;
using System.Collections;
using Cinemachine;
using Core.Cars;
using Core.Game;
using Core.Position;
using Core.Position.Checkpoints;
using Networking.Host;
using Networking.Shared;
using TMPro;
using UI;
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
        [SerializeField] public CinemachineVirtualCamera carCamera;
        [SerializeField] private CarController carController;
        [SerializeField] private Animator carModelAnimator;
        [SerializeField] private TMP_Text playerNameText;
        [SerializeField] private Transform playerModelParent;
        [Header("Settings")] 
        [SerializeField] private int ownerCamPriority = 20;
        [SerializeField] private float stunTime = 1.5f;
        
        
        public RacePosition position;
        public bool hasFinished = false;

        public NetworkVariable<FixedString32Bytes> PlayerName { get; private set; } =
            new NetworkVariable<FixedString32Bytes>();
        public NetworkVariable<FixedString32Bytes> ModelName { get; private set; } =
            new NetworkVariable<FixedString32Bytes>();

        public override void OnNetworkSpawn()
        {
            if (IsOwner)
            {
                //network player will have the highest priority camera so it will always be the one selected
                carCamera.Priority = ownerCamPriority; 
                playerNameText.gameObject.SetActive(false);
                
            }

            if (IsHost)
            {
                UserData userData =
                    HostSingleton.Instance.GameManager.NetworkServer.GetUserDataFromClientId(OwnerClientId);
                ModelName.Value = userData.carModelName;
                PlayerName.Value = userData.userName;
            }
        }

        private void Start()
        {
            Debug.Log($"{PlayerName.Value}: {ModelName.Value}");
            CarModel model = GlobalModels.Instance.GetModelByName(ModelName.Value.ToString());
            Instantiate(model.modelPrefab, playerModelParent);
            position.playerName = PlayerName.Value.ToString();
            playerNameText.text = PlayerName.Value.ToString();
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
            StartCoroutine(HandleFinish());
        }

        public IEnumerator HandleFinish()
        {
            //play finish animation
            yield return new WaitForSecondsRealtime(3f);
            if (IsHost)
            {
                NetworkObject.Despawn(true);
            }
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