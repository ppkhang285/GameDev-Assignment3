using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class Player : MonoBehaviour
{
    public int PlayerNo { get; set; }
    public Character[] Characters { get; set; } //All characters
    public int AP {get; set;} 
    public int Energy {get; set;}
    public int LordHP {get; set;}
    public List<Vector2Int> SpawnLocations {get; set;} // Locations for spawning chess of all player 
    // TODO: Adding constructor/ awake  


    public void CharMove(Vector2Int location, int index)
    {
        Character character = Characters[index];
        character.Location = location;
        character.AP -= 1;
    }

    public void CharAttack(int index)
    {
        Character character = Characters[index];
        character.AP -= 1;
    }

    public void CharTakeDmg(int dmg, int index)
    {
        Character character = Characters[index];
        character.CurrentHP -= dmg;
        if (character.CurrentHP <= 0)
        {
            character.Dead = true;
        }
    }

    public void SpawnChar(Vector2Int location, int index)
    {
        Character character = Characters[index];
        Energy -= character.characterStats.cost;
        character.Spawned = true;
        character.CurrentHP = character.characterStats.hp;
        character.AP = 1;
        character.Location = location;
    }

    public bool IsDead()
    {
        return LordHP <= 0 || OutOfChess();
    }

    public bool OutOfChess()
    {
        // Still has chess on board
        foreach (Character character in Characters)
        {
            if (character.Spawned && !character.Dead) return false;
        }

        // No spawn locations left
        if (SpawnLocations == null || SpawnLocations.Count == 0) return true;

        // No more energy to spawn
        for (int i = 0; i < Characters.Length; i++)
        { 
            if (!Characters[i].Spawned)
            {
                if (Characters[i].characterStats.cost <= Energy)
                    return false;
            }
        }
        return true;
    }

    public bool LostSpawnLocationAt(Vector2Int location)
    {
        for (int i = 0; i < SpawnLocations.Count; i++)
        {
            if (SpawnLocations[i] == location)
            {
                SpawnLocations.RemoveAt(i);
                return true;
            }
        }
        return false;
    }


    public List<float> ToFeatures()
    {
        List<float> features = new List<float>();

        features.Add(Energy);
        features.Add(LordHP);
        features.Add(SpawnLocations.Count);

        foreach (Character character in Characters)
        {
            features.Add(character.characterStats.cost);
            if (!character.Spawned)
            {
                features.Add(character.CurrentHP);
                features.Add(1);
            }
            else
            {
                features.Add(character.characterStats.hp);
                if (!character.Dead)
                    features.Add(2);
                else
                    features.Add(0);
            }
        }

        return features;
    }
}
