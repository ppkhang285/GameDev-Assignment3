using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEditor;

[Serializable]
public class CharacterData
{
    // Start is called before the first frame update
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

    // TODO: Add constructor 
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
}
