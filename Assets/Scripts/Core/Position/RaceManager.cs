using System;
using System.Collections.Generic;
using Core.Player;
using Unity.Netcode;

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

        public void InitalizePlayer(CarPlayer player)
        {
            playerObjects.Add(player);
        } 

        public void SortPositions() //calling this every frame could be expensive  
        {
            //sort by lap number
            //sort by checkpoint number
            //sort by distance to next checkpoint
            return 0;
        }

        public int GetPosition(CarPlayer player)
        {
            return playerObjects.FindIndex(x => x==player);
        }
    }
    
}