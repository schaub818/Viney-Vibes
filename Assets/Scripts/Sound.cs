using UnityEngine.Audio;
using UnityEngine;

namespace VineyVibes
{
    [System.Serializable]
    public class Sound
    {
        // the audio source of the clip
        [HideInInspector]
        public AudioSource source;
        
        // name of the sound
        public string name;
        
        // the type of the sound (sfx, music)
        public SoundType type;
        
        // the audio clip to be played
        public AudioClip clip;

        // volume of the clip
        [Range(0f, 1f)]
        public float volume;

        // the pitch of the clip (default is 1)
        [Range(0.1f, 3f)]
        public float pitch;

        // whether or not the clip loops
        public bool loop;

        

    }

}

