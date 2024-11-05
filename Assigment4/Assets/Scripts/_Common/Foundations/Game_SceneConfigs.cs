using RotaryHeart.Lib.SerializableDictionary;
using UnityEngine;
using GameBase.Enums;
using GameBase.Managers;

namespace GameBase.Configuration
{
    [CreateAssetMenu(menuName = "GameBase/Scene Config", fileName = "Scene Config")]
    public class Game_SceneConfigs : ScriptableObject
    {
        public SceneData data;
        public string GetSceneNameByType(SceneType type)
        {
            if (!this.data.ContainsKey(type))
                return Game_SceneManager.TranslateToSceneName(SceneType.UNKNOWN);
            return this.data[type];
        }
    }
    [System.Serializable]
    public class SceneData : SerializableDictionaryBase<SceneType, string> { }
}
