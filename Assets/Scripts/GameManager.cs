// --------------------------------------------
// Written by Dave Schaub
// with contributions from Andrés Fernandez
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

namespace VineyVibes
{
    // Manages game flow and progress tracking.
    public class GameManager : MonoBehaviour
    {
        // Determines how fast the vine moves
        public int DifficultyLevel
        {
            get { return difficultyLevel; }
        }

        // Stores the highest difficulty level the player has reached.
        public int HighestLevel
        {
            get { return highestLevel; }
        }

        // The number of rows to build up before the difficulty level increases.
        public int rowsPerLevel;

        // space player is given without obstacles after game over
        public int continueBuffer;
        public int continueLevelReduction;
        public int continuePiecesToAdd;

        // How far from the bottom of the screen the end of the vine should be
        public float vineBottomOffset;

        // Indicates whether the game is currently running
        public bool Active
        {
            get { return active; }
        }

        // Indicates whether the player has paused the game
        public bool Paused
        {
            get { return paused; }
        }

        // Stores the system managers.
        public BackgroundManager backgroundManager;
        public GridManager gridManager;
        public PieceManager pieceManager;
        public PickupManager pickupManager;
        public ObstacleManager obstacleManager;
        public TutorialManager tutorialManager;

        public LevelStateManager levelStateManager;

        // Stores the scene camera.
        public Camera mainCamera;

        // Stores the UI text elements.
        public TextMeshProUGUI scoreText;
        public TextMeshProUGUI hudHighScoreText;
        public TextMeshProUGUI guiHighScoreText;

        // Stores the menu canvases
        public GameObject gameOverMenu;
        public GameObject pauseMenu;

        // Stores the UI buttons
        public Button pauseButton;
        public Button fastForwardButton;
        public Button adButton;

        // Stores the fast forward button's sprites
        public Sprite fastForwardSprite;
        public Sprite playSprite;

        // Stores the fast forward multiplier
        public float fastForwardMultiplier;

        // The backing field for the DifficultyLevel property
        private int difficultyLevel;

        // The backing variable for the HighestLevel property
        private int highestLevel;

        // the backing field for the HighestRow property
        private int highestRow;

        // Stores the current score and high score
        private int score;
        private int highScore;

        // Stores how many times the current game has been continued
        private int continueCount;

        // Sets the starting speeds of the camera and vine.
        private float cameraTop;
        private float cameraBottom;

        // Sets where the camera should move to next and how fast it should get there
        private Vector3 cameraMoveTarget;
        private Vector3 cameraSpeed;

        // are we fast forwarding?
        private bool fastForward;
        private bool active;
        private bool paused;
        private bool gameContinued;

        // Initializes the game systems.
        public void StartGame()
        {
            // Initialize the system managers
            backgroundManager.Initialize();
            gridManager.Initialize();
            pieceManager.Initialize();
            obstacleManager.Initialize();
            pickupManager.Initialize();

            // Initialize the game tracking fields
            difficultyLevel = 1;
            highestLevel = 1;
            highestRow = 0;
            score = 0;
            highScore = PlayerPrefs.GetInt("HighScore", 0);
            continueCount = 0;

            // Display the high score
            guiHighScoreText.text = "High Score\n" + highScore;

            // Set the camera zoom level based on how tall the screen is
            mainCamera.orthographicSize = backgroundManager.Width * Screen.height / Screen.width * 0.5f;

            // Set the game speed to normal speed
            Time.timeScale = 1;

            // Stores the world positions of the bottom and top of the screen
            cameraTop = mainCamera.transform.position.y + mainCamera.orthographicSize;
            cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;

            // Position the camera based on the specified offset
            float vineBottomDistance = gridManager.Boxes[0][0].transform.position.y - cameraBottom;
            float cameraYTranslation = vineBottomDistance - vineBottomOffset;

            mainCamera.transform.Translate(0.0f, cameraYTranslation, 0.0f);
            cameraMoveTarget = mainCamera.transform.position;

            // Retrieve if the tutorial has been completed
            int tutorialPlayed = PlayerPrefs.GetInt("TutorialPlayed", 0);

            // Display the tutorial if it has not been completed
            if (tutorialPlayed == 0)
            {
                tutorialManager.StartTutorial();
                active = false;
                pauseButton.interactable = false;
                fastForwardButton.interactable = false;
            }
            else
            {
                active = true;
            }

            // Set the game as active and the initial vine speed
            pieceManager.GameActive = active;
            pieceManager.PreviousFrameActive = active;
            pieceManager.SetVineGrowTime(1);
            gameOverMenu.SetActive(false);

            // set fast forward to false
            fastForward = false;
            paused = false;
            gameContinued = false;
        }

        // Resets the game back to its initial state
        public void RestartGame()
        {
            SceneManager.LoadScene("MainGame");

            StartGame();
        }

        // Pauses the game and brings up the pause menu
        public void PauseGame()
        {
            // Deactivate the game
            active = false;
            paused = true;
            fastForward = false;
            pieceManager.GameActive = false;

            // Disable the pause button
            fastForwardButton.interactable = false;

            // Bring up the pause menu
            pauseMenu.SetActive(true);

            // Pause the game
            Time.timeScale = 0;
        }

        // resumes the game after it was paused
        public void ResumeGame()
        {
            // Reset the game speed
            Time.timeScale = 1;

            // Re-activate the game and hide the pause menu
            active = true;
            paused = false;
            pieceManager.GameActive = true;
            fastForwardButton.interactable = true;
            fastForwardButton.image.sprite = fastForwardSprite;
            pauseMenu.SetActive(false);
        }

        // Toggles game pause on and off
        public void TogglePause()
        {
            if (paused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }

        // Activates the game when the player finishes the tutorial
        public void EndTutorial()
        {
            // Activate the game
            active = true;
            pieceManager.GameActive = true;
            pauseButton.interactable = true;
            fastForwardButton.interactable = true;
        }

        // called on game over and creates a scenario that allows
        // the player to continue playing
        public void Continue()
        {
            // Increment continue count and add pieces to the player's queue
            continueCount++;
            pieceManager.AddPieces(continuePiecesToAdd);

            // destroy nearby obstacles to let the player keep going
            int[] bounds = { pieceManager.HighestPieceY, pieceManager.HighestPieceY + continueBuffer + 1}; 
            obstacleManager.DeleteObstacles(bounds);

            // Reset the game speed
            Time.timeScale = 1;

            // Slow the game back down and re-activate it
            UpdateDifficultyLevel();
            active = true;
            pieceManager.GameActive = true;
            gameContinued = true;

            // Hide the game over menu and reactivate the game UI buttons
            gameOverMenu.SetActive(false);
            fastForwardButton.interactable = true;
            fastForwardButton.image.sprite = fastForwardSprite;
            pauseButton.interactable = true;

            // Show the "keep growing" popup
            Vector2 continuePopupPosition = mainCamera.transform.position;
            ContinueGamePopup.Create(new Vector3(continuePopupPosition.x, continuePopupPosition.y));
        }

        // Speeds the game up
        public void FastForward()
        {
            // invert fast forward
            fastForward = !fastForward;

            // call respective function
            if(fastForward)
            {
                Time.timeScale = fastForwardMultiplier;
                fastForwardButton.image.sprite = playSprite;
            }
            else
            {
                Time.timeScale = 1;
                fastForwardButton.image.sprite = fastForwardSprite;
            }
        }

        public void SaveProgress()
        {

        }

        public void LoadProgress()
        {

        }

        // Deactivates the game when one of the game over conditions are met
        public void GameOver()
        {
            // Deactivate the game
            active = false;
            fastForward = false;

            // halt the piece manager
            pieceManager.GameActive = false;

            // Pause the game speed
            Time.timeScale = 0;

            // show the game over screen
            gameOverMenu.SetActive(true);
            fastForwardButton.interactable = false;
            pauseButton.interactable = false;
        }

        // Start is called before the first frame update
        private void Start()
        {
            // Initialize the game systems
            StartGame();
        }

        // Update is called once per frame
        private void Update()
        {
            // Update the high score text with the current high score.
            UpdateScore();

            // Check if either game over condition has been met
            if (!pieceManager.DoneGrowing && pieceManager.PieceQueue.Count > 0)
            {
                // Have the camera follow the vine
                SyncCameraToVine();
                gameContinued = false;
            }
            else if (!gameContinued)
            {
                // If the vine reaches the end of the lattice or there are no more pieces to place,
                // activate Game Over
                GameOver();
            }

            // Quit the game if the escape key is pressed
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Application.Quit();
            }

            // update game when camera position changes
            CheckCameraPosition();

            // Increase the difficulty level if the player is not fast forwarding the game
            if(!fastForward)
            {
                UpdateDifficultyLevel();
            }

            // Store whether or not the game was active this frame
            pieceManager.PreviousFrameActive = active;
        }

        private void GenerateLevel(int levelNumber)
        {

        }

        // Makes the camera follow the vine
        private void SyncCameraToVine()
        {
            // Move the camera target if the vine has grown to a higher row
            if (pieceManager.VineY > highestRow)
            {
                highestRow = pieceManager.VineY;
                cameraMoveTarget.y += gridManager.boxSize;
            }
            
            // Move the camera toward the target
            mainCamera.transform.position = Vector3.SmoothDamp(
                    mainCamera.transform.position, cameraMoveTarget, 
                    ref cameraSpeed, pieceManager.VineGrowTime);

            // Update the camera bounds
            cameraTop = mainCamera.transform.position.y + mainCamera.orthographicSize;
            cameraBottom = mainCamera.transform.position.y - mainCamera.orthographicSize;
        }

        // Updates teh player's score
        private void UpdateScore()
        {
            // Update the player's score if the lattice is higher than the previous frame
            if (pieceManager.HighestPieceY > score)
            {
                score = pieceManager.HighestPieceY;
            }

            // Update the high score if the player's current score is larger
            if (score > highScore)
            {
                highScore = score;
                PlayerPrefs.SetInt("HighScore", highScore);
                guiHighScoreText.text = "New High Score!\n" + highScore;
            }

            // Display the current score and high score
            scoreText.text = "Score:" + score;
            hudHighScoreText.text = "Best:" + highScore;
        }

        // Speeds up the vine as the lattice gets taller
        private void UpdateDifficultyLevel()
        {
            // Calculate new difficulty level based on how many rows the lattice covers
            int remainder = pieceManager.HighestPieceY % rowsPerLevel;
            int newDifficultyLevel = ((pieceManager.HighestPieceY - remainder) / rowsPerLevel) + 1;
            newDifficultyLevel = newDifficultyLevel - (continueLevelReduction * continueCount);
            newDifficultyLevel = Mathf.Clamp(newDifficultyLevel, 1, newDifficultyLevel);

            // Update the difficulty level if it has changed
            if (newDifficultyLevel != difficultyLevel)
            {
                difficultyLevel = newDifficultyLevel;
                pieceManager.SetVineGrowTime(difficultyLevel);
            }
        }

        // Adds and removes grid rows based on the camera's position
        private void CheckCameraPosition()
        {
            // Add additional background parts if the camera is near the top of the background
            if (cameraTop > backgroundManager.Height - 1.0f)
            {
                backgroundManager.AddBackground();
            }

            // if camera top is near highest row...
            if(gridManager.HighestRowNearCameraView(cameraTop))
            {
                // ...add rows and fetch bounds
                int[] bounds = gridManager.AddRows();

                // ...use bounds from add rows to create new obstacles
                //Debug.Log("Adding Obstacles");
                // pass in the correct obstacle type into this function
                // to change obstacle type spawned
                obstacleManager.AddObstacles(bounds, levelStateManager.State);

                // ...use bounds from add rows to create new pickups
                //Debug.Log("Adding Pickups");
                pickupManager.AddPickups(bounds);
            
            }
            // if lowest row is out of camera sight...
            if(gridManager.LowestRowOutOfCameraView(cameraBottom))
            {
                // ...delete lowest row
                gridManager.DeleteLowestRow();

            }
        }
    }
}