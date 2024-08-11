using System;
using System.Collections;
using Core.Player;
using Unity.VisualScripting;
using UnityEngine;

namespace Core.Items
{
    public class ItemBox : MonoBehaviour
    {
        [SerializeField] private GameObject itemModel;
        private bool canCollect = true;
        private const float ItemRespawnTime = 5f;
        
        private void OnTriggerEnter(Collider other)
        {
            if (!canCollect)
            {
                return;
            }
            Debug.Log($"Triggered by {other.name}");
            if (other.TryGetComponent(out ItemHandler itemHandler))
            {
                itemHandler.HandlePickup();
                StartCoroutine(HandleCollect());
            }
        }

        private IEnumerator HandleCollect()
        {
            itemModel.SetActive(false);
            canCollect = false;
            yield return new WaitForSeconds(ItemRespawnTime);
            canCollect = true;
            itemModel.SetActive(true);
        }
    }
}