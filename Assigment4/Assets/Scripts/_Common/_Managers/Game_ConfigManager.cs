using UnityEngine;
using GameBase.Configuration;

namespace GameBase.Managers
{
    public class Game_ConfigManager : MonoBehaviour
    {
        public static Game_ConfigManager Instance { get; private set; }
        public Game_SceneConfigs Game_SceneConfig;

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
    }
}
