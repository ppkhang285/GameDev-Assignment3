using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GameBase.Enums;
using GameBase.Events;
using GameBase.Common;
using GameBase.Constants;
using GameBase.AudioPlayer;

namespace GameBase.Managers
{
    public class Game_Manager : MonoBehaviour
    {
        public static Game_Manager Instance { get; private set; }
        [SerializeField] GameObject sceneTransition;

        [SerializeField] int buildVersion;
        [SerializeField] int buildTime;
        [SerializeField] int buildSubTime;
        [SerializeField] TMP_Text versionText;
        public int BuildVersion => this.buildVersion;
        public int BuildTime => this.buildTime;
        public int BuildSubTime => this.buildSubTime;
        public AudioSource AudioSource { get; set; }
        public bool isLoadSceneComplete = true;
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
            Game_SceneTransition sceneTransitionInstance = FindObjectOfType<Game_SceneTransition>();
            if (sceneTransitionInstance == null)
            {
                Instantiate(this.sceneTransition, transform);
            }
#if ENABLE_CHEAT
            this.runtimeConsole.SetActive(true);
            this.versionText.text = $"Version {this.buildVersion}.{this.buildTime}.{this.buildSubTime} - Cheat";
#else
            this.versionText.text = $"Version {this.buildVersion}.{this.buildTime}.{this.buildSubTime} - Release";
#endif
        }

        private void Start()
        {
            SaveLoadManager.Instance.Load();
        }
        void OnApplicationQuit()
        {
            //ProfileManager.Instance.Save();
        }
        public void LoadSoundMap(SoundType soundType)
        {
            Game_SoundManager.Instance.LoadSoundMap(soundType);
            AudioSource?.Stop();
        }
        public void UnloadSoundMap(SoundType soundType)
        {
            Game_SoundManager.Instance.UnloadSoundMap(soundType);
        }
        public void PlayMusic(SoundID soundID, float volume = 1f)
        {
            AudioSource = Game_SoundManager.Instance.PlayMusic(soundID, volume);
        }
        void LoadSceneWithTransition(SceneType sceneType, bool isTransitionOut, TransitionType transitionType, SoundType soundType, System.Action cb = null)
        {
            TransitionIn(
                () => GameEvents.LOAD_SCENE(
                    sceneType,
                    () =>
                    {
                        if (isTransitionOut)
                        {
                            TransitionOut(null, transitionType);
                        }
                        cb?.Invoke();
                    }
                ), transitionType, soundType
            );
        }
        void LoadSceneAsyncWithTransition(SceneType sceneType, bool isTransitionOut, TransitionType transitionType, SoundType soundType, System.Action cb = null)
        {
            TransitionIn(
                () => GameEvents.LOAD_SCENE_ASYNC(
                    sceneType,
                    () =>
                    {
                        if (isTransitionOut)
                        {
                            TransitionOut(null, transitionType);
                        }
                        cb?.Invoke();
                    }
                ), transitionType, soundType
            );
        }
        void UnloadScene(SceneType sceneType, System.Action cb = null)
        {
            GameEvents.UNLOAD_SCENE(sceneType, cb);
        }
        void ReLoadSceneWithTransition(bool isTransitionOut, TransitionType transitionType, System.Action cb = null)
        {
            TransitionIn(
                () =>
                {
                    if (isTransitionOut)
                    {
                        TransitionOut(null, transitionType);
                    }
                    cb?.Invoke();
                }
                , transitionType
            );
        }
        public void ReLoadSceneManually(TransitionType transitionType, System.Action cb = null)
        {
            isLoadSceneComplete = false;
            ReLoadSceneWithTransition(true, transitionType, cb);
        }
        /// <summary>
        /// Use to load scene. Remembere to give SoundType if you want to load another SoundMap. If you load another SoundMap, it means you Unload the current SoundMap
        /// </summary>
        public void LoadSceneManually(SceneType sceneType, TransitionType transitionType, SoundType soundType = SoundType.NONE, System.Action cb = null, bool isStopAllMusicPlaying = false)
        {
            isLoadSceneComplete = false;
            //if (isStopAllMusicPlaying || soundType != SoundType.NONE) 
            if (isStopAllMusicPlaying && soundType != SoundType.NONE)
                Game_SoundManager.Instance.StopAllMusicPlaying();
            LoadSceneWithTransition(sceneType, true, transitionType, soundType, cb);
        }
        /// <summary>
        /// Use to load scene. Remembere to give SoundType if you want to load another SoundMap. If you load another SoundMap, it means you Unload the current SoundMap
        /// </summary>
        public void LoadSceneAsyncManually(SceneType sceneType, TransitionType transitionType, SoundType soundType = SoundType.NONE, System.Action cb = null, bool isStopAllMusicPlaying = false)
        {
            isLoadSceneComplete = false;
            if (isStopAllMusicPlaying || soundType != SoundType.NONE)
                Game_SoundManager.Instance.StopAllMusicPlaying();
            LoadSceneAsyncWithTransition(sceneType, true, transitionType, soundType, cb);
        }
        public void UnloadSceneManually(SceneType sceneType, System.Action cb = null)
        {
            UnloadScene(sceneType, cb);
        }
        // public void LoadSceneManually(string sceneName, TransitionType transitionType)
        // {
        //     SceneType sceneType = GameSceneManager.TranslateToSceneType(sceneName);
        //     if (sceneType == SceneType.UNKNOWN)
        //     {
        //         Debug.LogError("Cannot load scene: " + sceneName + ", Scene Name is not in SceneConfig yet!");
        //         return;
        //     }
        //     LoadSceneWithTransition(sceneType, true, transitionType);
        // }
        public void TransitionIn(System.Action cb = null, TransitionType transitionType = TransitionType.NONE, SoundType soundType = SoundType.NONE)
        {
            StartCoroutine(Cor_TransitionIn(cb, transitionType, soundType));
        }
        IEnumerator Cor_TransitionIn(System.Action cb = null, TransitionType transitionType = TransitionType.NONE, SoundType soundType = SoundType.NONE)
        {
            if (soundType != SoundType.NONE)
            {
                Game_SoundManager.Instance.ClearSoundMapExceptCommonSoundMap();
                Game_SoundManager.Instance.LoadSoundMap(soundType);
            }

            Game_SceneTransition.Instance.TransitionIn(transitionType);
            yield return new WaitForSeconds(GameConstants.TRANSITION_TIME);
            GameEvents.ON_LOADING?.Invoke(true);
            cb?.Invoke();
        }
        public void TransitionOut(System.Action cb = null, TransitionType transitionType = TransitionType.NONE)
        {
            StartCoroutine(Cor_TransitionOut(cb, transitionType));
        }
        IEnumerator Cor_TransitionOut(System.Action cb = null, TransitionType transitionType = TransitionType.NONE)
        {
            yield return new WaitForSeconds(GameConstants.TRANSITION_TIME);
            yield return null;
            Game_SceneTransition.Instance.TransitionOut(transitionType);
            //SetInitData();
            //SaveLoadManager.Instance.Load();
            GameEvents.ON_LOADING?.Invoke(false);
            cb?.Invoke();
            yield return new WaitForSeconds(0.6f);
            isLoadSceneComplete = true;
        }
        //public void SetInitData(int chapterIndex, int levelIndex)
        //{
        //    StartCoroutine(Cor_InitData(chapterIndex, levelIndex));
        //}
        //public void LoadMenuChapter()
        //{
        //    StartCoroutine(Cor_LoadMenuChapter());
        //}
        //public void LoadMenuLevel(int chapterIndex)
        //{
        //    StartCoroutine(Cor_LoadMenuLevel(chapterIndex));
        //}
        //private IEnumerator Cor_LoadMenuChapter()
        //{
        //    yield return new WaitUntil(() => UIManager.Instance != null);
        //    UIManager.Instance.IntoChapterMenu();
        //}
        //private IEnumerator Cor_LoadMenuLevel(int chapterIndex)
        //{
        //    yield return new WaitUntil(() => UIManager.Instance != null);
        //    UIManager.Instance.IntoLevelMenu(chapterIndex);
        //}
        //private IEnumerator Cor_InitData(int chapterIndex, int levelIndex)
        //{
        //    yield return new WaitUntil(() => GameplayManager.Instance != null);
        //    //string levelName = "Level_" + (levelIndex+1).ToString();

        //    GameplayManager.Instance.LoadLevel(chapterIndex, levelIndex);
        //}

        
    }
}
