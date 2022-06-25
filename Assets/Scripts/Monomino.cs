using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Manages single-square game pieces
    public class Monomino : Piece
    {
        // The time it takes the vine to grow across the piece
        public override float GrowTime
        { 
            get => throw new System.NotImplementedException();
            set => throw new System.NotImplementedException();
        }

        // The color of the lattice piece
        public override Color SpriteColor 
        { 
            get => throw new System.NotImplementedException(); 
            set => throw new System.NotImplementedException(); 
        }

        // The last part of the piece. Required to implement the base Piece class.
        public override PiecePart EndPart
        {
            get => throw new System.NotImplementedException();
        }

        // Initializes the piece's fields
        public override void Initialize()
        {
            throw new System.NotImplementedException();
        }

        // Places the piece in the game grid
        public override void Place(int x, int y, int endPointX, int endPointY)
        {
            throw new System.NotImplementedException(); 
        }

        // Removes the piece from the game grid
        public override void Remove()
        {
            throw new System.NotImplementedException();
        }

        // Sets the shape of the vine as it grows across the end part
        public override void SetEndPartVineType(int placedX)
        {
            throw new System.NotImplementedException();
        }

        // Grows the vine across the piece
        protected override void Grow()
        {
            throw new System.NotImplementedException();
        }

        // Start is called before the first frame update
        private void Start()
        {

        }

        // Update is called once per frame
        private void Update()
        {

        }
    }
}