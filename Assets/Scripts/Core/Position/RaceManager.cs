using System;
using System.Collections.Generic;
using Core.Player;
using Core.Position.Checkpoints;
using Unity.Netcode;
using UnityEngine;

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

        private void Update()
        {
            SortPositions();
        }

        public void InitializePlayer(CarPlayer player)
        {
            playerObjects.Add(player);
        } 

        public void SortPositions() //really expensive function, need to optimise
        {
            for (int i = 0; i < playerObjects.Count-1; i++)
            {
                bool swapped = false;
                
                for (int j = 0; j < playerObjects.Count-i-1; j++)
                {
                    CarPlayer player = playerObjects[j];
                    CarPlayer otherPlayer = playerObjects[j+1];
                    if (otherPlayer.position.lapNumber > player.position.lapNumber)
                    {
                        swapped = true;
                    }
                    else if (otherPlayer.position.checkpointNumber > player.position.checkpointNumber)
                    {
                        swapped = true;
                    }
                    else if (otherPlayer.position.checkpointNumber == player.position.checkpointNumber)
                    {
                        Vector3 nextCheckpointPosition =
                            CheckpointManager.Instance.GetCheckpointPosition(player.position.checkpointNumber + 1);
                        float playerDistance =
                            Vector3.Distance(nextCheckpointPosition,
                                player.transform.position);
                        float otherPlayerDistance =
                            Vector3.Distance(nextCheckpointPosition,
                                otherPlayer.transform.position);
                        if (otherPlayerDistance > playerDistance)
                        {
                            swapped = true;
                        }
                    }

                    if (swapped)
                    {
                        playerObjects[j] = otherPlayer;
                        playerObjects[j+1] = player;
                    }
                }

                if (!swapped)
                {
                    return;
                }
            }
        }

        public int GetPosition(CarPlayer player)
        {
            return playerObjects.FindIndex(x => x==player);
        }
    }
    
}