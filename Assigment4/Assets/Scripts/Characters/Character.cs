using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


// Attach to the Chess pieces game objects on the board
public class Character : MonoBehaviour
{
    [SerializeField] private CharacterData data;

    public void Initialize(string name)
    {
        CharacterStats characterStats = AssetDatabase.LoadAssetAtPath<CharacterStats>("Assets/Scripts/Characters/Stats/" + name + ".asset");
        data = new CharacterData(characterStats);
        Debug.Log(data.characterStats.hp);
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
