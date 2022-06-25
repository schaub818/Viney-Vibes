// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // The base class for obstacles
    public class Obstacle : MonoBehaviour
    {
        // width and height are defined in the inspector
        public int Width;
        public int Height;

        // The grid X coordinate of the top-left corner of the obstacle
        public int X
        {
            get { return x; }
            set { x = value; }
        }

        // The grid Y coordinate of the top-left corner of the obstacle
        public int Y
        {
            get { return y; }
            set { y = value; }
        }

        // The backing fields for the int properties
        private int x;
        private int y;

        // Sets the grid coordinates for the top-left corner of the obstacle
        public void SetCoords(int xIn, int yIn)
        {
            x = xIn;
            y = yIn;
        }
    }
}