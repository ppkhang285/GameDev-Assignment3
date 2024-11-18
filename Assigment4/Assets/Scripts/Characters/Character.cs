using NaughtyAttributes;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


public class CharBar
{

    private GameObject bar;
    private TMP_Text teamText;
    private TMP_Text hpText;
    private TMP_Text apText;


    public CharBar(GameObject bar, int teamNumber, int hp, int ap)
    {
        this.bar = bar;
        SetupObject();

        teamNumber += 1;
        teamText.text = "P" + teamNumber.ToString();
        UpdateStats(hp, ap);

    }

    private void SetupObject()
    {
        GameObject slider = bar.transform.GetChild(0).gameObject;

        hpText = slider.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();
        apText = slider.transform.GetChild(2).gameObject.GetComponent<TMP_Text>();
        teamText = bar.transform.GetChild(1).gameObject.GetComponent<TMP_Text>();

       
    }


    public void UpdateStats(int hp, int ap)
    {
        hpText.text = hp.ToString();
        apText.text = ap.ToString();

    }
}

// Attach to the Chess pieces game objects on the board
public class Character : MonoBehaviour
{

    public CharacterData Data { get; set; }

    private CharBar bar;
    private Animator animator;
   
    public void Initialize(string name, int team)
    {
        // CharacterStats characterStats = AssetDatabase.LoadAssetAtPath<CharacterStats>("Assets/Scripts/Characters/Stats/" + name + ".asset");
        CharacterStats characterStats = Resources.Load<CharacterStats>("Stats/" + name);
        Data = new CharacterData(characterStats, team);

        Setup();
    }
    // Start is called before the first frame updates
    void Start()
    {
        //Setup();
        animator = GetComponent<Animator>();
    }


    private void Setup()
    {
        

        GameObject barPrefab = Resources.Load<GameObject>("Gameplay/UI/Char_bar");
        if (barPrefab != null)
        {
            Debug.Log($"No bar prefab loaded at {name}");
        }
        GameObject newBar = Instantiate(barPrefab, transform);

        bar = new CharBar(newBar, Data.PlayerTeam, Data.CurrentHP, Data.AP);

    }


    // Update is called once per frame
    void Update()
    {
        
    }

    public void UpdateStats()
    {
        bar.UpdateStats(Data.CurrentHP, Data.AP);
    }

    public void CharMove(bool direction)
    {
        // The character moves visually
    }

    public void CharAttack(bool direction)
    {
        // The character attacks visually
        StartCoroutine(PlayAttackAnim(direction));
        
    }

    public void TakeDmg()
    {
        // The character takes damage visually
    }

   
    
    public IEnumerator PlayAttackAnim(bool direction)
    {
        string stringDirec = direction ? "right" : "left";
        string attackName = "attack" + "_" + stringDirec;
        string idleName = "idle" + "_" + stringDirec;

        yield return StartCoroutine(PlayAnimation(attackName));

        StartCoroutine(PlayAnimation(idleName));
        
    }


    private IEnumerator PlayAnimation(string animationName)
    {
        
        
        animator.Play(animationName);

        // Wait for the duration of the animation (you should know the length of the animation)
        float animationDuration = GetAnimationDuration(animationName);

        // Wait for the animation to finish
        yield return new WaitForSeconds(animationDuration);

        // Log when the animation has finished
        Debug.Log($"Animation {animationName} finished!");
    }

    private float GetAnimationDuration(string animationName)
    {

        // Check if animator and runtimeAnimatorController are set
        if (animator.runtimeAnimatorController != null)
        {
            // Loop through all animation clips in the animator controller
            foreach (var clip in animator.runtimeAnimatorController.animationClips)
            {
                if (clip.name == animationName)
                {
                    return clip.length; // Return the length of the animation
                }
            }
        }

        // If no matching clip was found, return 0
        Debug.LogWarning($"Animation '{animationName}' not found in animator!");
        return 0;
    }
}
