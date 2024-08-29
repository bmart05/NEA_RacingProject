using System;
using Core.Items;
using Input;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;

namespace Core.Player
{
    public class ItemHandler : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CarController carController;
        
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
                SynchronisePickupServerRpc(null);
                return;
            }

            Tag itemTag = null;
            
            if (currentItem.HasTag("Spawner"))
            {
                if (currentItem.HasTag("Projectile", out itemTag))
                {
                    SpawnDummyProjectile(itemTag.clientGameObject,itemTag.value1);
                    HandleProjectileServerRpc();
                }     
            }

            if (currentItem.HasTag("Boost", out itemTag))
            {
                carController.HandleBoostServerRpc(itemTag.value1,itemTag.value2);
            }

            remainingUses--;
        }

        [ServerRpc]
        public void HandleProjectileServerRpc()
        {
            //parse item: stupid rpcs
            Tag tag = ParseTagFromItem(currentItem, "Projectile");
            GameObject serverProjectile = tag.serverGameObject;
            float projectileSpeed = tag.value1;
            
            GameObject projectile = Instantiate(serverProjectile, transform.position, transform.rotation);

            if (projectile.TryGetComponent(out Rigidbody rb))
            {
                rb.velocity = projectile.transform.forward * projectileSpeed;
            }
            SpawnDummyProjectileClientRpc(projectileSpeed);
        }

        [ClientRpc]
        public void SpawnDummyProjectileClientRpc(float projectileSpeed)
        {
            if (IsOwner)
            {
                return;
            }
            
            Tag tag = ParseTagFromItem(currentItem, "Projectile");
            GameObject dummyProjectile = tag.clientGameObject;
            SpawnDummyProjectile(dummyProjectile,projectileSpeed);
        }

        private void SpawnDummyProjectile(GameObject dummyProjectile, float projectileSpeed)
        {
            GameObject projectile = Instantiate(dummyProjectile, transform.position, transform.rotation);

            if (projectile.TryGetComponent(out Rigidbody rb))
            {
                rb.velocity = projectile.transform.forward * projectileSpeed;
            }
        }

        private Tag ParseTagFromItem(Item item,string tagName)
        {
            Tag tag = null;
            item.HasTag(tagName, out tag);
            if (tag == null)
            {
                Debug.LogError("Invalid tag name");
            }
            
            return tag;
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
            SynchronisePickupServerRpc(currentItem.name);
        }
        
        [ServerRpc]
        public void SynchronisePickupServerRpc(string itemName)
        {
            SynchronisePickup(itemName);
            SynchronisePickupClientRpc(itemName);
        }
        
        [ClientRpc]
        public void SynchronisePickupClientRpc(string itemName)
        {
            if (IsOwner)
            {
                return;
            }

            SynchronisePickup(itemName);
        }

        private void SynchronisePickup(string itemName)
        {
            if (itemName == null)
            {
                currentItem = null;
            }
            currentItem = GlobalItems.Instance.GetItemByName(itemName);
        }
        
        private void HandleFire(bool shouldFire)
        {
            _shouldFire = shouldFire;
        }
        
    }
}