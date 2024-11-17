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
    

    private Animator animator;
    public void Initialize(string name, int team)
    {
        CharacterStats characterStats = AssetDatabase.LoadAssetAtPath<CharacterStats>("Assets/Scripts/Characters/Stats/" + name + ".asset");
        Data = new CharacterData(characterStats, team);
    }
    // Start is called before the first frame updates
    void Start()
    {
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CharMove()
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
