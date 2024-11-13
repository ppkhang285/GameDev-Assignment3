using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameState
{
    // Start is called before the first frame update
    public int Size { get; private set; }
    private (int, int) [][] cells; // (-1, -1) means empty cell, first item indicates the player that owns the chess, second item is the type of the chess

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

    public GameState ShallowCopy()
    {
        return (GameState)this.MemberwiseClone();
    }
}
