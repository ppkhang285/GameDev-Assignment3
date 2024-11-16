using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// Attach to the Chess pieces game objects on the board
public class Character : MonoBehaviour
{
    public CharacterData Data { get; set; }

    public void Initialize(string name)
    {
        CharacterStats characterStats = AssetDatabase.LoadAssetAtPath<CharacterStats>("Assets/Scripts/Characters/Stats/" + name + ".asset");
        Data = new CharacterData(characterStats);
        Debug.Log(Data.characterStats.hp);
    }
    // Start is called before the first frame updates
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
