using System.Collections;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
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
            yield return StartCoroutine(HandlePlayerTurn());

            // After the player's turn, move to the next player
            ChangeTurn();
        }
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

    private void ChangeTurn()
    {
        int currentIdx = System.Array.IndexOf(playerPool, currentPlayer);
        currentPlayer = playerPool[GetNextPlayer(currentIdx)];
    }
}
