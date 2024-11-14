using System.Collections;
using System.Linq;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    public int Size { get; private set; } // Size of the board
    public int Turn { get; set; } // Current turn in the game
    private (int, int, int) [][] cells; // (-1, -1, 0) means empty cell, first item indicates the player that owns the chess, second item is the type of the chess, third item is its current hp
    int[] lordsHP; // Lord's HP of all players
    int[] energies; // Energy of all players
    List<List<Vector2Int>> spawnLocations; // Locations for spawning chess of all player

    public GameState(int size)
    {
        // TODO: implement constructor
        Size = size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                this.cells[i][j] = (-1, -1, 0); // initialize empty cells, TODO: intialize cells for lords
            }
        }
    }

    public (int, int, int) GetCell(int i, int j)
    {
        return cells[i][j];
    }

    public void SetCell((int, int, int) val, int i, int j)
    {
        if (val.Item1 >= -1 && val.Item3 >= 0) 
        {
            cells[i][j] = val;
        }
        
    }

    public int GetLordHP(int player)
    {
        return lordsHP[player];
    }

    public void SetLordHP(int player, int hp)
    {
        lordsHP[player] = Mathf.Max(hp, 0);
    }

    public int GetEnergy(int player)
    {
        return energies[player];
    }

    public void SetEnergy(int player, int energy)
    {
        if (energy > GameConstants.MaxEnergy)
            energies[player] = GameConstants.MaxEnergy;

        if (energy < 0)
            energies[player] = 0;
    }

    public List<Vector2Int> GetSpawnLocation(int player)
    {
        return spawnLocations[player];
    }

    public GameState ShallowCopy()
    {
        return (GameState)this.MemberwiseClone();
    }

    public List<float> ToFeatures()
    {
        List<float> features = new List<float>();

        features.Add(Turn);

        foreach (var row in cells)
        {
            foreach (var cell in row)
            {
                // Encode cell ownership and type:
                features.Add(cell.Item1); // Owner
                features.Add(cell.Item2); // Chess type
                features.Add(cell.Item3); // Its current health
            }
        }

        // Add each player's lord HP
        features.AddRange(lordsHP.Select(hp => (float)hp));

        // Add each player's energy
        features.AddRange(energies.Select(energy => (float)energy));

        // Add spawn locations (flattened into coordinates)
        foreach (var locations in spawnLocations)
        {
            foreach (var loc in locations)
            {
                features.Add(loc.x);
                features.Add(loc.y);
            }
        }

        return features;
    }
}
