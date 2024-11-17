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

            //Debug.Log("Current Player: " + currentPlayer);

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
        //Debug.Log(currentPlayer + " is taking their turn.");
        List<Move> sequence = GameplayManager.Instance.AI.GetMove(GameplayManager.Instance.gameState, System.Array.IndexOf(playerPool, currentPlayer));
        GameplayManager.Instance.ApplyMoveSequence(sequence);
        yield return null;
    }

    private IEnumerator HandlePlayerTurn()
    {
        // Simulate waiting for the player's actions (replace with real input logic)
        // Debug.Log(currentPlayer + " is taking their turn.");
        List<Move> sequence = GameplayManager.Instance.AI.GetMove(GameplayManager.Instance.gameState, System.Array.IndexOf(playerPool, currentPlayer));
        bool turnEnded = false;

        while (!turnEnded)
        {
            if (Input.GetKeyDown(KeyCode.Return)) // TODO: check "End Turn" button click
            {
                Debug.Log("Player ended their turn.");
                GameplayManager.Instance.ApplyMoveSequence(sequence);
                turnEnded = true;
            }

            if (Input.GetMouseButtonDown(0)) // Left mouse click
            {
                Vector2Int clickedCell = Spawner.GetClickedCell();
                Debug.Log($"Clicked cell: Row {clickedCell.y}, Column {clickedCell.x}");
            }

            // Wait until the next frame to check again
            yield return null;
        }
        
    }

    public int GetNextPlayer(int currentIdx) // TODO: Change logic later when a player is defeated;
    {
        bool[] defeated = GameplayManager.Instance.gameState.Defeated;
        for (int i = 1; i < playerPool.Length; i++)
        {
            if (!defeated[(currentIdx + i) % playerPool.Length]) return (currentIdx + i) % playerPool.Length;
        }
        return currentIdx;
    }

    public int GetCurrentPlayer()
    {
        return System.Array.IndexOf(playerPool, currentPlayer);
    }

    private void ChangeTurn()
    {
        int currentIdx = GetCurrentPlayer();
        int nextIdx = GetNextPlayer(currentIdx);
        if (nextIdx == currentIdx)
        {
            Debug.Log("Player " + nextIdx.ToString() + " has won");
            currentState = GameplayState.GameOver;
            return;
        }
        currentPlayer = playerPool[nextIdx];
        GameplayManager.Instance.ChangeTurn();
    }
}
