using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{

    public static string level; 
    public static int deckNo;

    public static int NoPlayer; // No of enemy
    public static bool isPVP;


    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }
    public void ChooseDeck(int No) {
        deckNo = No;
    }

    public void ChooseLevel(string lvl = "Easy") {
        level = lvl;
    }

    public void StartPVP(){
        GameplayManager.isPVP = true;
        GameplayManager.ExtChosenDeck = deckNo;
        MoveToGameplay();
    }

    public void StartPVE(){
        GameplayManager.isPVP = false;
        GameplayManager.ExtChosenDeck = deckNo;
        GameplayManager.ExtLevel = level;
        GameplayManager.ExtNumberPlayer = NoPlayer + 1;    
        MoveToGameplay();
    }

    public void SetNoNumber(int No) {
        NoPlayer = No;
    }

    public void MoveToGameplay() {
        LoadScene("Gameplay");
    }
}
