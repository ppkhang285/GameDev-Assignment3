using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

namespace GameBase.Managers
{
    public class SaveLoadManager : MonoBehaviour
    {
        static public SaveLoadManager Instance { get; private set; }
        public GameDataOrigin GameDataOrigin;
        public GameData GameData;

        private void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }

        [Button]
        public void Save()
        {
            GameDataOrigin = GameData.ConvertToGameDataOrigin();
            StartCoroutine(Cor_SaveLoadProgress("Save success"));
            //print("SAVE game");
        }
        [Button]
        public void Load()
        {
            SaveLoadSystem.LoadData(GameDataOrigin);
            GameData.SetupData();

            //StartCoroutine(Cor_LoadPlayer());
            StartCoroutine(Cor_SaveLoadProgress("Load successfully"));
            //Debug.Log("Load successed");
        }
        [Button]
        public void ResetData()
        {
            GameDataOrigin gamedataOrigin = new GameDataOrigin();
            GameDataOrigin = gamedataOrigin;

            SaveLoadSystem.SaveData(GameDataOrigin);
            GameData.SetupData();
            StartCoroutine(Cor_ResetData());
            //Debug.Log("Reset data successed");
        }

        IEnumerator Cor_SaveLoadProgress(string progressStr)
        {
            yield return new WaitUntil(() => this.GameData.IsSaveLoadProcessing == false);
            SaveLoadSystem.SaveData(GameDataOrigin);
            this.Log(progressStr, Color.green);
        }
        IEnumerator Cor_ResetData()
        {
            //Save();
            yield return new WaitUntil(() => this.GameData.IsSaveLoadProcessing == false);
            Load();
            yield return new WaitUntil(() => this.GameData.IsSaveLoadProcessing == false);
            //StartCoroutine(Cor_LoadPlayer());
            StartCoroutine(Cor_SaveLoadProgress("reset data success"));
        }
    }
}

