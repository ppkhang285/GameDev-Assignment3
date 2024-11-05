using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBase.Managers;

public class SoundSettings : MonoBehaviour
{
    bool alreadySet;
    [SerializeField] Slider SFXSlider, MusicSlider;
    void Start()
    {
        StartCoroutine(Cor_WaitUntilLoad());
    }
    IEnumerator Cor_WaitUntilLoad()
    {
        yield return new WaitUntil(() => Game_SoundManager.Instance.LoadSave());
        SFXSlider.value = Game_SoundManager.Instance.GetSFXVolume() * 10f;
        MusicSlider.value = Game_SoundManager.Instance.GetMusicVolume() * 10f;

        alreadySet = true;
    }
    public void SettingSFXVolume()
    {
        Game_SoundManager.Instance.SetSFXVolume(SFXSlider.value / 10f);
        if (alreadySet) Game_SoundManager.Instance.PlaySound(GameBase.AudioPlayer.SoundID.SFX_BUTTON_CLICK);
    }
    public void SettingMusicVolume()
    {
        var musicVolume = MusicSlider.value / 10f;
        Game_SoundManager.Instance.SetMusicVolume(musicVolume);
        if (alreadySet) Game_SoundManager.Instance.PlaySoundHelper(GameBase.AudioPlayer.SoundID.SFX_BUTTON_CLICK, musicVolume);
    }
    public void MuteSFX()
    {
        if (Game_SoundManager.Instance.GetSFXVolume() == 0)
        {
            Game_SoundManager.Instance.SetSFXVolume(5);
            SFXSlider.value = 5;
            Game_SoundManager.Instance.PlaySound(GameBase.AudioPlayer.SoundID.SFX_BUTTON_CLICK, 0.5f);
        }
        else
        {
            Game_SoundManager.Instance.SetSFXVolume(0f);
            SFXSlider.value = 0;
        }
    }
    public void MuteMusic()
    {
        if (Game_SoundManager.Instance.GetMusicVolume() == 0)
        {
            Game_SoundManager.Instance.SetMusicVolume(5);
            MusicSlider.value = 5;
            Game_SoundManager.Instance.PlaySoundHelper(GameBase.AudioPlayer.SoundID.SFX_BUTTON_CLICK, 0.5f);
        }
        else
        {
            Game_SoundManager.Instance.SetMusicVolume(0f);
            MusicSlider.value = 0;
        }
    }
}
