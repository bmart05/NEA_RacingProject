using System;
using System.Collections.Generic;
using Core.Player;
using Unity.Netcode;
using UnityEngine;

namespace Core.Position.Checkpoints
{
    public class CheckpointManager : NetworkBehaviour
    {
        private static CheckpointManager _instance;

        public static CheckpointManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<CheckpointManager>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }
        
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

        public void ActivateCheckpoint(Checkpoint checkpoint,CarPlayer player)
        {
            if ((checkpoint.Index + 1) == checkpoints.Count)
            {
                ActivateLap(player);
                return;
            }
            if ((checkpoint.Index-player.position.checkpointNumber)<=1)//if the checkpoint is the next checkpoint or a previous one
            {
                player.position.checkpointNumber = checkpoint.Index;
                Debug.Log($"Activated checkpoint {checkpoint.Index}");
            }
            else
            {
                Debug.Log("Stop trying to skip the track!");
            }
        }

        public void ActivateLap(CarPlayer player)
        {
            Debug.Log("Attemped to finish lap");
            if ((player.position.checkpointNumber+2) == checkpoints.Count)
            {
                player.position.lapNumber++;
                player.position.checkpointNumber = 0;
                Debug.Log("Finished lap");
                if (player.position.lapNumber == (RaceManager.Instance.NumLaps+1))
                {
                    player.SetFinished();
                }
            }
        }
    }
}