// --------------------------------------------
// Written by Dave Schaub
// with contributions from Andrés Fernandez
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Advertisements;

namespace VineyVibes
{
    // Manages the display of rewarded ads in the game over screen
    public class AdManager : MonoBehaviour, IUnityAdsListener
    {
        // gets ref to game manager to continue
        public GameManager gameManager;

        // the IOS ID of our game
        private string myGameIdIOS = "4004130";

        // the Android ID of our game
        private string myGameIdAndroid = "4004131";

        // the type of ad to display (rewarded)
        private string myVideoPlacement = "rewardedVideo";

        // whether or not the ad has started
        private bool adStarted;

        // whether or not we're on test mode
        public bool testMode = true;

        // set ad to be initialized
        private void Start()
        {
            // Register this script as the advertising system's event handler
            Advertisement.AddListener(this);

            // using #if determines if this code compiles or not
            #if UNITY_IOS
            {
                // init ios version
                Advertisement.Initialize(myGameIdIOS, testMode);
            }
            #else
            {
                // init android version
                Advertisement.Initialize(myGameIdAndroid, testMode);
            }
            #endif
        }

        // shows rewarded ad to the player
        public void ShowAd()
        {
            // check if init, ready, and has not started
            if(Advertisement.IsReady(myVideoPlacement))
            {
                // show ad
                Advertisement.Show(myVideoPlacement);
            }
            else
            {
                Debug.LogError("Ad video is not ready yet. Try again later");
            }
        }

        // Handles when the ad video finishes playing
        public void OnUnityAdsDidFinish(string placementID, ShowResult result)
        {
            switch (result)
            {
                // Log an error if the video failed to play
                case ShowResult.Failed:
                    Debug.LogError("The ad did not fininsh due to an error");

                    break;

                // Log a message if the player skipped the video
                case ShowResult.Skipped:
                    Debug.Log("Player skipped ad");

                    break;
                    
                // Continue the game if the player watched the video to the end
                case ShowResult.Finished:
                    gameManager.Continue();

                    break;
            }
        }

        // Executes code when the ad video is ready to play
        public void OnUnityAdsReady(string placementID)
        {
            // COMMENTED CODE: It might be necessary to disable this button until the ad is ready
            //gameManager.adButton.enabled = true;
        }

        // Logs an error if there is an issue with playing the video
        public void OnUnityAdsDidError(string message)
        {
            Debug.LogError(message);
        }

        // Ads startup handler. Needs to be here, but doesn't need to do anything.
        public void OnUnityAdsDidStart(string placementID)
        {
            // No action required
        }

        // Removes the ad system event handler when this script is destroyed
        public void OnDestroy()
        {
            Advertisement.RemoveListener(this);
        }
    }
}

