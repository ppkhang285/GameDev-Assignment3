using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


// Attach to the Chess pieces game objects on the board
public class Character : MonoBehaviour
{
    public CharacterData Data { get; set; }

    public void Initialize(string name, int team)
    {
        CharacterStats characterStats = AssetDatabase.LoadAssetAtPath<CharacterStats>("Assets/Scripts/Characters/Stats/" + name + ".asset");
        Data = new CharacterData(characterStats, team);
    }
    // Start is called before the first frame updates
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CharMove()
    {
        // The character moves visually
    }

    public void CharAttack()
    {
        // The character attacks visually
    }

    public void TakeDmg()
    {
        // The character takes damage visually
    }

    [Button]
    public void Test()
    {

        Animator animator = GetComponent<Animator>();
        Debug.Log(animator.name);
        Debug.Log("Play anim");
        animator.Play("idle_left");
    }
}
