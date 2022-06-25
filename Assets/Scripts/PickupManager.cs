// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Manages placing pickups in the game grid
    public class PickupManager : MonoBehaviour
    {
        
        // talks with the grid manager to determine if a tile is occupied
        public GridManager gridManager;
        
        // talks with piece manager to determine current highest piece
        public PieceManager pieceManager;
        
        // list of pickups determined through inspector
        public List<Pickup> pickups;

        // number of pickups to add to the board
        public int pickupsToAdd;

        // how far away from the player should the pickup spawn?
        public int verticalOffset = 5;

        // this represents how many times the pickup can be generated
        // and fail to get placed; it's like a "fail streak".
        // a stubbornness of "100" means the pickup manager will try
        // place a pickup 100 times and if it fails on the 100th, it will stop placing any more pickups
        public int stubbornness = 20;

        // Adds starting pickups into the game grid
        public void Initialize()
        {
            // grab bounds
            int[] bounds = { verticalOffset, gridManager.Height - 1};

            // add pickups
            AddPickups(pickupsToAdd, bounds);
        }

        // spawn set pickups within certain bounds
        public void AddPickups(int[] bounds)
        {
            // calls other function
            AddPickups(pickupsToAdd, bounds);
        }
        
        // spawns number of pickups in a given range
        public void AddPickups(int numberOfPickups, int[] bounds)
        {
            // init pickups on board to 0
            int pickupsOnBoard = 0;

            int failedAttemptStreak = 0;
            
            // place pickups pickups
            while(pickupsOnBoard < numberOfPickups)
            {
                // establish lower and upper bound to spawn pickup
                int lowerBound = bounds[0];
                int upperBound = bounds[1];

                // fetch coordinates 
                int posX = Random.Range(0, gridManager.Width);
                int posY = Random.Range(lowerBound, upperBound);

                // check to see if this space (posX, posY) is occupied, has a pickup, or has an obstacle
                if(!gridManager.Boxes[posX][posY].Occupied &&
                   !gridManager.Boxes[posX][posY].HasPickup &&
                   !gridManager.Boxes[posX][posY].HasObstacle)
                {
                    // mark this space as containing a pickup
                    gridManager.Boxes[posX][posY].HasPickup = true;

                    // generate random pickup index
                    int pickupIndex = Random.Range(0, pickups.Count);
                    
                    // generate a new pickup
                    Pickup randomPickup = Instantiate(
                        pickups[pickupIndex], 
                        gridManager.Boxes[posX][posY].transform.position, 
                        Quaternion.identity
                    );

                    // nest in grid manager to fetch later
                    randomPickup.transform.parent = gridManager.Boxes[posX][posY].transform;
                    
                    // initialize values
                    randomPickup.Initialize(posX, posY);

                    // update count
                    pickupsOnBoard++;
                }
                else
                {
                    failedAttemptStreak++;
                }

                // Exits the function if a valid spot has not been found in the specified number of
                // tries
                if(failedAttemptStreak > stubbornness)
                {
                    Debug.LogWarning(
                        "Failed to generate pickups " + stubbornness + " times in a row\n" + 
                        "Halting Add Pickups"
                    );
                    
                    return;
                }
            }
        }
    }
}