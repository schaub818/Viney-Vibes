// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Controls the display and properties of a single grid box
    public class GridBox : MonoBehaviour
    {
        // Has a piece been placed into this box?
        public bool Occupied
        {
            get { return occupied; }
            set 
            { 
                occupied = value;

                // Hide the box's sprite if the box is occupied
                GetComponent<SpriteRenderer>().enabled = !occupied;
            }
        }

        // Can a piece be placed into this box?
        public bool ValidTarget
        {
            get { return validTarget; }
            set 
            {
                validTarget = value;

                // If this box is a valid target, enable its trigger collider
                if (validTarget)
                {
                    GetComponent<Collider2D>().enabled = true;
                }
                else
                {
                    GetComponent<Collider2D>().enabled = false;
                }
            }
        }

        // Is this box occupied by an obstacle?
        public bool HasObstacle
        {
            get { return hasObstacle; }
            set 
            { 
                hasObstacle = value;
                Occupied = value;
            }
        }

        // Does this box contain a pickup?
        public bool HasPickup
        {
            get { return hasPickup; }
            set { hasPickup = value; }
        }
        
        // The grid X coordinate of this box
        public int X
        {
            get { return x; }
        }

        // The grid Y coordinate of this box
        public int Y
        {
            get { return y; }
        }

        // Backing fields for the bool properties
        private bool occupied;
        private bool validTarget;
        private bool hasObstacle;
        private bool hasPickup;

        // Backing fields for the int properties
        private int x;
        private int y;

        // sets the coordinates of the grid box
        public void SetCoords(int xCoord, int yCoord)
        {
            x = xCoord;
            y = yCoord;
        }

    }
}