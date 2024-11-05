using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase.Managers;

namespace GameBase.AudioPlayer
{
    public class MusicController : MonoSingleton<MusicController>
    {
        private AudioSource currentAudioSource;
        public void PlayMusic(SoundID musicID)
        {
            if (currentAudioSource)
                currentAudioSource.Stop();
            currentAudioSource = Game_SoundManager.Instance.PlayMusic(musicID);
        }
        public void PauseCurrentMusic()
        {
            if (currentAudioSource)
                currentAudioSource.Pause();
        }
        public void ResumeCurrentMusic()
        {
            if (currentAudioSource)
                currentAudioSource.UnPause();
        }

        public void StopCurrentMusic()
        {
            if (currentAudioSource)
                currentAudioSource.Stop();
        }

        public bool isPlaying()
        {
            return currentAudioSource.isPlaying;
        }
    }
}