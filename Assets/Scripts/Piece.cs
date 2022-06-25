// --------------------------------------------
// Written by Dave Schaub
// with contributions from Andrés Fernandez
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Provides basic functionality for game pieces. Should be extended for specific piece types.
    public abstract class Piece : MonoBehaviour
    {
        // Has this piece been placed in the grid?
        public bool Placed
        {
            get { return placed; }
        }

        // Is this piece currently growing its vine segment?
        public bool Growing
        {
            get { return growing; }
            set { growing = true; }
        }

        // Has this piece finished growing its vine segment?
        public bool DoneGrowing
        {
            get { return doneGrowing; }
        }

        // The row number of the highest part of this piece
        public int HighestPartY
        {
            get { return highestPartY; }
        }

        // The row number of the part that the vine is currently growing on
        public int VineY
        {
            get { return vineY; }
        }

        // The time it takes the vine to grow across a piece part
        public abstract float GrowTime
        {
            get;
            set;
        }

        // The color of the lattice sprite
        public abstract Color SpriteColor
        {
            get;
            set;
        }

        // The piece part on the opposite side of where this piece is connected to the previous
        // piece
        public abstract PiecePart EndPart
        {
            get;
        }

        // The parts contained in this piece
        public List<PiecePart> parts;

        // The backing fields for the boolean properties
        protected bool placed;
        protected bool growing;
        protected bool doneGrowing;

        // The backing field for the int properties
        protected int highestPartY;
        protected int vineY;
        
        // Initialize's the piece's fields
        public abstract void Initialize();

        // Places the piece at the specified point in the game grid
        public abstract void Place(int x, int y, int endPointX, int endPointY);

        // Removes the piece from the grid
        public abstract void Remove();

        // Sets the shape of the vine on the end part
        public abstract void SetEndPartVineType(int placedX);

        // Grows this piece's vine segment
        protected abstract void Grow();
    }
}