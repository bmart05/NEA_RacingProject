using System;
using Core.Items;
using Input;
using Unity.Collections;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class ItemHandler : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CarController carController;

        [Header("Settings")] 
        [SerializeField] private float boostStrength = 1.5f;
        [SerializeField] private float boostTime = 3f;
        
        [Header("Debug")]
        [SerializeField] private bool canPickup = true;
        [SerializeField] private int remainingUses = 0;
        [SerializeField] private Item currentItem;
        
        private bool _shouldFire;


        public override void OnNetworkSpawn()
        {
            if (!IsOwner)
            {
                return;
            }
            inputReader.FireEvent += HandleFire;
        }

        

        public override void OnNetworkDespawn()
        {
            if (!IsOwner)
            {
                return;
            }
            inputReader.FireEvent -= HandleFire;
            
        }

        private void Update()
        {
            if (!IsOwner)
            {
                return;
            }

            if (!_shouldFire)
            {
                return;
            }

            if (currentItem == null)
            {
                return;
            }

            if (remainingUses <= 0)
            {
                canPickup = true;
                currentItem = null;
                ItemUI.Instance.UpdateUI(null);
                return;
            }

            if (currentItem.HasTag("Projectile"))
            {
                //handle projectile code
            }

            if (currentItem.HasTag("Boost"))
            {
                StartCoroutine(carController.Boost(boostStrength, boostTime));
            }

            remainingUses--;
        }

        public void HandlePickup()
        {
            if (!IsOwner)
            {
                return;
            }
            
            if (!canPickup)
            {
                return;
            }
            canPickup = false;


            currentItem = GlobalItems.Instance.GetRandomItem();
            ItemUI.Instance.UpdateUI(currentItem);
            remainingUses = currentItem.maxUses;
        }
        
        private void HandleFire(bool shouldFire)
        {
            _shouldFire = shouldFire;
        }
        
    }
}