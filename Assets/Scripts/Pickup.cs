// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // The base class for bonus pickups
    public abstract class Pickup : MonoBehaviour
    {   
        // pickup takes a reference to piece manager
        // in order to update the piece count
        public PieceManager pieceManager;

        // The grid X coordinate where the pickup is located
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        // The grid Y coordinate where the pickup is located
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        // Has this pickup been collected
        public bool Collected
        {
            get { return collected; }
            set { collected = value; }
        }

        // The backing fields for the public properties
        protected bool collected;
        protected int x;
        protected int y;

        // Collects this piece
        public virtual void Collect()
        {
            return;
        }

        // Initializes this piece's fields
        public void Initialize(int posX, int posY)
        {
            x = posX;
            y = posY;
            collected = false;
        }
    }
}
