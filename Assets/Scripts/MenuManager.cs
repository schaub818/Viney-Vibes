// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace VineyVibes
{
    // Controls the main and pause menus
    public class MenuManager : MonoBehaviour
    {
        // Stores the game and settings managers
        public GameManager gameManager;
        public SettingsManager settingsManager;

        // Starts a new game
        public void NewGame()
        {
            Debug.Log("Starting a new game");

            // Load the MainGame scene
            SceneManager.LoadScene("MainGame");
        }

        // Loads a saved checkpoint
        public void LoadCheckpoint()
        {
            Debug.Log("Loading Checkpoint");
        }

        // Saves game settings changes
        public void ApplySettingsChanges()
        {
            Debug.Log("Applying Settings");

            // TO DO
            // settings manager work here
        }

        // Reverts changes to game settings
        public void CancelSettingsChanges()
        {
            Debug.Log("Cancelling Settings");

            // TODO
            // settings manager work here
        }
    }
}