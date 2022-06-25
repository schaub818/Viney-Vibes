using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Manages placement of obstacles in the game grid
    public class ObstacleManager : MonoBehaviour
    {
        // must be able to mark boxes on the grid as occupied
        public GridManager gridManager;

        // must know where the current highest piece is
        public PieceManager pieceManager;
       
        // defined list of ALL obstacles
        public List<Obstacle> obstacles;

        // defined list of more specific obstacles
        public List<Obstacle> buildingObstacles;
        public List<Obstacle> skyObstacles;
        public List<Obstacle> spaceObstacles;

        // number of obstacles to be added
        public int obstaclesToAdd = 3;

        // obstacles should have 'cushion' tiles between each other
        // ex: 1 means two beehives will always have 1 tile for the player
        // to go through
        public int cushion = 1;

        // offset spawn location from current highest piece
        // prevents obstacle from spawning RIGHT ON TOP of the player
        public int verticalOffset = 5;

        // represents the number of times an obstacle can fail to place
        // before the script gives up.
        public int stubbornness = 20;
        
        // Adds starting obstacles to the game grid
        public void Initialize()
        {
            // set bounds
            int[] bounds = { verticalOffset, gridManager.Height };

            // add obstacles to the board
            AddObstacles(obstaclesToAdd, bounds, LevelState.Building);
        }

        // Adds additional obstacles to the game grid
        public void AddObstacles(int[] bounds, LevelState state)
        {
            AddObstacles(obstaclesToAdd, bounds, state);
        }

        // adds obstacles to the board
        public void AddObstacles(int numberOfObstacles, int[] bounds, LevelState state)
        {
            int obstaclesOnBoard = 0;

            while(obstaclesOnBoard < numberOfObstacles)
            {
                // spawning obstacles
                SpawnRandomObstacle(bounds, state);

                // increment count
                obstaclesOnBoard++;
            }

        }

        // deletes obstacles
        public void DeleteObstacles(int[] bounds)
        {
            // fetch bounds
            int low = bounds[0];
            int high = bounds[1];

            // iterate through grid
            for(int i = 0; i < gridManager.Width; i++)
            {
                for(int j = low; j < high; j++)
                {
                    // fetch grid box
                    GridBox box = gridManager.Boxes[i][j];

                    // see if it has a child and obstacle
                    if(box.HasObstacle && box.transform.childCount > 0)
                    {                        
                        // reset this obstacle's space
                        Obstacle o = box.GetComponentInChildren<Obstacle>();

                        // set coordinates (were not stored)
                        o.SetCoords(i, j);

                        // reset space occupied
                        SetGridDoesNotHaveObstacle(o);

                        // destroy the obstacle
                        Destroy(o.gameObject);
                    }
                }
            }

            // Updates the the squares in the grid the current piece can be placed into
            gridManager.UpdateValidTargets();
        }

        // spawns a random obstacle on a random spot on the board
        private void SpawnRandomObstacle(int[] bounds, LevelState state)
        {
            // define local list of obstacles to be updated
            List<Obstacle> poolOfObstacles;

            // switch on type
            if(state == LevelState.Grass)
            {
                poolOfObstacles = buildingObstacles;
            }
            else if(state == LevelState.Building)
            {
                poolOfObstacles = buildingObstacles;
            }
            else if(state == LevelState.SkyTransition)
            {
                poolOfObstacles = skyObstacles;
            }
            else if(state == LevelState.Sky)
            {
                poolOfObstacles = skyObstacles;
            }
            else if(state == LevelState.SpaceTransition)
            {
                poolOfObstacles = spaceObstacles;
            }
            else if(state == LevelState.Space)
            {
                poolOfObstacles = spaceObstacles;
            }
            else
            {
                poolOfObstacles = obstacles;
            }

            // fetch random obstacle
            Obstacle obstacle = poolOfObstacles[Random.Range(0, poolOfObstacles.Count)];

            // modified X and Y domain
            int modifiedX = gridManager.Width;

            // analyze and filter possible spawn location on the board
            if(obstacle.Width > 1)
            {
                // shrink the possible X spawn locations
                modifiedX -= obstacle.Width;
            }

            // set vertical lower/upper bound
            int lowerBound = bounds[0];
            int upperBound = bounds[1];

            // fetch a new grid index
            int gridIndexX = Random.Range(0, modifiedX);
            int gridIndexY = Random.Range(lowerBound, upperBound);

            bool spaceNotAvailable = true;

            int counter = 0;
            
            // loops until an obstacle can be added
            while(spaceNotAvailable)
            {

                if(counter > stubbornness)
                {
                    Debug.Log("Re-rolled " + stubbornness + " times\nCould not spawn more obstacles");
                    return;
                }

                // check if that space is available
                if(GridSpaceAvailable(gridIndexX, gridIndexY, obstacle))
                {  
                    // spawn the obstacle
                    SpawnObstacle(gridIndexX, gridIndexY, obstacle);

                    // break out of loop
                    spaceNotAvailable = false;
                }
                else
                {
                    // spawn elsewhere
                    gridIndexX = Random.Range(0, modifiedX);
                    gridIndexY = Random.Range(lowerBound, upperBound);
                }

                counter++;
            }
            
        }

        // spawns a provided obstacle given coordinates on the board
        private void SpawnObstacle(int x, int y, Obstacle obstacle)
        {
            // update coordinates
            obstacle.SetCoords(x, y);

            // update the grid
            SetGridHasObstacle(obstacle);

            // insantiate the obstacle
            Obstacle o = Instantiate(
                obstacle, 
                GetCenter(obstacle), 
                Quaternion.identity
            );

            // nest
            o.transform.parent = gridManager.Boxes[x][y].transform;
        }

        // takes in grid coordinates to calculate the 
        // center between grid boxes in world position.
        private Vector3 GetCenter(int x, int y, int width, int height)
        {
            // calculate world position x1 and x2
            float x1 = gridManager.Boxes[x][y].transform.position.x;
            float x2 = gridManager.Boxes[x + width - 1][y].transform.position.x;

            // calculate world position y1 and y2
            float y1 = gridManager.Boxes[x][y].transform.position.y;
            float y2 = gridManager.Boxes[x][y + height - 1].transform.position.y;

            // calculate offsets
            float xOffset = (x2 - x1) / width;
            float yOffset = (y2 - y1) / height;

            // calculate center vector using offsets and initial location
            float updatedX = x1 + xOffset;
            float updatedY = y1 + yOffset;

            // return centered vector
            return new Vector3(updatedX, updatedY, 0);

        }

        // helper for GetCenter
        private Vector3 GetCenter(Obstacle o)
        {
            return GetCenter(o.X, o.Y, o.Width, o.Height);
        }

        // looks through grid given an initial coordinate
        // and dimensions to then update each grid box's properties
        private void SetGridHasObstacle(int x, int y, int width, int height)
        {  
            // calculate X2
            int x2 = x + (width - 1);

            // calculate Y2
            int y2 = y + (height - 1);

            // store first column
            int initialY = y;

            // iterate through that grid space, marking area as HasObstacle
            while(x < x2 + 1)
            {
                while(y < y2 + 1)
                {
                    // update this tile to has obstacle
                    gridManager.Boxes[x][y].HasObstacle = true;

                    // forward count
                    y++;
                }

                // reset column
                y = initialY;

                // forward count
                x++;
            }
            
        }

        // helper function that uses the properties of obstacle
        private void SetGridHasObstacle(Obstacle o)
        {
            SetGridHasObstacle(o.X, o.Y, o.Width, o.Height);
        }

        // reset the grid space; used when deleting an obstacle
        private void SetGridDoesNotHaveObstacle(int x, int y, int width, int height)
        {  
            // calculate X2
            int x2 = x + (width - 1);

            // calculate Y2
            int y2 = y + (height - 1);

            // store first column
            int initialY = y;

            // iterate through that grid space, marking area as HasObstacle
            while(x < x2 + 1)
            {
                while(y < y2 + 1)
                {
                    // update this tile to has obstacle
                    gridManager.Boxes[x][y].HasObstacle = false;

                    // forward count
                    y++;
                }

                // reset column
                y = initialY;

                // forward count
                x++;
            }
            
        }

        // Removes an obstacle from the game grid
        private void SetGridDoesNotHaveObstacle(Obstacle o)
        {
            SetGridDoesNotHaveObstacle(o.X, o.Y, o.Width, o.Height);
        }

        // takes in grid coordinates and dimensions to
        // calculate if the obstacle could fit in this area
        private bool GridSpaceAvailable(int x, int y, Obstacle obstacle)
        {
            // step back X and Y to create space for the player
            x -= cushion;
            y -= cushion;

            // check the space given to see if the obstacle can fit
            // step this one forward as well to give the player more room
            int x2 = x + obstacle.Width + cushion;
            int y2 = y + obstacle.Height + cushion;

            // check if out of bounds right away
            if(
                x2 >= gridManager.Width || x < 0 ||
                y2 >= gridManager.Height || y < 0
            )
            {
                return false;
            }
            
            int initialY = y;

            // iterate through matrix
            while(x < x2 + 1)
            {
                while(y < y2 + 1)
                {
                    
                    // if occupied or has obstacle, return false immediately
                    if(
                        gridManager.Boxes[x][y].Occupied || 
                        gridManager.Boxes[x][y].HasObstacle ||
                        gridManager.Boxes[x][y].HasPickup)
                    {
                        return false;
                    }                    
                    
                    // forward count
                    y++;
                }

                // reset column
                y = initialY;

                // forward count
                x++;
            }

            // if we made it through, there's space
            return true;
        }
    }
}