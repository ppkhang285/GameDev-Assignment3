using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using GameBase.Events;

namespace GameBase.Managers
{
    public class Game_LoadingManager : MonoBehaviour
    {
        public static Game_LoadingManager Instance { get; private set; }

        [SerializeField] GameObject loadingCanvas;
        [SerializeField] Image loadingIcon;

        // Use when having many sprites (random)
        // [SerializeField] List<Sprite> loadingSprites;

        // Use when having 1 sprite
        [SerializeField] Sprite loadingSprite;

        void Awake()
        {
            if (Instance != null)
            {
                Destroy(gameObject);
                return;
            }
            Instance = this;
        }
        // Start is called before the first frame update
        void Start()
        {
            GameEvents.ON_LOADING += HandleLoading;
        }
        void OnDestroy()
        {
            GameEvents.ON_LOADING -= HandleLoading;
        }
        void HandleLoading(bool isLoading)
        {
            if (isLoading)
                this.Log("Start Loading", Color.white);
            else this.Log("End Loading", Color.white);

            loadingCanvas.SetActive(isLoading);

            if (isLoading)
            {
                // Use when having many sprites (random)
                // int rand = Random.Range(0, loadingSprites.Count);
                // loadingIcon.sprite = loadingSprites[rand];

                // Use when having 1 sprite
                loadingIcon.sprite = loadingSprite;

                loadingIcon.SetNativeSize();
            }
        }
    }
}
