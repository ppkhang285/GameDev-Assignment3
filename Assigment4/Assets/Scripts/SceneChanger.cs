using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ChooseDeck(int deckNo) {
        GameplayManager.ExtChosenDeck = deckNo;
        MoveToGameplay();
    }

    public void ChooseLevel(string level = "Easy") {
        GameplayManager.ExtLevel = level;
    }

    public void MoveToGameplay() {
        LoadScene("Gameplay");
    }

}
