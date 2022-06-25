// --------------------------------------------
// Written by Andrés Fernandez
// with contributions from Dave Schaub
// --------------------------------------------

using UnityEngine.Audio;
using System;
using UnityEngine;
using UnityEngine.UI;

namespace VineyVibes
{
    // Manages the game's sound effects and background music
    public class AudioManager : MonoBehaviour
    {
        // player prefs
        private static readonly string firstPlay = "firstPlay";
        private static readonly string MusicPref = "MusicPref";
        private static readonly string SoundPref = "SoundPref";

        // Stores whether this is the first time the game has launched
        private int firstPlayInt;

        // sliders
        public Slider musicSlider, soundSlider;

        // floats for volume
        private float musicVolume, soundVolume;
        
        // Components that play sound effect and music audio
        public AudioSource musicAudioSource;
        public AudioSource soundEffectSource;

        // Start is called before the first frame update
        private void Start()
        {
            // fetch player pref
            firstPlayInt = PlayerPrefs.GetInt(firstPlay);

            // check if this is the first play
            if(firstPlayInt == 0)
            {
                // define default audio values
                musicVolume = 0.13f;
                soundVolume = 0.5f;

                // update max values
                musicSlider.maxValue = musicVolume;
                soundSlider.maxValue = soundVolume;

                // update slider values
                musicSlider.value = musicSlider.maxValue;
                soundSlider.value = soundSlider.maxValue;

                // save values as new player prefs
                PlayerPrefs.SetFloat(MusicPref, musicVolume);
                PlayerPrefs.SetFloat(SoundPref, soundVolume);

                // update first play
                PlayerPrefs.SetInt(firstPlay, -1);

            }
            else
            {
                // update music
                musicVolume = PlayerPrefs.GetFloat(MusicPref);
                musicSlider.value = musicVolume;

                // update sound
                soundVolume = PlayerPrefs.GetFloat(SoundPref);
                soundSlider.value = soundVolume;
            }
        
        }

        // Plays the currently selected sound effect
        public void PlaySoundEffect()
        {
            soundEffectSource.Play();
        }

        // updates the sound effects
        public void UpdateSoundEffectsVolume()
        {
            // beta code...will be updated
            soundEffectSource.volume = soundSlider.value;

            // save settings
            PlayerPrefs.SetFloat(SoundPref, soundSlider.value);
        }

        // updates the music volume
        public void UpdateMusicVolume()
        {
            // update music
            musicAudioSource.volume = musicSlider.value;

            // save settings
            PlayerPrefs.SetFloat(MusicPref, musicSlider.value);
        }
    }
}


