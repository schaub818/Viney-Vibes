// --------------------------------------------
// Written by Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    public class TutorialSpriteController : MonoBehaviour
    {
        public TutorialManager tutorialManager;

        public void ShowGameOver()
        {
            tutorialManager.ShowGameOver();
        }
    }
}