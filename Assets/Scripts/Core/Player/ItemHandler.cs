using System;
using Core.Items;
using Input;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class ItemHandler : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;
        

        [SerializeField] private bool canPickup = true;
        [SerializeField] private int remainingUses = 0;
        private bool _shouldFire;
        [SerializeField] private Item currentItem;
        


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
                currentItem = null;
                canPickup = true;
                return;
            }

            if (currentItem.HasTag("Projectile"))
            {
                //handle projectile code
            }

            if (currentItem.HasTag("Booster"))
            {
                //handle booster code
            }

            remainingUses--;
        }

        public void HandlePickup()
        {
            if (!canPickup)
            {
                return;
            }
            canPickup = false;


            currentItem = GlobalItems.Instance.GetRandomItem();
            remainingUses = currentItem.maxUses;
        }
        
        private void HandleFire(bool shouldFire)
        {
            _shouldFire = shouldFire;
        }
    }
}