using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
public class SceneChanger : MonoBehaviour
{

    private string level = "Easy"; 
    private int deckNo = 0;

    private int AI = 1;
    private bool isPVP = false;


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
        GameplayManager.isPVP = isPVP;
        GameplayManager.ExtChosenDeck = deckNo;
        GameplayManager.ExtLevel = level;
        GameplayManager.ExtNumberPlayer = AI + 1;
        MoveToGameplay();
    }

    public void SetAINumber(int No){
        AI = No;
    }

    public void MoveToGameplay() {
        LoadScene("Gameplay");
    }
}
