using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // Start is called before the first frame update
    public int Size { get; private set; }
    public int NumPlayer { get; private set; }
    private (int, int) [][] cells; // (-1, -1) means empty cell, first item indicates the player that owns the chess, second item is the type of the chess
    List<int> lordsHP; // Lord's HP of all players
    List<int> energies; // Energy of all players
    List<List<Vector2Int>> spawnLocations; // Locations for spawning chess of all player
    private List<bool> defeated;

    public GameState(int size)
    {
        Size = size;
        for (int i = 0; i < size; i++)
        {
            for (int j = 0; j < size; j++)
            {
                this.cells[i][j] = (-1, -1);
            }
        }
    }

    public (int, int) GetCell(int i, int j)
    {
        return cells[i][j];
    }

    public void SetCell((int, int) val, int i, int j)
    {
        if (val.Item1 >= -1) 
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
}
