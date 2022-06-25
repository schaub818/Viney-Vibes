// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Manages generation, placement, and updating of the game grid
    public class GridManager : MonoBehaviour
    {
        // grab a reference to the background manager
        // so we can generate boxes UP TO the top of the background
        public BackgroundManager backgroundManager;

        // number of grid boxes ABOVE the camera view
        public int gridPaddingTop;

        // number of grid boxes BELOW the camera view
        public int gridPaddingBottom;

        // the size of each individual box in the grid
        public float boxSize;

        // the initial height of the grid
        public int initialHeight;

        // the initial width of the grid
        public int initialWidth;

        // number of rows that are spawned when camera reaches top row
        // public int rowsToSpawn;

        // the current height of the grid
        public int Height
        {
            get { return height; }
        }

        // the current width of the grid
        public int Width
        {
            get { return width; }
        }

        // The X coordinate of the current end point
        public int EndPointX
        {
            get { return endPointX; }
        }

        // The Y coordinate of the current end point
        public int EndPointY
        {
            get { return endPointY; }
        }

        // the box prefab used to create the Grid
        public GridBox boxPrefab;

        // the grid, represented through a nested list
        public List<List<GridBox>> Boxes
        {
            get { return boxes; }
        }

        // The default color of the grid boxes
        public Color boxColor;

        // The color for the current valid target boxes
        public Color targetBoxColor;

        // backing field of the grid's current height
        private int height;

        // backing field of the grid's current width
        private int width;

        // backing field of the grid itself
        private List<List<GridBox>> boxes;

        // Backing fields of the end point coordinates
        private int endPointX = -1;
        private int endPointY = -1;

        // variables that store highest and lowest rows, for speed
        private int lowestRow;
        private int highestRow;

        // public functions

        // create a grid given a width and a heigth
        public void Initialize()
        {
            // update width and height
            width = initialWidth;
            height = initialHeight;

            // create a linked list matrix
            boxes = new List<List<GridBox>>();

            // use width and height to create a grid
            // iterate through rows []
            for (int x = 0; x < width; x++)
            {
                // create current, empty row
                List<GridBox> currentRow = new List<GridBox>();

                // add the row to the matrix
                boxes.Add(currentRow);

                // iterate through columns [ [] ]
                for (int y = 0; y < height; y++)
                {
                    // create a new box prefab
                    GridBox box = SpawnBox(boxPrefab);

                    // nest the boxes inside of the grid manager for readability
                    box.transform.parent = this.transform;

                    // add that box to the grid
                    boxes[x].Add(box);

                    // get box position based on the box size & respective scaling
                    float posX = this.transform.position.x + (x * boxSize + (boxSize / 2));
                    float posY = this.transform.position.y + (y * boxSize + (boxSize / 2));

                    // fetch scales
                    float xScale = box.transform.localScale.x * boxSize;
                    float yScale = box.transform.localScale.y * boxSize;

                    // add item to the world, set coordinates, update scale
                    boxes[x][y].transform.position = new Vector3(posX, posY, 0.0f);
                    // COMMENTED CODE: scaling broke grid spacing when sprite pixels per unit
                    // setting was changed.
                    //boxes[x][y].transform.localScale = new Vector3(xScale, yScale, 0.0f);
                    boxes[x][y].name = "GridBox" + x + "_" + y;
                    boxes[x][y].GetComponent<GridBox>().SetCoords(x, y);
                }
            }

            // add starting placable row to the grid
            SetFirstRowPlacable();

            // update lowest/highest rows
            lowestRow = 0;
            highestRow = height - 1;

        }

        // helper function that passes in rowsToSpawn
        public int[] AddRows()
        {
            // float maxHeight = backgroundManager.Height;
            // float minHeight = boxes[0].Count * boxSize;

            // int rowsToAdd = (int) ((maxHeight - minHeight) / boxSize);

            // Debug.Log("Max: " + maxHeight);
            // Debug.Log("Min: "  + minHeight);
            // Debug.Log("Rows to add: "  + rowsToAdd);

            return AddRows(20);
        }

        // Adds the specified number of rows to the top of the grid
        public int[] AddRows(int numberOfRows)
        {
            // Stores the number of rows in the grid before adding additional rows
            int startingRowCount = boxes[0].Count;
            
            // store bounds of new area added
            int[] bounds = {startingRowCount, startingRowCount + numberOfRows - 1 };
            
            for (int x = 0; x < width; x++)
            {
                // create current, empty row
                List<GridBox> currentRow = new List<GridBox>();

                // add the row to the matrix
                boxes.Add(currentRow);

                // iterate through columns [ [] ]
                for (int y = startingRowCount; y < startingRowCount + numberOfRows; y++)
                {
                    // create a new box prefab
                    GridBox box = SpawnBox(boxPrefab);

                    // nest the boxes inside of the grid manager for readability
                    box.transform.parent = this.transform;

                    // add that box to the grid
                    boxes[x].Add(box);

                    // get box position based on the box size & respective scaling
                    float posX = this.transform.position.x + (x * boxSize + (boxSize / 2));
                    float posY = this.transform.position.y + (y * boxSize + (boxSize / 2));

                    // fetch scales
                    float xScale = box.transform.localScale.x * boxSize;
                    float yScale = box.transform.localScale.y * boxSize;

                    // add item to the world, set coordinates, update scale
                    boxes[x][y].transform.position = new Vector3(posX, posY, 0.0f);
                    boxes[x][y].name = "GridBox" + x + "_" + y;
                    boxes[x][y].GetComponent<GridBox>().SetCoords(x, y);
                }
            }

            // update the highest row
            highestRow += numberOfRows;

            // update height
            height += numberOfRows;

            // return bounds
            return bounds;
        }

        // deletes the lowest row
        public void DeleteLowestRow()
        {
            DeleteRow(lowestRow);
        }

        // delete a row given some index
        public void DeleteRow(int row)
        {
            for(int column = 0; column < width; column++)
            {
                // delete instanced gridbox
                Destroy(GameObject.Find("Managers/GridManager/GridBox" + column + "_" + row));

            }
            
            // update lowest row
            lowestRow++;
        }

        // deletes the row given, within some bounds
        public void DeleteRows(int lowerBound, int upperBound)
        {
            while(lowerBound < upperBound)
            {
                // delete that row
                DeleteRow(lowerBound);

                // increment count
                lowerBound++;
            }
        }

        // Sets the grid coordinates of the current end of the lattice
        public void SetEndPoint(int x, int y)
        {
            endPointX = x;
            endPointY = y;

            boxes[endPointX][endPointY].GetComponent<SpriteRenderer>().color = Color.white;
        }

        // Sets the specified row as occupied so that it's hidden
        public void SetRowOccupied(int row)
        {
            for (int i = 0; i < width; i++)
            {
                boxes[i][row].Occupied = true;
            }
        }

        // mark spaces on the grid as valid or invalid targets
        public void UpdateValidTargets()
        {
            if (boxes == null)
            {
                Debug.LogError("Initialize Grid before attempting to update targets.");

                return;
            }

            // Clear all current targets
            // iterate through rows
            // NOTE: not crashing in this for loop
            for (int x = 0; x < width; x++)
            {
                // iterate through columns, starting from the lowest row
                for (int y = lowestRow; y < height; y++)
                {
                    // fetch box to wipe
                    GridBox box = boxes[x][y];

                    // make this box an invalid target
                    box.ValidTarget = false;

                    // update its color
                    UpdateBoxColor(box);
                }
            }
            
            // NOTE: Crashing here?
            // using the current endpointX and endpointY, set valid targets
            if(endPointX > -1) 
            {
                // making above a valid target...
                // in bounds Y and above is not an obstacle?
                if (endPointY < boxes[endPointX].Count - 1 && !boxes[endPointX][endPointY + 1].HasObstacle)
                {
                    // make above a valid target
                    boxes[endPointX][endPointY + 1].ValidTarget = true;

                    // update the color
                    UpdateBoxColor(boxes[endPointX][endPointY + 1]);
                }
                
                // making left a valid target...
                // not located in the left-most column
                if (endPointX > 0)
                {
                    // left is not occupied or an obstacle?
                    if (!boxes[endPointX - 1][endPointY].Occupied && !boxes[endPointX - 1][endPointY].HasObstacle)
                    {
                        // make left a valid target
                        boxes[endPointX - 1][endPointY].ValidTarget = true;

                        // update the color
                        UpdateBoxColor(boxes[endPointX - 1][endPointY]);
                    }
                }

                // located anywhere but the rightmost row?
                if (endPointX < width - 1)
                {
                    // right is not occupied already and does not have an obstacle?
                    if (!boxes[endPointX + 1][endPointY].Occupied && !boxes[endPointX + 1][endPointY].HasObstacle)
                    {
                        // make right a valid target
                        boxes[endPointX + 1][endPointY].ValidTarget = true;
                        
                        // update color
                        UpdateBoxColor(boxes[endPointX + 1][endPointY]);
                    }
                }
            }
        }

        // sets the first row as a valid target and updates the color
        private void SetFirstRowPlacable()
        {
            // set first row as a valid target
            for(int w = 0; w < width; w++)
            {
                // make it a valid target
                boxes[w][0].ValidTarget = true;

                // update its color
                UpdateBoxColor(boxes[w][0]);
            }

        }

        // updates color of box in seperate function
        private void UpdateBoxColor(GridBox b)
        {
            // update color if the current box is a valid target or not
            if(b.ValidTarget)
            {
                // update valid tiles
                b.GetComponent<SpriteRenderer>().color = targetBoxColor;
                
            }
            else 
            {
                // update generic tiles
                b.GetComponent<SpriteRenderer>().color = boxColor;
            }
        }

        // updates gridbox color and then instantiates
        private GridBox SpawnBox(GridBox b)
        {
            // update color
            UpdateBoxColor(b);

            // instantiate
            return Instantiate(b);
        }

        public bool LowestRowOutOfCameraView(float cameraBottom)
        {
            if(boxes[0][lowestRow].transform.position.y < cameraBottom)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public bool HighestRowNearCameraView(float cameraTop)
        {
            if(boxes[0][highestRow].transform.position.y < cameraTop + 1.0f)
            {
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}