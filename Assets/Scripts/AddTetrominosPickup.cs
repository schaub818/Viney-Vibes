// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Adds tetronimo pieces to the player's piece queue
    public class AddTetrominosPickup : Pickup
    {
        // The number of pieces to add to the queue
        public int numberToAdd;

        // Adds pieces to the player's queue and destroys the pickup
        public override void Collect()
        {
            // sfx?
            // vfx?

            // update bool?
            collected = true;

            // add pieces
            pieceManager.AddPieces(numberToAdd);

            // destroy?
            Destroy(this.transform.gameObject);
        }
    }
}

