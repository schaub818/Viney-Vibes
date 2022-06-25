// --------------------------------------------
// Written by Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VineyVibes
{
    public class TutorialManager : MonoBehaviour
    {
        public bool Active
        {
            get { return active; }
        }

        public bool Done
        {
            get { return done; }
        }

        public GameManager gameManager;

        public GameObject tutorialUI;
        public GameObject gameOverText;

        public TextMeshProUGUI tutorialText;

        public Animator tutorialAnimator;

        public Button previousButton;
        public Button nextButton;
        public Button endButton;

        public List<string> screenTexts;

        private int currentScreen;

        private bool active;
        private bool done;

        public void StartTutorial()
        {
            if (screenTexts.Count > 0)
            {
                currentScreen = 0;
                active = true;
                done = false;

                tutorialUI.SetActive(true);
                tutorialText.text = screenTexts[0];
                tutorialAnimator.SetInteger("TutorialScreen", 0);
                previousButton.enabled = false;
                previousButton.image.enabled = false;
                nextButton.enabled = true;
                endButton.enabled = false;
                endButton.image.enabled = false;
            }
            else
            {
                Debug.LogError("There are no tutorial screens configured");
            }
        }

        public void EndTutorial()
        {
            PlayerPrefs.SetInt("TutorialPlayed", 1);

            gameManager.EndTutorial();

            tutorialUI.SetActive(false);
            active = false;
            done = true;
        }

        public void NextScreen()
        {
            currentScreen++;

            tutorialText.text = screenTexts[currentScreen];
            tutorialAnimator.SetInteger("TutorialScreen", currentScreen);

            if (currentScreen == screenTexts.Count - 1)
            {
                nextButton.enabled = false;
                nextButton.image.enabled = false;
                endButton.enabled = true;
                endButton.image.enabled = true;
            }

            if (!previousButton.enabled)
            {
                previousButton.enabled = true;
                previousButton.image.enabled = true;
            }
        }

        public void PreviousScreen()
        {
            currentScreen--;

            tutorialText.text = screenTexts[currentScreen];
            tutorialAnimator.SetInteger("TutorialScreen", currentScreen);

            if (currentScreen == 0)
            {
                previousButton.enabled = false;
                previousButton.image.enabled = false;
            }

            if (!nextButton.enabled)
            {
                nextButton.enabled = true;
                nextButton.image.enabled = true;
            }

            if (endButton.enabled)
            {
                endButton.enabled = false;
                endButton.image.enabled = false;
            }

            if (gameOverText.activeInHierarchy)
            {
                gameOverText.SetActive(false);
            }
        }

        public void ShowGameOver()
        {
            gameOverText.SetActive(true);
        }

        // Start is called before the first frame update
        private void Start()
        {
            currentScreen = 0;
            active = false;
        }
    }
}