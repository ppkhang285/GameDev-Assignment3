using System.Collections;
using System.Linq;
using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class GameState
{
    public int Turn { get; set; } // Current turn in the game
    private (int, int, int) [][] cells; // (-1, -1, 0) means empty cell, first item indicates the player that owns the chess, second item is the type of the chess, third item is its current hp
    Players[]  players; // list of players 

    public GameState(int turn, (int, int, int) [][] cells, Players[] players)
    {
        Turn = turn;

        for (int i = 0; i < cells.Length; i++)
        {
            for (int j = 0; j < cells.Length; j++)
            {
                this.cells[i][j] = cells[i][j]; 
            }
        }

        Array.Copy(players, this.players, players.Length); //TODO: may need to manually deep copy the players
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

    public Players GetPlayer(int i)
    {
        return players[i];
    }

    public bool HasLost(int currentPlayer)
    {
        if (Turn > GameConstants.TurnReceiveEnergy)
            return players[currentPlayer].IsDead();
        else
            return players[currentPlayer].LordHP <= 0;
    }

    public bool HasWon(int currentPlayer)
    {
        for (int i = 0; i < players.Length; i++)
        {
            if (i != currentPlayer)
            {
                if (!HasLost(i)) return false;
            }
        }
        return true;
    }

    public List<float> ToFeatures()
    {
        List<float> features = new List<float>();

        features.Add(Turn);

        foreach (var row in cells)
        {
            foreach (var cell in row)
            {
                features.Add(cell.Item1); // Owner of the chess
            }
        }

        // Add each player's lord HP
        foreach (Players player in players)
        {
            features.AddRange(player.ToFeatures());
        }

        return features;
    }
}
