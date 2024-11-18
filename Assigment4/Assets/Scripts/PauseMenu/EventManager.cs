using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EventManager : MonoBehaviour
{
    public GameObject pauseMenu;
    public GameObject settingMenu;
    public Slider masterVolumeSlider;
    public Slider musicSlider;
    public Slider sfxSlider;
    public AudioSource masterVolumeSource;
    public AudioSource musicSource;
    public AudioSource sfxSource;
    // Start is called before the first frame update
    void Start()
    {
        masterVolumeSlider.value = masterVolumeSource.volume;
        musicSlider.value = musicSource.volume;
        sfxSlider.value = sfxSource.volume;
    }

    // Update is called once per frame
    void Update()
    {
        //event key press
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (Time.timeScale == 0)
            {
                OnResumedGame();
            }
            else
            {
                OnPausedGame();
            }
        }

    }
    public void OnPausedGame()
    {
        pauseMenu.SetActive(true);
        Time.timeScale = 0;
    }
    public void OnResumedGame()
    {
        pauseMenu.SetActive(false);
        Time.timeScale = 1;
    }
    public void OnOpenSettingGame()
    {
        settingMenu.SetActive(true);
        pauseMenu.SetActive(false);
    }
    public void OnCloseSettingGame()
    {
        settingMenu.SetActive(false);
        pauseMenu.SetActive(true);
    }
    public void OnQuitGame()
    {
        //Thay bằng hàm leave game bên pun 2
        Debug.Log("Game Quit");
        Application.Quit();
    }
    public void OnUpdateMasterVolume(float value)
    {
        masterVolumeSource.volume = value;
    }
    public void OnUpdateMusicVolume(float value)
    {
        musicSource.volume = value;
    }
    public void OnUpdateSFXVolume(float value)
    {
        sfxSource.volume = value;
    }
}
