using System;
using System.Collections.Generic;
using System.Linq;
using Core.Game;
using Core.Player;
using Core.Position.Checkpoints;
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
        public int NumLaps { get; private set; } = 3;

        public List<Transform> startingPositions;
        

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

        private void SortPositions()
        {
            playerObjects = playerObjects
                .OrderBy(p => p.position.lapNumber)
                .ThenBy(p => p.position.checkpointNumber)
                .ThenByDescending(p => Vector3.Distance(
                    CheckpointManager.Instance.GetCheckpointPosition(p.position.checkpointNumber+1),
                    p.transform.position)).ToList();

            
        }

        private void CheckFinishedPlayers()
        {
            int total = 0;
            foreach (var player in playerObjects)
            {
                if (player.hasFinished)
                {
                    total++;
                }
            }

            if (total >= (Mathf.Floor(playerObjects.Count / 2)))
            {
                //start countdown to finish
            }
            else if (total == playerObjects.Count)
            {
                GameManager.Instance.HandleFinishGame();
            }
        }

        

        public int GetPosition(CarPlayer player)
        {
            return playerObjects.Count - playerObjects.FindIndex(x => x==player);
        }
    }
    
}