using System;
using Core.Player;
using UnityEngine;

namespace Core.Position.Checkpoints
{
    public class FinishLine : MonoBehaviour
    {
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarPlayer player))
            {
                CheckpointManager.Instance.ActivateLap(player);
            }
        }
    }
}