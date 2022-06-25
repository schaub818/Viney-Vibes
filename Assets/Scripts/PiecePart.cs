// --------------------------------------------
// Written by Dave Schaub
// with contributions from Andrés Fernandez
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // A single part of a game piece. Pieces are made up of one or more parts.
    public class PiecePart : MonoBehaviour
    {
        // The grid X coordinate of this part.
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        // The grid Y coordinate of this part.
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        // The local X coordinate of this part. Used for calculating the grid X coordinate after
        // this part's piece has been placed.
        public int OffsetX
        {
            get { return offsetX; }
            set { offsetX = value; }
        }

        // The local X coordinate of this part. Used for calculating the grid X coordinate after
        // this part's piece has been placed.
        public int OffsetY
        {
            get { return offsetY; }
            set { offsetY = value; }
        }

        // The amount of time it takes this part to grow its vine segment.
        public float GrowTime
        {
            get { return vine.GrowTime; }
            set { vine.GrowTime = value; }
        }

        // Can this part be connected to other pieces?
        public bool Connectable
        {
            get { return connectable; }
            set { connectable = value; }
        }

        // Has this part been connected to another piece?
        public bool Connected
        {
            get { return connected; }
            set { connected = value; }
        }

        
        // Has this part been placed in the grid?
        public bool Placed
        {
            get { return placed; }
            set
            {
                placed = value;

                // If the part has been placed, remove all stored target boxes.
                if (placed)
                {
                    targetBoxes.Clear();
                }
            }
        }

        // Is this part's vine segment currently growing?
        public bool Growing
        {
            get { return vine.Growing; }
            set { vine.Growing = value; }
        }

        // Has this part's vine segment finished growing?
        public bool DoneGrowing
        {
            get { return vine.DoneGrowing; }
        }

        // Stores the valid grid placement targets that this part is currently overlapping.
        public List<GridBox> TargetBoxes
        {
            get { return targetBoxes; }
        }

        // Stores this part's vine segment.
        public Vine vine;

        // The backing fields for the int properties.
        private int x;
        private int y;
        private int offsetX;
        private int offsetY;

        // The backing fields for the bool properties.
        private bool connectable;
        private bool connected;
        private bool placed;

        // The backing field for the TargetBoxes property.
        private List<GridBox> targetBoxes;

        public void Initialize()
        {
            // Initializes the private int fields.
            x = -1;
            y = -1;
            offsetX = -1;
            offsetY = -1;

            // Initializes the private bool fields.
            connectable = false;
            connected = false;
            placed = false;

            // Initializes the target boxes list.
            targetBoxes = new List<GridBox>();
        }

        // Stores any target placement grid boxes that this part is currently overlapping.
        private void OnTriggerEnter2D(Collider2D collision)
        {
            // Only store target boxes if this part has not been placed in the grid.
            if (connectable && !placed)
            {
                // If the currently overlapped grid box is a valid target, store it.
                if (collision.gameObject.GetComponent<GridBox>().ValidTarget)
                {
                    targetBoxes.Add(collision.gameObject.GetComponent<GridBox>());
                }
            }
        }

        // Removes any target placement grid boxes that this part has stopped overlapping.
        private void OnTriggerExit2D(Collider2D collision)
        {
            // If the box this piece stopped overlapping is a valid target, remove it from the
            // list.
            if (connectable && collision.gameObject.GetComponent<GridBox>().ValidTarget)
            {
                targetBoxes.Remove(collision.gameObject.GetComponent<GridBox>());
            }
        }
    }
}