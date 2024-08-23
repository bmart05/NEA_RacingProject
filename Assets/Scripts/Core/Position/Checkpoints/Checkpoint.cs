using System;
using Core.Player;
using UnityEngine;

namespace Core.Position.Checkpoints
{
    public class Checkpoint : MonoBehaviour
    {
        private CheckpointManager _checkpointManager;
        public int Index { get; private set; }
        
        public void Initialize(int index)
        {
            Index = index;
        }
        
        private void OnTriggerEnter(Collider other)
        {
            if (other.TryGetComponent(out CarPlayer player))
            {
                _checkpointManager.ActivateCheckpoint(this,player.Position);
            }
        }
    }
}