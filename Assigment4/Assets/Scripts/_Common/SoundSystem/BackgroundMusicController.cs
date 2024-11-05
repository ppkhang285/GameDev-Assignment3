using UnityEngine;
using GameBase.Managers;
using GameBase.AudioPlayer;
using NaughtyAttributes;

public class BackgroundMusicController : MonoBehaviour
{
    [SerializeField] bool introMusic;
    [SerializeField, ShowIf("introMusic")] SoundID backgroundIntroMusic;
    [SerializeField] SoundID backgroundMusic;
    void Start()
    {
        var sound = Game_SoundManager.Instance;
        if (introMusic)
        {
            sound.PlayMusicWithIntro(backgroundIntroMusic, backgroundMusic);
        }
        else
        {
            var audioSource = sound.PlayMusic(backgroundMusic);
            if (audioSource != null) audioSource.loop = true;
        }
    }
}
