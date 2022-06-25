// --------------------------------------------
// Written by Dave Schaub
// with contributions from Andrés Fernandez
// --------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes
{
    // Generates the play field background
    public class BackgroundManager : MonoBehaviour
    {
        public LevelStateManager lsm;

        // "chunks" are groups of background panels
        // one chunk would be one sprite
        // two chunks would be two sprites
        // this number is used to generate more of that level each time
        // if the building lasts 6 chunks, it would generate that many
        public int backgroundChunkSize;

        // Sets the maximum distance from the center of the screen each type of decoration can be
        // placed
        public float windowSafeArea;
        public float cloudSafeArea;
        public float planetSafeArea;
        // Sets how likely a background sprite is to have a decoration on it
        public float decorationDensity;

        // The total height of all the current background sprites
        public float Height
        {
            get { return height; }
        }

        // The width of the background
        public float Width
        {
            get { return width; }
        }

        // Stores prefabs of the transition sprites
        public GameObject groundSprite;
        public GameObject skyTransitionSprite;
        public GameObject spaceTransitionSprite;

        // Stores prefabs of the background and decoration sprites
        public List<GameObject> brickWallGameObjects;
        public List<GameObject> windowGameObjects;
        public List<GameObject> skyGameObjects;
        public List<GameObject> cloudGameObjects;
        public List<GameObject> spaceGameObjects;
        public List<GameObject> planetGameObjects;

        // The total number of space sprites currently in the background
        private int spaceSpriteCount;
        
        // How far up to set the next sprite in the background
        private float yOffset;
        // The backing fields for the Height and Width properties
        private float height;
        private float width;

        // Sets all the wall and sky sprites and their associated decorations
        public void Initialize()
        {
            // Initialize the space sprite count
            spaceSpriteCount = 0;

            // Make sure decorationDensity is between 0.0 and 1.0
            decorationDensity = Mathf.Clamp(decorationDensity, 0.0f, 1.0f);

            AddBackground();
        }

        public void AddBackground() 
        {
            switch(lsm.State)
            {
                case LevelState.Grass: 
                    // add grass
                    AddGrass();
                    // building is a part of level one
                    lsm.Next();
                    // add building
                    AddBuilding();
                    // exit
                    break;
                case LevelState.Building:
                    AddBuilding();
                    break;
                case LevelState.SkyTransition:
                    AddSkyTransition();
                    break;
                case LevelState.Sky:
                    AddSky();
                    break;
                case LevelState.SpaceTransition:
                    AddSpaceTransition();
                    break;
                case LevelState.Space:
                    AddSpace();
                    break;
                default: break;
            }
            
            // always try move the state forward
            lsm.Next();
        }

        private void AddGrass() 
        {
            // Create and place the ground sprite
            GameObject grassyArea = Instantiate<GameObject>(groundSprite);
            grassyArea.transform.position = transform.position;
            grassyArea.transform.parent = this.transform;

            // Update the height of the background
            height = grassyArea.GetComponent<SpriteRenderer>().bounds.size.y;
            width = grassyArea.GetComponent<SpriteRenderer>().bounds.size.x;
        }

        private void AddBuilding() 
        {
            int randomIndex;
            float decorationChance;
            
            // Used to store the random position of the decoration
            float decorationXPosition;
            

            // Create and place the wall sprites
            for (int i = 0; i < backgroundChunkSize; i++)
            {
                // Select a random sprite from the wall sprites collection
                randomIndex = Random.Range(0, brickWallGameObjects.Count);

                // Create a wall sprite object from the randomly selected sprite
                GameObject brickWallObject = 
                        Instantiate<GameObject>(brickWallGameObjects[randomIndex]);

                // Nest in the background manager
                brickWallObject.transform.parent = this.transform;

                // Update the placement offset
                yOffset += brickWallObject.GetComponent<SpriteRenderer>().bounds.extents.y;

                // Place the wall sprite object and give it a name
                brickWallObject.transform.position = 
                        new Vector3(transform.position.x, transform.position.y + yOffset, 0.0f);
                brickWallObject.name = "BrickWall" + i;

                // Update the placement offset and background height
                yOffset += brickWallObject.GetComponent<SpriteRenderer>().bounds.extents.y;
                height += brickWallObject.GetComponent<SpriteRenderer>().bounds.size.y;

                // Generate a random number to determine if this wall sprite will have a window on 
                // it
                decorationChance = Random.Range(0.0f, 1.0f);

                // If decorationChance says we should create a window, do it
                if (decorationChance <= decorationDensity)
                {
                    // Select a random sprite from the window sprite list
                    randomIndex = Random.Range(0, windowGameObjects.Count);
                    // Select a random X position for the window within the safe area
                    decorationXPosition = Random.Range(0 - windowSafeArea, windowSafeArea);
                    
                    // Create a window sprite object from the randomly selected sprite
                    GameObject decorationObject = Instantiate<GameObject>(windowGameObjects[randomIndex]);
                    decorationObject.transform.parent = this.transform;

                    // Place the window object and give it a name
                    decorationObject.transform.position = new Vector3(
                            brickWallObject.GetComponent<SpriteRenderer>().bounds.center.x + 
                            decorationXPosition,
                            brickWallObject.GetComponent<SpriteRenderer>().bounds.center.y,
                            0.0f);
                    decorationObject.name = "Window" + i;
                }
            }
        }

        private void AddSkyTransition() 
        {
            // Create the sky transition object
            GameObject skyTransitionObject = Instantiate<GameObject>(skyTransitionSprite);

            // Nest in background manager
            skyTransitionObject.transform.parent = this.transform;

            // Update the placement offset
            yOffset += skyTransitionObject.GetComponent<SpriteRenderer>().bounds.extents.y;

            // Place the sky transition object and give it a name
            skyTransitionObject.transform.position =
                    new Vector3(transform.position.x, transform.position.y + yOffset, 0.0f);
            skyTransitionObject.name = "SkyTransition";

            // Update the placement offset and background height
            yOffset += skyTransitionObject.GetComponent<SpriteRenderer>().bounds.extents.y;
            height += skyTransitionObject.GetComponent<SpriteRenderer>().bounds.size.y;
        }

        private void AddSky() 
        {
            int randomIndex;
            float decorationChance;
            
            // Used to store the random position of the decoration
            float decorationXPosition;
            float decorationYPosition;

            // Create and place the sky sprites
            for (int i = 0; i < backgroundChunkSize; i++)
            {
                // Select a random sprite from the sky sprites collection
                randomIndex = Random.Range(0, skyGameObjects.Count);

                // Create sky sprite object from the randomly selected sprite
                GameObject skyObject = Instantiate<GameObject>(skyGameObjects[randomIndex]);

                // Update the placement offset
                yOffset += skyObject.GetComponent<SpriteRenderer>().bounds.extents.y;

                // Place the sky sprite object and give it a name
                skyObject.transform.position =
                        new Vector3(transform.position.x, transform.position.y + yOffset, 0.0f);
                skyObject.name = "Sky" + i;

                // Update the placement offset and background height
                yOffset += skyObject.GetComponent<SpriteRenderer>().bounds.extents.y;
                height += skyObject.GetComponent<SpriteRenderer>().bounds.size.y;

                // Generate a random number to determine if this sky sprite will have a cloud on it
                decorationChance = Random.Range(0.0f, 1.0f);

                // If decorationChance says we should create a cloud, do it
                if (decorationChance <= decorationDensity)
                {
                    // Select a random sprite from the cloud sprite list
                    randomIndex = Random.Range(0, cloudGameObjects.Count);
                    // Select a random X position for the cloud within the safe area
                    decorationXPosition = Random.Range(0 - cloudSafeArea, cloudSafeArea);
                    decorationYPosition = Random.Range(
                            0 - skyObject.GetComponent<SpriteRenderer>().bounds.extents.y,
                            skyObject.GetComponent<SpriteRenderer>().bounds.extents.y);

                    // Create a cloud sprite object from the randomly selected sprite
                    GameObject decorationObject = Instantiate<GameObject>(cloudGameObjects[randomIndex]);
                    decorationObject.transform.parent = this.transform;

                    // Place the cloud object and give it a name
                    decorationObject.transform.position = new Vector3(
                            skyObject.GetComponent<SpriteRenderer>().bounds.center.x + 
                            decorationXPosition,
                            skyObject.GetComponent<SpriteRenderer>().bounds.center.y + 
                            decorationYPosition,
                            0.0f);
                    decorationObject.name = "Cloud" + i;
                }
            }
        }

        private void AddSpaceTransition() 
        {
            // Create the space transition object
            GameObject spaceTransitionObject = Instantiate<GameObject>(spaceTransitionSprite);
            spaceTransitionObject.transform.parent = this.transform.parent;

            // Update the placement offset
            yOffset += spaceTransitionObject.GetComponent<SpriteRenderer>().bounds.extents.y;

            // Place the space transition object and give it a name
            spaceTransitionObject.transform.position =
                    new Vector3(transform.position.x, transform.position.y + yOffset, 0.0f);
            spaceTransitionObject.name = "SpaceTransition";

            // Update the placement offset and background height
            yOffset += spaceTransitionObject.GetComponent<SpriteRenderer>().bounds.extents.y;
            height += spaceTransitionObject.GetComponent<SpriteRenderer>().bounds.size.y;
        }

        // Adds space sprites to the top of the background
        private void AddSpace()
        {
            // Used to randomly select sprites from the space sprites prefab collection
            int randomIndex;

            // Used to determine if a decoration will be placed on a background sprite
            float decorationChance;
            // Used to store the random position of the decoration
            float decorationXPosition;
            float decorationYPosition;

            // Create and place the space sprites
            for (int i = 0; i < backgroundChunkSize; i++)
            {
                // Randomly select a sprite from the space sprites prefab collection and update the
                // count of space sprites
                randomIndex = Random.Range(0, spaceGameObjects.Count);
                spaceSpriteCount++;

                // Create a space sprite object from the randomly selected sprite
                GameObject spaceObject = Instantiate<GameObject>(spaceGameObjects[randomIndex]);

                // Update the placement offset
                yOffset += spaceObject.GetComponent<SpriteRenderer>().bounds.extents.y;

                // Place the space sprite and give it a name
                spaceObject.transform.position =
                        new Vector3(transform.position.x, transform.position.y + yOffset, 0.0f);
                spaceObject.name = "Space" + spaceSpriteCount;

                // Update the placement offset and background height
                yOffset += spaceObject.GetComponent<SpriteRenderer>().bounds.extents.y;
                height += spaceObject.GetComponent<SpriteRenderer>().bounds.size.y;

                // Generate a random number to determine if this space sprite will have a planet 
                // on it
                decorationChance = Random.Range(0.0f, 1.0f);

                // If decorationChance says we should create a planet, do it
                if (decorationChance <= decorationDensity)
                {
                    // Select a random sprite from the planet sprite list
                    randomIndex = Random.Range(0, planetGameObjects.Count);
                    // Select a random X position for the planet within the safe area
                    decorationXPosition = Random.Range(0 - planetSafeArea, planetSafeArea);
                    decorationYPosition = Random.Range(
                            0 - spaceObject.GetComponent<SpriteRenderer>().bounds.extents.y,
                            spaceObject.GetComponent<SpriteRenderer>().bounds.extents.y);

                    // Create a planet sprite object from the randomly selected sprite
                    GameObject decorationObject = Instantiate<GameObject>(planetGameObjects[randomIndex]);

                    // Place the planet object and give it a name
                    decorationObject.transform.position = new Vector3(
                            spaceObject.GetComponent<SpriteRenderer>().bounds.center.x +
                            decorationXPosition,
                            spaceObject.GetComponent<SpriteRenderer>().bounds.center.y +
                            decorationYPosition,
                            0.0f);
                    decorationObject.name = "Cloud" + i;
                }
            }
        }
    }
}