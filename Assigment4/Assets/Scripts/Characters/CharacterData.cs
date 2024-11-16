using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class CharacterData
{
    public CharacterStats characterStats;
    private int _hp;
    public int CurrentHP
    {
        get { return _hp; }
        set { if (value <= 0) _hp = 0; else _hp = 1; }
    }

    private int _ap;
    public int AP {
        get { return _ap; }
        set { if (value <= 0) _ap = 0; else _ap = 1; }
    }

    public Vector2Int Location { get; set; }
    public bool Spawned { get; set; }
    public bool Dead { get; set; }

    public CharacterData(CharacterStats stats) // Default constructor for unspawned chess
    {
        characterStats = stats;
        CurrentHP = characterStats.hp;
        AP = 0;
        Location = new Vector2Int(-1, -1);
        Spawned = false;
        Dead = false;
    }

    public CharacterData(CharacterStats stats, int hp, int ap, Vector2Int loc, bool spawned, bool dead)
    {
        characterStats = stats;
        CurrentHP = hp;
        AP = ap;
        Location = loc;
        Spawned = spawned;
        Dead = dead;
    }

    public void Spawn(Vector2Int location)
    {
        Spawned = true;
        CurrentHP = characterStats.hp;
        AP = 1;
        Location = location;
    }

    public void Die()
    {
        CurrentHP = 0;
        AP = 0;
        Location = new Vector2Int(-1, -1);
        Spawned = true;
        Dead = true;
    }

    public List<float> ToFeatures()
    {
        List<float> features = new List<float>();
        features.Add(characterStats.cost);
        features.Add(characterStats.hp);
        features.Add(characterStats.damage);
        features.Add(characterStats.attackRange);
        features.Add(characterStats.movementRange);
        features.Add(CurrentHP);
        features.Add(AP);
        features.Add(Location.x);
        features.Add(Location.y);
        if (Spawned)
            features.Add(1);
        else
            features.Add(0);
        if (Dead)
            features.Add(1);
        else
            features.Add(0);
        return features;
    }
}
