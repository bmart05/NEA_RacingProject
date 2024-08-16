using System;
using Core.Player;
using UnityEngine;

namespace Core
{
    public class BoostPad : MonoBehaviour
    {
        [SerializeField] private float boostStrength = 1.5f;
        [SerializeField] private float boostTime = 3f;

        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarController carController))
            {
                if (carController.IsOwner)
                {
                    carController.HandleBoostServerRpc(boostStrength,boostTime);
                }
            }
        }
    }
}