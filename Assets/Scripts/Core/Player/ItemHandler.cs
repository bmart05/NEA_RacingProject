using System;
using Core.Items;
using Input;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Netcode;
using UnityEngine;
using UnityEngine.Serialization;
using Utilities;

namespace Core.Player
{
    public class ItemHandler : NetworkBehaviour
    {
        [Header("References")] 
        [SerializeField] private InputReader inputReader;
        [SerializeField] private CarController carController;
        [SerializeField] private Transform frontItemSpawnPosition;
        [SerializeField] private Transform backItemSpawnPosition;
        [SerializeField] private float fireRate = 1f;
        [SerializeField] private AudioSource fxSource;
        [SerializeField] private AudioClip collectionFx;
        
        [Header("Debug")]
        [SerializeField] private bool canPickup = true;
        [SerializeField] private int remainingUses = 0;
        [SerializeField] private Item currentItem;
        
        private bool _shouldFire;
        private float previousFireTime;


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
            
            //thanks to dapper dino - udemy tutorial
            if (Time.time < (1 / fireRate) + previousFireTime)
            {
                return;
            }


            Tag itemTag = null;
            
            if (currentItem.HasTag("Projectile", out itemTag))
            {
                SpawnDummyProjectile(itemTag.clientGameObject,itemTag.value1);
                HandleProjectileServerRpc();
            } 
            
            if (currentItem.HasTag("Obstacle", out itemTag))
            {
                SpawnObstacle();
                SpawnObstacleServerRpc();
            }

            if (currentItem.HasTag("Boost", out itemTag))
            {
                carController.HandleBoostServerRpc(itemTag.value1,itemTag.value2);
            }
            
            fxSource.PlayOneShot(currentItem.itemSoundEffect);
            
            remainingUses--;
            previousFireTime = Time.time;
        }

        [ServerRpc]
        public void HandleProjectileServerRpc()
        {
            //parse item: stupid rpcs
            Tag tag = ParseTagFromItem(currentItem, "Projectile");
            GameObject serverProjectile = tag.serverGameObject;
            float projectileSpeed = tag.value1;
            
            GameObject projectile = Instantiate(serverProjectile, frontItemSpawnPosition.position, transform.rotation);
            
            if (projectile.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = projectile.transform.forward * projectileSpeed;
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
            GameObject projectile = Instantiate(dummyProjectile, frontItemSpawnPosition.position, transform.rotation);

            if (projectile.TryGetComponent(out Rigidbody rb))
            {
                rb.linearVelocity = projectile.transform.forward * projectileSpeed;
            }
        }

        [ServerRpc]
        private void SpawnObstacleServerRpc()
        {
            Tag tag = ParseTagFromItem(currentItem, "Obstacle");
            GameObject obstaclePrefab = tag.serverGameObject;
            
            Instantiate(obstaclePrefab,backItemSpawnPosition.position,quaternion.identity);
            
            SpawnObstacleClientRpc();
        }
        [ClientRpc]
        private void SpawnObstacleClientRpc()
        {
            if (IsOwner)
            {
                return;
            }
            SpawnObstacle();
        }

        private void SpawnObstacle()
        {
            Tag tag = ParseTagFromItem(currentItem, "Obstacle");
            GameObject obstaclePrefab = tag.clientGameObject;
            
            Instantiate(obstaclePrefab,backItemSpawnPosition.position,quaternion.identity);
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

            fxSource.PlayOneShot(collectionFx);
            
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