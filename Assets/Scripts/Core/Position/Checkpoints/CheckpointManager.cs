using System.Collections.Generic;
using Unity.Netcode;
using UnityEngine;

namespace Core.Position.Checkpoints
{
    public class CheckpointManager : NetworkBehaviour
    {
        [SerializeField] private List<Checkpoint> checkpoints = new List<Checkpoint>();

        public override void OnNetworkSpawn()
        {
            for (int i = 0; i < checkpoints.Count; i++)
            {
                checkpoints[i].Initialize(i);
            }
        }

        public Vector3 GetCheckpointPosition(int index)
        {
            return checkpoints[index].transform.position;
        }

        public void ActivateCheckpoint(Checkpoint checkpoint,RacePosition position)
        {
            if (position.checkpointNumber < checkpoint.Index)
            {
                position.checkpointNumber = checkpoint.Index;
                Debug.Log($"Activated checkpoint {checkpoint.Index}");
            }
            else
            {
                Debug.Log("Already activated this!");
            }
        }
    }
}