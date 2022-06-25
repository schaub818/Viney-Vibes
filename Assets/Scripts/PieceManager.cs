using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace VineyVibes
{
    // Manages pieces that have been placed, and pieces that are available to place.
    public class PieceManager : MonoBehaviour
    {
        
        // The number of pieces the player starts with.
        public int startingPieceCount;

        // Should we enable mouse or touch controls?
        public bool pcTesting;

        // The grid row number of the highest piece part. Determines the player's high score.
        public int HighestPieceY
        {
            get { return highestPieceY; }
        }

        // The row number that the vine is currently growing on
        public int VineY
        {
            get { return vineY; }
        }

        // The starting speed of the vine
        public float initialVineGrowTime;

        // How quickly the vine gains speed
        public float vineAcceleration;

        // Sets the exponential increase of the vine's speed
        public float vineAccelerationExponent;

        // How quickly the current piece flashes when placed incorrectly
        public float pieceFlashTime;

        // The amount of time it takes for a vine to finish growing on a piece part.
        public float VineGrowTime
        {
            get { return vineGrowTime; }
        }

        // Is the vine currently growing on this piece?
        public bool Growing
        {
            get { return growing; }
        }

        // Has the vine finished growing across this piece?
        public bool DoneGrowing
        {
            get { return doneGrowing; }
        }

        // Is the player currently dragging the piece?
        public bool DraggingPiece
        {
            get { return draggingPiece; }
        }

        // Is the game currently active?
        public bool GameActive
        {
            get { return gameActive; }
            set { gameActive = value; }
        }

        // Was the game active last frame?
        public bool PreviousFrameActive
        {
            get { return previousFrameActive; }
            set { previousFrameActive = value; }
        }

        // Stores prefabs for the standard and bonus pieces.
        public List<GameObject> piecePrefabs;

        // Stores the mini piece sprites for the current piece button
        public List<Sprite> miniPieceSprites;

        // Stores the extra mini piece sprites for the next piece image
        public List<Sprite> extraMiniPieceSprites;
        
        // Stores the prefab for the bonus piece
        // COMMENTED CODE: uncomment when implementing bonus pieces.
        //public GameObject bonusPiecePrefab;

        // A list of the pieces the player has available to place.
        public List<Tetromino> PieceQueue
        {
            get { return pieceQueue; }
        }

        // A list of the bonus single-square pieces the player has collected.
        // COMMENTED CODE: uncomment when implementing bonus pieces.
        //public List<GameObject> BonusPieceQueue
        //{
        //    get { return bonusPieceQueue; }
        //}

        // A list of the pieces the player has placed.
        public List<Piece> PlacedPieces
        {
            get { return placedPieces; }
        }

        // Stores the game's grid manager
        public GridManager gridManager;

        // The text that displays how many pieces the player has left to place.
        public TextMeshProUGUI pieceCountText;

        // game object to spawn pieces
        public GameObject pieceContainer;

        // Button for the current mini piece
        public Button currentPieceButton;

        // Image for the next mini piece
        public Image nextPieceImage;

        // The color to flash the current piece when placed incorrectly
        public Color pieceFlashColor;

        // The backing field for the int properties.
        private int highestPieceY;
        private int vineY;

        // The backing field the VineGrowTime property
        private float vineGrowTime;

        // The backing fields for the boolean properties.
        private bool growing;
        private bool doneGrowing;
        private bool gameActive;
        private bool previousFrameActive;
        private bool draggingPiece;
        private bool rotateKeyPressed;
        private bool movedThisTouch;
        private bool buttonClicked;

        // The backing fields for the List properties.
        private List<Tetromino> pieceQueue;
        // COMMENTED CODE: uncomment when implementing bonus pieces.
        //private List<GameObject> bonusPieceQueue;
        private List<Piece> placedPieces;

        // Initializes class data and adds starting pieces to the piece queue.
        public void Initialize()
        {
            // Initialize the backing fields for the PieceManager's properties.
            pieceQueue = new List<Tetromino>();
            // COMMENTED CODE: uncomment when implementing bonus pieces.
            //bonusPieceQueue = new List<GameObject>();
            placedPieces = new List<Piece>();

            highestPieceY = 0;

            growing = false;
            doneGrowing = false;
            gameActive = false;
            draggingPiece = false;
            rotateKeyPressed = false;
            movedThisTouch = false;
            buttonClicked = false;

            // Add random starting pieces to the player's piece queue.
            if (startingPieceCount > 0)
            {
                for (int i = 0; i < startingPieceCount; i++)
                {
                    int randomTypeIndex = Random.Range(0, 5);
                    int randomOrientationIndex = Random.Range(0, 4);

                    // insantiate a new piece
                    GameObject piece = Instantiate<GameObject>(piecePrefabs[randomTypeIndex]);

                    // get tetromino component and init with type/rotation
                    piece.GetComponent<Tetromino>().Initialize((TetronimoType)randomTypeIndex,
                            (PieceOrientation)randomOrientationIndex);

                    // nest the piece in the "Pieces" parent
                    piece.transform.parent = transform;

                    // place on screen
                    piece.transform.position = new Vector3(1000.0f, 0.0f, 0.0f);

                    // declare name
                    piece.name = "Piece" + i;

                    // add it to the queue
                    pieceQueue.Add(piece.GetComponent<Tetromino>());
                }

                // Set the current piece and next piece in the HUD
                SetNextPiece();
            }

            // Display how many pieces are remaining in the piece queue.
            pieceCountText.text = "Pieces:" + pieceQueue.Count;
        }

        // Rotates the current piece 90 degrees clockwise.
        public void RotateCurrentPiece()
        {
            // Rotate the current piece
            pieceQueue[0].Rotate();

            // Rotate the image in the current piece button to match the new orientation of the
            // current piece
            currentPieceButton.transform.rotation = pieceQueue[0].transform.rotation;
        }

        // Handles when the player starts dragging the current piece
        public void StartDraggingPiece()
        {
            // If the game is active, mark that we are dragging the current piece
            if (gameActive)
            {
                draggingPiece = true;
            }
        }

        // Prevents the current piece from being rotated if a UI button has been clicked
        public void ButtonClick()
        {
            // Mark that a button has been clicked this frame
            buttonClicked = true;
        }

        // Adds the specified number of pieces to the piece queue.
        public void AddPieces(int numberOfPieces)
        {
            // Add random pieces to the player's piece queue.
            if (numberOfPieces > 0)
            {
                for (int i = 0; i < numberOfPieces; i++)
                {
                    // Generate random indexes to pull from the piece type and orientation lists
                    int randomTypeIndex = Random.Range(0, 5);
                    int randomOrientationIndex = Random.Range(0, 4);

                    // insantiate a new piece
                    GameObject piece = Instantiate<GameObject>(piecePrefabs[randomTypeIndex]);

                    // get tetromino component and init with type/rotation
                    piece.GetComponent<Tetromino>().Initialize((TetronimoType)randomTypeIndex,
                            (PieceOrientation)randomOrientationIndex);

                    // nest the piece in the "Pieces" parent
                    piece.transform.parent = transform;

                    // place on screen
                    piece.transform.position = new Vector3(1000.0f, 0.0f, 0.0f);

                    // declare name
                    piece.name = "Piece" + i;

                    // add it to the queue
                    pieceQueue.Add(piece.GetComponent<Tetromino>());
                }
            }

            // Display how many pieces are remaining in the piece queue.
            pieceCountText.text = "Pieces:" + pieceQueue.Count;

            // Set the current piece and next piece in the HUD
            SetNextPiece();
        }

        // Sets the time it takes the vine to grow across a piece part based on the current game
        // difficulty
        public void SetVineGrowTime(int difficultyLevel)
        {
            // Calculate how much time should be deducted from the starting vine growth time
            // based on the current game difficulty level
            float vineTimeReduction = vineAcceleration * Mathf.Pow(difficultyLevel - 1, vineAccelerationExponent);

            vineGrowTime = initialVineGrowTime - vineTimeReduction;

            // Prevent the vine grow time from becomming too short
            vineGrowTime = Mathf.Clamp(vineGrowTime, 0.3f, vineGrowTime);

            // Set the current vine grow time on all the pieces in the piece queue
            foreach (Piece piece in pieceQueue)
            {
                piece.GrowTime = vineGrowTime;
            }

            // Set the current vine grow time on all placed pieces
            foreach (Piece piece in placedPieces)
            {
                piece.GrowTime = vineGrowTime;
            }
        }

        // Processes piece queue changes if the current piece has been placed, and grows the vine.
        private void Update()
        {
            // Process changes if the game is currently active.
            if (gameActive)
            {
                // Handle piece dragging and placement.
                HandlePieceDragging();
                HandleRotateKey();

                // Grow the vine if it is currently growing.
                if (growing)
                {
                    Grow();
                }
            }
        }

        // Handles mouse input and drags the current piece in the piece queue.
        private void HandlePieceDragging()
        {
            // Only handle mouse input if there is a piece available to drag.
            if (pieceQueue.Count > 0)
            {
                // If the game is being tested on PC, handle mouse inputs
                if (pcTesting)
                {
                    // Store the world position of the mouse cursor.
                    Vector2 mousePosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

                    // If the left mouse button is held down, try to drag the current piece.
                    if (Input.GetMouseButtonDown(0))
                    {
                        // Resets whether the piece has been moved in the current click input
                        movedThisTouch = false;

                        // If the mouse is over the current piece, mark that we are dragging it.
                        if (pieceQueue[0].gameObject.GetComponent<Collider2D>() ==
                                Physics2D.OverlapPoint(mousePosition, 1 << 8))
                        {
                            // Mark that we are dragging the current piece and set mousePieceOffset
                            // if we just started dragging it.
                            draggingPiece = true;
                        }
                    }

                    // If the the left mouse button was released and we are dragging the current
                    // piece, test if the piece was dropped on a valid target.
                    if (Input.GetMouseButtonUp(0))
                    {
                        if (draggingPiece)
                        {
                            // Stores whether the current piece has been connected to the previous
                            // piece.
                            bool connected = false;

                            // Stores the index of the part of the current piece that was connected 
                            // to the previous piece.
                            int connectedIndex = -1;
                            int targetX = -1;
                            int targetY = -1;

                            // Mark that we are no longer dragging the current piece.
                            draggingPiece = false;

                            // Find the part of the current piece that is connected.
                            for (int i = 0; i < pieceQueue[0].parts.Count; i++)
                            {
                                if (pieceQueue[0].parts[i].Connectable &&
                                        pieceQueue[0].parts[i].TargetBoxes.Count > 0)
                                {
                                    connected = true;
                                    connectedIndex = i;

                                    break;
                                }
                            }

                            // If we are connected, figure out which target box the piece was
                            // dropped on.
                            if (connected)
                            {
                                // Stores the distances that we will be testing.
                                float testDistance;
                                float shortestDistance = 1000.0f;

                                // Stores the grid coordinates of the closest target box.
                                int targetBoxIndex = -1;

                                // Loop through all the target boxes that our piece is over, and
                                // figure out which box the piece is overlapping the most.
                                for (int i = 0; 
                                        i < pieceQueue[0].parts[connectedIndex].TargetBoxes.Count;
                                        i++)
                                {
                                    // Calculate the distance from the center of the connected part
                                    // to the center of the box we're testing
                                    testDistance = Vector3.Distance(
                                            pieceQueue[0].parts[connectedIndex].transform.position,
                                            pieceQueue[0].parts[connectedIndex].TargetBoxes[i]
                                            .transform.position);

                                    // Update the shortest distance and store the test box's index
                                    // if the calculated distance is shorter than the current
                                    // shortest distance
                                    if (testDistance < shortestDistance)
                                    {
                                        shortestDistance = testDistance;
                                        targetBoxIndex = i;
                                    }
                                }

                                // Connect the piece to the most overlapped target box.
                                pieceQueue[0].parts[connectedIndex].Connected = true;
                                targetX = pieceQueue[0].parts[connectedIndex]
                                        .TargetBoxes[targetBoxIndex].X;
                                targetY = pieceQueue[0].parts[connectedIndex]
                                        .TargetBoxes[targetBoxIndex].Y;

                                // Place the piece using the grid coordinates of the closest target
                                // box.
                                pieceQueue[0].Place(targetX, targetY,
                                        gridManager.EndPointX, gridManager.EndPointY);

                                // Check each piece part to make sure none are outside of the grid,
                                // or are on a box that has an obstacle or part of another piece.
                                foreach (PiecePart part in pieceQueue[0].parts)
                                {
                                    // Disconnect the piece and flash it if part of it is outside
                                    // the grid
                                    if (part.X < 0 || part.X > gridManager.Width - 1 || part.Y < 0)
                                    {
                                        pieceQueue[0].Remove();
                                        connected = false;
                                        StartCoroutine(FlashPieceColor());

                                        break;
                                    }
                                    // Disconnect the piece and flash it if part of it is on a box
                                    // that is occupied
                                    else if (gridManager.Boxes[part.X][part.Y].Occupied ||
                                            gridManager.Boxes[part.X][part.Y].HasObstacle)
                                    {
                                        pieceQueue[0].Remove();
                                        connected = false;
                                        StartCoroutine(FlashPieceColor());

                                        break;
                                    }
                                }

                                // If all the piece parts are on valid grid boxes, position the
                                // piece on the grid where it was placed and update the grid's
                                // valid target boxes.
                                if (connected)
                                {
                                    // Calculate the new world position of the connected piece
                                    float newX = gridManager.Boxes[targetX][targetY]
                                            .transform.position.x +
                                            (pieceQueue[0].transform.position.x -
                                            pieceQueue[0].parts[connectedIndex]
                                            .transform.position.x);

                                    float newY = gridManager.Boxes[targetX][targetY]
                                            .transform.position.y +
                                            (pieceQueue[0].transform.position.y -
                                            pieceQueue[0].parts[connectedIndex]
                                            .transform.position.y);

                                    // Move the connected piece to its new position
                                    pieceQueue[0].transform.position = new Vector2(newX, newY);

                                    // Mark all the grid boxes that the connected piece occupies
                                    // as being occupied
                                    foreach (PiecePart part in pieceQueue[0].parts)
                                    {
                                        gridManager.Boxes[part.X][part.Y].Occupied = true;
                                    }

                                    // Set the end point of the connected piece based on which end
                                    // is connected
                                    if (connectedIndex == 0)
                                    {
                                        gridManager.SetEndPoint(pieceQueue[0].parts[3].X,
                                                pieceQueue[0].parts[3].Y);
                                    }
                                    else
                                    {
                                        gridManager.SetEndPoint(pieceQueue[0].parts[0].X,
                                                pieceQueue[0].parts[0].Y);
                                    }

                                    // Update the grid's valid targets
                                    gridManager.UpdateValidTargets();

                                    // If a piece was previously placed, set the vine type of its
                                    // end part
                                    if (placedPieces.Count > 0)
                                    {
                                        if (!placedPieces[placedPieces.Count - 1].EndPart.Growing)
                                        {
                                            placedPieces[placedPieces.Count - 1]
                                                    .SetEndPartVineType(targetX);
                                        }
                                    }

                                    // Finalize piece placement and set the new current and next
                                    // pieces in the HUD
                                    PlacePiece();
                                    SetNextPiece();

                                    // If the vine has not started growing yet, set it to start growing.
                                    if (!growing)
                                    {
                                        growing = true;
                                    }
                                    
                                    // Store the row number of the previous highest part of the lattice
                                    int oldHighestPieceY = highestPieceY;

                                    // Store the row number of the current highest part of the lattice.
                                    highestPieceY = placedPieces[placedPieces.Count - 1].HighestPartY;

                                    // Set the rows between the previous highest row number and the
                                    // new highest row number as occupied so they disappear and
                                    // can't have pieces placed in them
                                    for (int i = oldHighestPieceY; i < highestPieceY; i++)
                                    {
                                        gridManager.SetRowOccupied(i);
                                    }
                                }
                            }
                        }

                        // If the piece hasn't been dragged and a UI button hasn't been clicked
                        // in the current mouse click, and if the game was active in the previous
                        // frame, rotate the current piece
                        if (!movedThisTouch && !buttonClicked && previousFrameActive)
                        {
                            RotateCurrentPiece();
                        }

                        // Reset whether a UI button has been click in the current mouse click
                        buttonClicked = false;
                    }

                    // If we are dragging the current piece, have it follow the mouse cursor.
                    if (draggingPiece)
                    {
                        movedThisTouch = true;

                        pieceQueue[0].transform.position =
                                new Vector3(mousePosition.x, mousePosition.y, 0.0f);

                        if (currentPieceButton.image.enabled)
                        {
                            currentPieceButton.image.enabled = false;
                        }
                    }
                }
                else if (Input.touchCount > 0)
                {
                    Touch touch = Input.GetTouch(0);
                    Vector2 touchPosition = Camera.main.ScreenToWorldPoint(touch.position);

                    if (touch.phase == TouchPhase.Began)
                    {
                        movedThisTouch = false;
                        
                        if (PieceQueue[0].gameObject.GetComponent<Collider2D>() ==
                                Physics2D.OverlapPoint(touchPosition, 1 << 8))
                        {
                            draggingPiece = true;
                        }
                    }
                    else if (touch.phase == TouchPhase.Ended)
                    {
                        if (draggingPiece)
                        {
                            // Stores whether the current piece has been connected to the previous 
                            // piece.
                            bool connected = false;

                            // Stores the index of the part of the current piece that was connected
                            // to the previous piece.
                            int connectedIndex = -1;
                            int targetX = -1;
                            int targetY = -1;

                            // Find the part of the current piece that is connected.
                            for (int i = 0; i < pieceQueue[0].parts.Count; i++)
                            {
                                if (pieceQueue[0].parts[i].Connectable &&
                                        pieceQueue[0].parts[i].TargetBoxes.Count > 0)
                                {
                                    connected = true;
                                    connectedIndex = i;

                                    break;
                                }
                            }

                            // If we are connected, figure out which target box the piece was 
                            // dropped on.
                            if (connected)
                            {
                                // Stores the distances that we will be testing.
                                float testDistance;
                                float shortestDistance = 1000.0f;

                                // Stores the grid coordinates of the closest target box.
                                int targetBoxIndex = -1;

                                // Loop through all the target boxes that our piece is over, and
                                // figure out which box the piece is overlapping the most.
                                for (int i = 0; 
                                        i < pieceQueue[0].parts[connectedIndex].TargetBoxes.Count;
                                        i++)
                                {
                                    testDistance = Vector3.Distance(
                                            pieceQueue[0].parts[connectedIndex].transform.position,
                                            pieceQueue[0].parts[connectedIndex].TargetBoxes[i]
                                            .transform.position);


                                    if (testDistance < shortestDistance)
                                    {
                                        shortestDistance = testDistance;
                                        targetBoxIndex = i;
                                    }
                                }

                                // Connect the piece to the most overlapped target box.
                                pieceQueue[0].parts[connectedIndex].Connected = true;
                                targetX = pieceQueue[0].parts[connectedIndex]
                                        .TargetBoxes[targetBoxIndex].X;
                                targetY = pieceQueue[0].parts[connectedIndex]
                                        .TargetBoxes[targetBoxIndex].Y;

                                // Place the piece using the grid coordinates of the closest target
                                // box.
                                pieceQueue[0].Place(targetX, targetY,
                                        gridManager.EndPointX, gridManager.EndPointY);

                                // Check each piece part to make sure none are outside of the grid,
                                // or are on a box that has an obstacle or part of another piece.
                                foreach (PiecePart part in pieceQueue[0].parts)
                                {
                                    if (part.X < 0 || part.X > gridManager.Width - 1 || part.Y < 0)
                                    {
                                        pieceQueue[0].Remove();
                                        connected = false;
                                        StartCoroutine(FlashPieceColor());

                                        break;
                                    }
                                    else if (gridManager.Boxes[part.X][part.Y].Occupied ||
                                            gridManager.Boxes[part.X][part.Y].HasObstacle)
                                    {
                                        pieceQueue[0].Remove();
                                        connected = false;
                                        StartCoroutine(FlashPieceColor());

                                        break;
                                    }
                                }

                                // If all the piece parts are on valid grid boxes, position the
                                // piece on the grid where it was placed and update the grid's 
                                // valid target boxes.
                                if (connected)
                                {
                                    float newX = 
                                            gridManager.Boxes[targetX][targetY]
                                            .transform.position.x
                                            + (pieceQueue[0].transform.position.x -
                                            pieceQueue[0].parts[connectedIndex]
                                            .transform.position.x);
                                    float newY = 
                                            gridManager.Boxes[targetX][targetY]
                                            .transform.position.y +
                                            (pieceQueue[0].transform.position.y -
                                            pieceQueue[0].parts[connectedIndex]
                                            .transform.position.y);

                                    pieceQueue[0].transform.position = new Vector2(newX, newY);

                                    foreach (PiecePart part in pieceQueue[0].parts)
                                    {
                                        gridManager.Boxes[part.X][part.Y].Occupied = true;
                                    }

                                    if (connectedIndex == 0)
                                    {
                                        gridManager.SetEndPoint(pieceQueue[0].parts[3].X,
                                                pieceQueue[0].parts[3].Y);
                                    }
                                    else
                                    {
                                        gridManager.SetEndPoint(pieceQueue[0].parts[0].X,
                                                pieceQueue[0].parts[0].Y);
                                    }

                                    gridManager.UpdateValidTargets();

                                    if (placedPieces.Count > 0)
                                    {
                                        if (!placedPieces[placedPieces.Count - 1].EndPart.Growing)
                                        {
                                            placedPieces[placedPieces.Count - 1]
                                                    .SetEndPartVineType(targetX);
                                        }
                                    }

                                    PlacePiece();
                                    SetNextPiece();

                                    // Mark that we are no longer dragging the current piece.
                                    draggingPiece = false;

                                    currentPieceButton.image.enabled = true;

                                    // If the vine has not started growing yet, set it to start 
                                    // growing.
                                    if (!growing)
                                    {
                                        growing = true;
                                    }

                                    // Store the row number of the highest part of the lattice.
                                    highestPieceY = placedPieces[placedPieces.Count - 1]
                                            .HighestPartY;
                                }
                            }
                        }
                        
                        if (!movedThisTouch && !buttonClicked && previousFrameActive)
                        {
                            RotateCurrentPiece();
                        }

                        buttonClicked = false;
                    }
                    else if (touch.phase == TouchPhase.Moved)
                    {
                        movedThisTouch = true;

                        if (draggingPiece)
                        {
                            pieceQueue[0].transform.position =
                                    new Vector3(touchPosition.x, touchPosition.y, 0.0f);

                            if (currentPieceButton.image.enabled)
                            {
                                currentPieceButton.image.enabled = false;
                            }
                        }
                    }
                }
            }
        }

        private void HandleRotateKey()
        {
            if (Input.GetKeyDown(KeyCode.R) && !rotateKeyPressed)
            {
                rotateKeyPressed = true;

                RotateCurrentPiece();
            }

            if (Input.GetKeyUp(KeyCode.R))
            {
                rotateKeyPressed = false;
            }
        }

        // Updates the piece queues, highest piece score, and piece count text.
        private void PlacePiece()
        {
            pieceQueue[0].gameObject.transform.parent = pieceContainer.transform;

            // Move the current piece from the available queue to the placed queue.
            placedPieces.Add(pieceQueue[0]);            

            // Track score change
            int deltaScore = placedPieces[placedPieces.Count - 1].HighestPartY - highestPieceY;

            // Show point popup
            CreatePointPopup(deltaScore);

            // Look for and collect any pickups
            CheckPickups(pieceQueue[0]);

            // Remove placed piece from piece queue
            pieceQueue.RemoveAt(0);

            // Display how many pieces are remaining in the piece queue.
            pieceCountText.text = "Pieces:" + pieceQueue.Count;
        }

        private void CheckPickups(Piece piece)
        {
            // iterate through parts
            foreach(PiecePart part in piece.parts)
            {
                // if a tile has a pickup...
                if(gridManager.Boxes[part.X][part.Y].HasPickup)
                {
                    // fetch gridbox where pickup is located
                    GridBox gridBox = gridManager.Boxes[part.X][part.Y];

                    // grab pickup from child of grid box
                    Pickup pickup = gridBox.transform.GetChild(0).GetComponent<Pickup>();
                    
                    // ALPHA CODE: set the pickup's piece manager to prep for collect call
                    // there may be a better way to do this but this is what i got to work
                    pickup.pieceManager = this;
                    
                    // collect the pickup and update game
                    pickup.Collect();
                    
                }

            }
            
        }

        // creates a point popup when a piece is placed
        private void CreatePointPopup(int deltaScore)
        {
            // Fetch last mouse position
            Vector2 spawnPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            // Create new popup with that position
            PointsPopup.Create(new Vector3(spawnPosition.x, spawnPosition.y), deltaScore);

        }

        // Adjusts the properties of the current and next piece in the piece queue so they are
        // in the correct locations and the current piece can be dragged.
        private void SetNextPiece()
        {
            // Set the properties of the current piece if there are pieces remaining in the queue.
            if (pieceQueue.Count > 0)
            {
                currentPieceButton.image.sprite = miniPieceSprites[(int)pieceQueue[0].Type];
                currentPieceButton.image.enabled = true;
                currentPieceButton.transform.rotation = pieceQueue[0].transform.rotation;
            }
            else
            {
                currentPieceButton.image.enabled = false;
            }

            // Set the properties of the next piece if there is more than one piece left in the
            // piece queue.
            if (pieceQueue.Count > 1)
            {
                nextPieceImage.sprite = extraMiniPieceSprites[(int)pieceQueue[1].Type];
                nextPieceImage.enabled = true;
                nextPieceImage.transform.rotation = pieceQueue[1].transform.rotation;
            }
            else
            {
                nextPieceImage.enabled = false;
            }
        }

        // Grows the vine through all placed pieces, from bottom to top.
        private void Grow()
        {
            // Mark that the vine is done growing if the most recently placed piece is done growing.
            if (placedPieces[placedPieces.Count - 1].DoneGrowing)
            {
                growing = false;
                doneGrowing = true;
            }
            // If the most recently placed piece is not done growing, grow the vine.
            else
            {
                growing = true;
                doneGrowing = false;

                // Stores the index of the piece placed directly before the piece being checked in
                // the loop.
                int prevIndex;
                int growingIndex = 0;

                // Find the most recently placed piece that is done growing, and tell the next
                // placed piece after it to start growing.
                for (int i = placedPieces.Count - 1; i > -1; i--)
                {
                    // If we are currently considering a piece other than the first one that was
                    // placed, check if the piece placed directly before it has finished growing.
                    if (i > 0)
                    {
                        prevIndex = i - 1;

                        // If the piece placed directly before the piece we are considering has
                        // finished growing, start growing the piece being considered.
                        if (placedPieces[prevIndex].DoneGrowing)
                        {
                            placedPieces[i].Growing = true;
                            growingIndex = i;

                            break;
                        }
                    }
                    // If we are currently considering the first piece that was placed, check if
                    // it is growing.
                    else
                    {
                        if (!placedPieces[0].Growing && !placedPieces[0].DoneGrowing)
                        {
                            placedPieces[0].Growing = true;
                            growingIndex = 0;

                            break;
                        }
                    }
                }

                vineY = placedPieces[growingIndex].VineY;
            }
        }

        private IEnumerator FlashPieceColor()
        {
            float flashTimer = 0;
            float amplitudeR = Color.white.r - pieceFlashColor.r;
            float amplitudeG = Color.white.g - pieceFlashColor.g;
            float amplitudeB = Color.white.b - pieceFlashColor.b;
            float deltaR = 0;
            float deltaG = 0;
            float deltaB = 0;

            while (flashTimer < pieceFlashTime)
            {
                float theta = Time.timeSinceLevelLoad / pieceFlashTime;
                flashTimer += Time.deltaTime;

                deltaR = amplitudeR * Mathf.Sin(flashTimer / (pieceFlashTime * 0.25f));
                deltaG = amplitudeG * Mathf.Sin(flashTimer / (pieceFlashTime * 0.25f));
                deltaB = amplitudeB * Mathf.Sin(flashTimer / (pieceFlashTime * 0.25f));

                if (pieceQueue.Count > 0)
                {
                    pieceQueue[0].SpriteColor = new Color(
                            Color.white.r - deltaR,
                            Color.white.g - deltaG,
                            Color.white.b - deltaB);
                }

                yield return null;
            }

            pieceQueue[0].SpriteColor = Color.white;
        }
    }
}