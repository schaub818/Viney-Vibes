// --------------------------------------------
// Written by Andrés Fernandez
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    public class AddMonominoPickup : Pickup
    {
        public override void Collect()
        {
            // sfx?
            // vfx?

            // add piece
            pieceManager.AddPieces(1);

            // update bool?
            collected = true;

            // destroy?
            Destroy(this.transform.gameObject);
        }

    }
}

