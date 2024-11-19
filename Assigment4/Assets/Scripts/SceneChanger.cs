using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{

    public static string level; 
    public static int deckNo;
    public static int NoPlayer; // No of enemy
    public static bool isPVP = true;


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

    public void ChoosePVP(){
        isPVP = true;
        GameplayManager.ExtNumberPlayer = 2;
    }

    public void ChoosePVE(){
        isPVP = false;    
    }

    public void SetNoNumber(int No) {
        NoPlayer = No;
    }

    public void MoveToGameplay() {
        GameplayManager.isPVP = isPVP;
        GameplayManager.ExtChosenDeck = deckNo;
        GameplayManager.ExtLevel = level;
        GameplayManager.ExtNumberPlayer = NoPlayer + 1; 
        LoadScene("Gameplay");
    }

    public  void AfterdeckChoose(){
        if (isPVP) {
            LoadScene("LobbyMenu");
        } else {
           MoveToGameplay();
        }

    }

}
