using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BattleHandler : MonoBehaviour
{
    private enum GameplayState { Start, Playing, GameOver };
    private enum PlayerTurn { Player1Turn, Player2Turn, Player3Turn, Player4Turn }

    private GameplayState currentState;
    // private PlayerTurn[] playerPool;
    private Queue<PlayerTurn> playerPool;

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
        playerPool = new Queue<PlayerTurn>();
        for (int i = 0; i < playerNum; i++)
        {
            playerPool.Enqueue((PlayerTurn)i);
        }
        currentState = GameplayState.Start;
        currentPlayer = playerPool.Dequeue();
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

    public int GetNextPlayer(bool[] defeated) 
    {
        this.CheckPlayer(defeated);
        // Mimic cycling queue
        int currentPlayer = int(playerPool.Dequeue());
        playerPool.Enqueue(currentPlayer);
        return currentPlayer;
    }

    private void ChangeTurn()
    {
        int currentIdx = System.Array.IndexOf(playerPool, currentPlayer);
        currentPlayer = playerPool[GetNextPlayer(currentIdx)];
    }
    public int SeekNextPlayer(){
        this.CheckQueue(defeated);
        return (int)playerPool.Peek();
    }

    private void CheckPlayer(bool[] defeated){
        while (defeated [(int)playerPool.Peek()]);
            playerPool.Dequeue();
    }
}

