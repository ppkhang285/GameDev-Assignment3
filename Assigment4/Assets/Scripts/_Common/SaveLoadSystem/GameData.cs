using System.Collections.Generic;
using UnityEngine;
using System;
using GameBase.Configuration;
using GameBase.Enums;
using GameBase.Constants;

namespace GameBase.Managers
{
    [Serializable]
    public struct GameData
    {
        public bool isHaveSaveData;
        public bool isPlayedTutorial;
        public int currentLevel;

        public bool IsSaveLoadProcessing;

        public void SetupData()
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = SaveLoadManager.Instance.GameDataOrigin;

            isHaveSaveData = gameDataOrigin.isHaveSaveData;
            isPlayedTutorial = gameDataOrigin.isPlayedTutorial;
            currentLevel = gameDataOrigin.currentLevel;

            SaveLoadManager.Instance.GameDataOrigin = gameDataOrigin;
            IsSaveLoadProcessing = false;
        }

        public GameDataOrigin ConvertToGameDataOrigin()
        {
            IsSaveLoadProcessing = true;
            GameDataOrigin gameDataOrigin = new GameDataOrigin();

            gameDataOrigin.isHaveSaveData = isHaveSaveData;
            gameDataOrigin.isPlayedTutorial = isPlayedTutorial;
            gameDataOrigin.currentLevel = currentLevel;

            IsSaveLoadProcessing = false;
            return gameDataOrigin;
        }
    }

    [Serializable]
    public struct GameDataOrigin
    {
        public bool isHaveSaveData;
        public bool isPlayedTutorial;
        public int currentLevel;
    }
}
