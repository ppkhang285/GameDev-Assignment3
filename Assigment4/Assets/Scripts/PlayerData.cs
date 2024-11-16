using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

public enum PlayerType
{
    Human,
    AI
}

[Serializable]
public class PlayerData
{
    public int PlayerNo { get; set; }
    public PlayerType Type { get; private set; }
    public CharacterData[] Characters { get; set; }
    public int AP {get; set;} 
    public int Energy {get; set;}
    public Vector2Int LordLocation { get; private set; }
    public int LordHP {get; set;}
    public List<Vector2Int> SpawnLocations {get; set;} 

    public PlayerData(int playerNo, PlayerType type, CharacterData[] characters, Vector2Int lordLocation)
    {
        PlayerNo = playerNo;
        Type = type;
        Characters = characters;
        AP = GameConstants.MaxAP;
        Energy = GameConstants.StartEnergy;
        LordHP = GameConstants.LordHP;
        LordLocation = lordLocation;
        SpawnLocations = new List<Vector2Int>();
        if (LordLocation.x == 0)
        {
            SpawnLocations.Add(new Vector2Int(LordLocation.x + 1, LordLocation.y)); // To the right of the lord 
        } else if (LordLocation.x == GameConstants.BoardSize - 1)
        {
            SpawnLocations.Add(new Vector2Int(LordLocation.x - 1, LordLocation.y)); // To the left of the lord 
        } else
        {
            SpawnLocations.Add(new Vector2Int(LordLocation.x, LordLocation.y));
        }

        if (LordLocation.y == 0)
        {
            SpawnLocations.Add(new Vector2Int(LordLocation.x, LordLocation.y + 1)); // Below the lord 
        } else if (LordLocation.y == GameConstants.BoardSize - 1)
        {
            SpawnLocations.Add(new Vector2Int(LordLocation.x, LordLocation.y - 1)); // Above the lord 
        } else
        {
            SpawnLocations.Add(new Vector2Int(LordLocation.x, LordLocation.y));
        }
    }

    public void CharMove(Vector2Int location, int index)
    {
        CharacterData character = Characters[index];
        character.Location = location;
        character.AP -= 1;
    }

    public void CharAttack(int index)
    {
        CharacterData character = Characters[index];
        character.AP -= 1;
    }

    public void CharTakeDmg(int dmg, int index)
    {
        CharacterData character = Characters[index];
        character.CurrentHP -= dmg;
        if (character.CurrentHP <= 0)
        {
            character.Die();
        }
    }

    public void SpawnChar(Vector2Int location, int index)
    {
        CharacterData character = Characters[index];
        Energy -= character.characterStats.cost;
        character.Spawn(location);
    }

    public bool IsDead()
    {
        return LordHP <= 0 || OutOfChess();
    }

    public bool OutOfChess()
    {
        // Still has chess on board
        foreach (CharacterData character in Characters)
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

    public void ClearBoard()
    {
        foreach (CharacterData character in Characters)
        {
            character.Die();
        }
        AP = 0;
        Energy = 0;
        LordLocation = new Vector2Int(-1, -1);
        LordHP = 0;
        SpawnLocations.Clear();
    }


    public List<float> ToFeatures()
    {
        List<float> features = new List<float>();

        features.Add(Energy);
        features.Add(LordHP);
        features.Add(SpawnLocations.Count);

        foreach (CharacterData character in Characters)
        {
            features.AddRange(character.ToFeatures());
        }

        return features;
    }
}
