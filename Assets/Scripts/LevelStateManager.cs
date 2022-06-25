using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace VineyVibes 
{
    public class LevelStateManager : MonoBehaviour
    {
        public int buildingChunkCount = 3;
        public int skyChunkCount = 5;
        public LevelState State 
        {
            get { return state; }
        }

        private int chunkCount;

        public LevelStateManager() {
            state = LevelState.Grass;
            chunkCount = 1;
        }

        
        // backing field for the state
        private LevelState state;

        public LevelState Next() 
        {
            switch(state) 
            {
                case LevelState.Grass: 
                    state = LevelState.Building;
                    break;
                case LevelState.Building:
                    if(chunkCount > buildingChunkCount)
                    {
                        state = LevelState.SkyTransition;
                        chunkCount = 1;
                    }
                    else 
                    {
                        chunkCount++;
                    }
                    break;
                case LevelState.SkyTransition:
                    state = LevelState.Sky;
                    break;
                case LevelState.Sky:
                    if(chunkCount > skyChunkCount) 
                    {
                        state = LevelState.SpaceTransition;
                        chunkCount = 1;
                    }
                    else 
                    {
                        chunkCount++;
                    }
                    break;
                case LevelState.SpaceTransition:
                    state = LevelState.Space;
                    break;
                case LevelState.Space:
                    state = LevelState.Space;
                    break;
                default: break;
            }

            return state;
        }
    }

}
