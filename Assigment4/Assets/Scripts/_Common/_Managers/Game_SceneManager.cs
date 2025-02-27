using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using GameBase.Enums;
using GameBase.Events;
using GameBase.Constants;
using GameBase.Configuration;

namespace GameBase.Managers
{
    public class Game_SceneManager : MonoBehaviour
    {
        public static readonly string UNKNOWN_SCENE_NAME = "Unknown";

        // Singleton Section
        public static Game_SceneManager Instance { get; private set; }
        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }


        void Start()
        {
            // * Subscribe 
            GameEvents.LOAD_SCENE += HandleLoadScene;
            GameEvents.LOAD_SCENE_ASYNC += HandleLoadSceneAsync;
            GameEvents.UNLOAD_SCENE += HandleUnloadScene;
        }
        void OnDestroy()
        {
            // * Unsubscribe
            GameEvents.LOAD_SCENE -= HandleLoadScene;
            GameEvents.LOAD_SCENE_ASYNC -= HandleLoadSceneAsync;
            GameEvents.UNLOAD_SCENE -= HandleUnloadScene;
        }

        private void HandleLoadSceneAsync(SceneType type, Action cb = null)
        {
            string message = string.Format("Request load scene {0} asynchronously", type.ToString());
            this.Log(message, Color.yellow);
            StartCoroutine(Cor_LoadSceneAsync(type, cb));
        }

        private void HandleUnloadScene(SceneType type, System.Action cb = null)
        {
            string message = string.Format("Request unload scene {0} asynchronously", type.ToString());
            this.Log(message, Color.yellow);
            StartCoroutine(Cor_UnloadScene(type, cb));
        }

        public void HandleLoadScene(SceneType type, System.Action cb = null)
        {
            string message = string.Format("Request load scene {0}", type.ToString());
            this.Log(message, Color.yellow);
            StartCoroutine(Cor_LoadScene(type, cb));
        }
        IEnumerator Cor_UnloadScene(SceneType type, System.Action cb = null)
        {
            yield return new WaitUntil(() => Game_ConfigManager.Instance != null);
            string sceneName = Game_ConfigManager.Instance.Game_SceneConfig.GetSceneNameByType(type);
            if (sceneName.Equals(UNKNOWN_SCENE_NAME))
            {
                string log = string.Format("Invalid scene type {0} with name {1}", type.ToString(), sceneName);
                this.Log(log, Color.red);
                yield break;
            }
            var async = SceneManager.UnloadSceneAsync(sceneName);
            while (!async.isDone) yield return null;
            string message = string.Format("Scene {0} unloaded successfully", type.ToString());
            this.Log(message, Color.green);
            cb?.Invoke();
        }
        IEnumerator Cor_LoadSceneAsync(SceneType type, System.Action cb = null)
        {
            yield return new WaitUntil(() => Game_ConfigManager.Instance != null);
            string sceneName = Game_ConfigManager.Instance.Game_SceneConfig.GetSceneNameByType(type);
            if (sceneName.Equals(UNKNOWN_SCENE_NAME))
            {
                string log = string.Format("Invalid scene type {0} with name {1}", type.ToString(), sceneName);
                this.Log(log, Color.red);
                yield break;
            }
            var async = SceneManager.LoadSceneAsync(sceneName, LoadSceneMode.Additive);
            while (!async.isDone) yield return null;
            SceneManager.SetActiveScene(SceneManager.GetSceneByName(sceneName));
            string message = string.Format("Scene {0} loaded asynchronously successfully", type.ToString());
            this.Log(message, Color.green);
            cb?.Invoke();
        }
        IEnumerator Cor_LoadScene(SceneType type, System.Action cb = null)
        {
            yield return new WaitUntil(() => Game_ConfigManager.Instance != null);
            string sceneName = Game_ConfigManager.Instance.Game_SceneConfig.GetSceneNameByType(type);
            if (sceneName.Equals(UNKNOWN_SCENE_NAME))
            {
                string log = string.Format("Invalid scene type {0} with name {1}", type.ToString(), sceneName);
                this.Log(log, Color.red);
                yield break;
            }
            SceneManager.LoadScene(sceneName);
            string message = string.Format("Scene {0} loaded successfully", type.ToString());
            this.Log(message, Color.green);
            cb?.Invoke();
        }
        public static SceneType TranslateToSceneType(string sceneName)
        {
            Game_SceneConfigs sceneConfig = Game_ConfigManager.Instance.Game_SceneConfig;
            foreach (var (type, name) in sceneConfig.data)
            {
                if (name == sceneName)
                {
                    return type;
                }
            }
            return SceneType.UNKNOWN;
        }
        public static string TranslateToSceneName(SceneType sceneType)
        {
            Game_SceneConfigs sceneConfig = Game_ConfigManager.Instance.Game_SceneConfig;
            if (!sceneConfig.data.ContainsKey(sceneType))
                return UNKNOWN_SCENE_NAME;
            return sceneConfig.data[sceneType];
        }
    }
}

