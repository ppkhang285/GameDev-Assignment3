using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public int DeckSize { get; private set; }
    public int BoardSize { get; private set; }
    public int NumPlayer { get; private set; }
    private Player[] players;
    //private Character [][] pieces;
    private GameState gameState;


    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); // Prevent duplicate instances
        }

        DeckSize = GameConstants.DeckSize;
        BoardSize = GameConstants.BoardSize;
        NumPlayer = 2; // TODO: receive num player from other scenes
        players = new Player[NumPlayer];
        Vector2Int[] locations = new Vector2Int[NumPlayer];

        if (NumPlayer < 2 || NumPlayer > 4)
        {
            throw new System.Exception("Invalid number of players");
        } else {
            locations[0] = new Vector2Int(0, 0);
            if (NumPlayer == 2)
            {
                locations[1] = new Vector2Int(BoardSize - 1, BoardSize - 1);
            } else if (NumPlayer == 3)
            {
                locations[1] = new Vector2Int(BoardSize - 1, 0);
                locations[2] = new Vector2Int(0, BoardSize - 1);
            } else
            {
                locations[1] = new Vector2Int(BoardSize - 1, 0);
                locations[2] = new Vector2Int(0, BoardSize - 1);
                locations[3] = new Vector2Int(BoardSize - 1, BoardSize - 1);
            }
        }
        for (int i = 0; i < NumPlayer; i++)
        {
            GameObject player = new GameObject("DefaultObject");
            players[i] = player.AddComponent<Player>();
            players[i].Initialize(i, i, PlayerType.AI, locations[i]);
        }
    }

    public int GetNextPlayer(int currentPlayer)
    {
        return (currentPlayer + 1) % NumPlayer; // TODO: Change logic later when a player is defeated;
    }

    

    public void ApplyMoveSequence(List<Move> moves) // Actually change the current game state
    {
        foreach (Move move in moves)
        {
            gameState.ApplyMove(move);
        }
    }

}
