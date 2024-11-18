using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenManager : MonoBehaviour
{
    public GameObject level1Button;
    public GameObject level2Button;
    public GameObject level3Button;
    public GameObject easyButton;
    public GameObject mediumButton;
    private int level { get; set; }
    private string difficulty { get; set; }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    void OnClickLevelButton(int level)
    {
        easyButton.SetActive(true);
        mediumButton.SetActive(true);
        this.level = level;
    }
    public void OnClickLevelOne()
    {
        OnClickLevelButton(1);
    }
    public void OnClickLevelTwo()
    {
        OnClickLevelButton(2);
    }
    public void OnClickLevelThree()
    {
        OnClickLevelButton(3);
    }
    void ChooseDifficulty(string difficulty)
    {
        this.difficulty = difficulty;
        //change scene to DeckCollection
        UnityEngine.SceneManagement.SceneManager.LoadScene("DeckCollection");
    }
    public void OnClickEasy()
    {
        ChooseDifficulty("Easy");
    }
    public void OnClickNormal()
    {
        ChooseDifficulty("Normal");
    }
}
