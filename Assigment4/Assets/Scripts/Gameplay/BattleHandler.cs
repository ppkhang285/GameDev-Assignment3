using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    public BoardSpawner Spawner;
    private enum GameplayState { Start, Playing, GameOver };
    private enum PlayerTurn { Player1Turn, Player2Turn, Player3Turn, Player4Turn }

    private GameplayState currentState;
    private PlayerTurn[] playerPool;
    private PlayerTurn currentPlayer;


    public void StartGameLoop()
    {
        Setup();
        StartCoroutine(GameLoop());
    }

    private void Setup()
    {
        int playerNum = GameplayManager.Instance.NumPlayer;
        Spawner.SpawnBoard();

        // Initialize playerPool based on playerNum
        playerPool = new PlayerTurn[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            playerPool[i] = (PlayerTurn)i;
        }

        currentState = GameplayState.Start;
        currentPlayer = playerPool[0]; // Start with Player 1
    }

    private IEnumerator GameLoop()
    {
        while (currentState != GameplayState.GameOver)
        {
            if (currentState == GameplayState.Start)
            {
               
                currentState = GameplayState.Playing;
                yield return null;
            }

            Debug.Log("Current Player: " + currentPlayer);

            // Handle the current player's turn
            PlayerType playerType = GameplayManager.Instance.players[System.Array.IndexOf(playerPool, currentPlayer)].GetComponent<Player>().Data.Type;
            if (playerType == PlayerType.Human)
                yield return StartCoroutine(HandlePlayerTurn());
            else 
                yield return StartCoroutine(HandleAITurn());

            // After the player's turn, move to the next player
            ChangeTurn();
        }
    }

    private IEnumerator HandleAITurn()
    {
        Debug.Log(currentPlayer + " is taking their turn.");
        List<Move> sequence = GameplayManager.Instance.AI.GetMove(GameplayManager.Instance.gameState, System.Array.IndexOf(playerPool, currentPlayer));
        foreach (Move move in sequence)
        {
            Debug.Log(move.ToString());
        }
        GameplayManager.Instance.ApplyMoveSequence(sequence);
        yield return null;
    }

    private IEnumerator HandlePlayerTurn()
    {
        // Simulate waiting for the player's actions (replace with real input logic)
        Debug.Log(currentPlayer + " is taking their turn.");
        yield return new WaitForSeconds(2f); // Placeholder for player input duration

        // Logic to detect end of turn can be added here
        
    }

    public int GetNextPlayer(int currentIdx) // TODO: Change logic later when a player is defeated;
    {
        
        return (currentIdx + 1) % playerPool.Length;
    }

    public int GetCurrentPlayer()
    {
        return System.Array.IndexOf(playerPool, currentPlayer);
    }

    private void ChangeTurn()
    {
        int currentIdx = GetCurrentPlayer();
        currentPlayer = playerPool[GetNextPlayer(currentIdx)];
        GameplayManager.Instance.ChangeTurn();
    }
}
