using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Core.Player;
using Core.Position.Checkpoints;
using UI;
using Unity.Netcode;
using UnityEngine;
using Utilities;
using Random = System.Random;

namespace Core.Position
{
    public class RaceManager : NetworkBehaviour
    {
        private static RaceManager _instance;

        public static RaceManager Instance
        {
            get
            {
                if (_instance != null)
                {
                    return _instance;
                }

                _instance = FindObjectOfType<RaceManager>();
                if (_instance == null)
                {
                    return null;
                }

                return _instance;
            }
        }

        public List<CarPlayer> playerObjects;
        [field: SerializeField] public int NumLaps { get; private set; } = 3;

        public List<Transform> startingPositions;
        public readonly Dictionary<ulong, RacePosition> FinishingPositions = new Dictionary<ulong, RacePosition>();
        public float StartingTime { get; private set; }
        public float LocalFinishTime { get; private set; }
        public const float DNFTime = 9999f; //in seconds, just under 3 hours so it will never be reached
        

        private void Update()
        {
            SortPositions();
            CheckFinishedPlayers();
        }

        public void InitializePlayer(CarPlayer player)
        {
            playerObjects.Add(player);
            player.position.lapNumber = 0;
            player.position.checkpointNumber = 0;
        }

        public Transform GetStartingPosition(int index)
        {
            return startingPositions[index];
        }

        public void SetStartingTime()
        {
            StartingTime = Time.realtimeSinceStartup;
        }

        private void SortPositions()
        {
            if (!GameManager.Instance.HasGameStarted.Value || GameManager.Instance.HasGameFinished.Value)
            {
                return;
            }
            playerObjects = playerObjects
                .OrderBy(p => p.position.lapNumber)
                .ThenBy(p => p.position.checkpointNumber)
                .ThenByDescending(p => Vector3.Distance(
                    CheckpointManager.Instance.GetCheckpointPosition(p.position.checkpointNumber+1),
                    p.transform.position)).ToList();

        }

        public void FinishPlayer(CarPlayer carPlayer)
        {
            carPlayer.position.finishingTime = Time.realtimeSinceStartup - StartingTime;
            if (carPlayer.IsOwner)
            {
                LocalFinishTime = carPlayer.position.finishingTime;
                Countdown.Instance.ShowFinish();
            }
            FinishingPositions.Add(carPlayer.OwnerClientId,carPlayer.position);
            playerObjects.Remove(carPlayer);
            carPlayer.SetFinished();
        }
        
        public void FinishPlayerDnf(CarPlayer carPlayer)
        {
            carPlayer.position.finishingTime = 5940f; //99mins and 99 seconds to make sure it will always be at the bottom of the list
            if (carPlayer.IsOwner)
            {
                LocalFinishTime = 5940f; //same here
                Countdown.Instance.ShowFinish();
            }
            FinishingPositions.Add(carPlayer.OwnerClientId,carPlayer.position);
            playerObjects.Remove(carPlayer);
            carPlayer.SetFinished();
        }

        private void CheckFinishedPlayers()
        {
            if (!IsHost)
            {
                return;
            }
            if (!GameManager.Instance.HasGameStarted.Value && !GameManager.Instance.HasGameFinished.Value)
            {
                return;
            }

            int total = FinishingPositions.Count;
            
            // //start countdown to finish if over half of the players have finished
            // if (total == Mathf.Round(GameManager.Instance.NumPlayers.Value * 0.5f)) //ensures it will never be a decimal value by rounding it
            // {
            //     GameManager.Instance.StartFinishCountdownServerRpc();
            // }
            
            if (total == GameManager.Instance.NumPlayers.Value)
            {
                GameManager.Instance.HandleFinishGameServerRpc();
            }
        }
        
        public int GetPosition(CarPlayer player)
        {
            return playerObjects.Count - playerObjects.FindIndex(x => x==player);
        }
    }
    
}