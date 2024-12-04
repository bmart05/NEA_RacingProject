using System;
using Core.Game;
using Core.Player;
using Unity.Netcode;
using UnityEngine;

namespace Utilities
{
    public class Billboard : NetworkBehaviour
    {
        public Transform ownerPlayerTransform;

        private void Start()
        {
            ownerPlayerTransform = GameManager.Instance.localPlayerObject.carCamera.transform; // this should be the player
        }

        private void Update()
        {
            Quaternion previousRotation = transform.rotation;
            transform.LookAt(ownerPlayerTransform.transform);
            transform.rotation = Quaternion.Euler(previousRotation.x, transform.rotation.y, previousRotation.z);

        }
    }
}