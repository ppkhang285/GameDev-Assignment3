using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{
    public static GameplayManager Instance { get; private set; }

    public int DeckSize { get; private set; }
    public int BoardSize { get; private set; }
    public int NumPlayer { get; private set; }
    //private Player[] players;
    //private Character [][] pieces;
    private GameState gameState;
    private int maxAP;
    private bool[] defeated;


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

        //players = FindObjectsOfType<Player>();
        GameObject newObject = new GameObject("DefaultObject");
        Character testChar = newObject.AddComponent<Character>();
        testChar.Initialize("1");
    }

    public int GetNextPlayer(int currentPlayer)
    {
        return (currentPlayer + 1) % NumPlayer; // TODO: Change logic later when a player is defeated;
    }

    public List<List<Move>> GetMoveSequences(GameState gameState, int currentPlayer)
    {
        List<List<Move>> moveSequences = new List<List<Move>>();
        GameState newState = DeepCopyUtility.DeepCopy(gameState);
        GenerateMoveSequencesRecursive(newState, new List<Move>(), currentPlayer, maxAP, moveSequences);

        return moveSequences;
    }

    private void GenerateMoveSequencesRecursive(GameState currentGameState, List<Move> currentSequence, int currentPlayer, int remainingMoves, List<List<Move>> moveSequences)
    {
        // If out of action point, can still spawn chess
        if (remainingMoves == 0) 
        {
            List<Move> legalSpawns = currentGameState.GetLegalSpawn(currentPlayer);

            // If cannot spawn, then out of move options
            if (legalSpawns.Count == 0)
            {
                moveSequences.Add(currentSequence);
                return;
            }

            foreach(Move spawn in legalSpawns)
            {
                currentSequence.Add(spawn);
                GameState newState = DeepCopyUtility.DeepCopy(currentGameState);
                newState.ApplyMove(spawn);
                GenerateMoveSequencesRecursive(newState, currentSequence, currentPlayer, remainingMoves, moveSequences); // Spawning does not cost action points
                currentSequence.RemoveAt(currentSequence.Count - 1);
            }
            
            return;
        }

        List<Move> legalMoves = currentGameState.GetLegalMoves(currentPlayer);
        foreach (Move move in legalMoves)
        {
            // Add the move to the current sequence
            currentSequence.Add(move);

            // Apply the move to simulate the next board state
            GameState newState = DeepCopyUtility.DeepCopy(currentGameState);
            newState.ApplyMove(move);

            // Recurse to generate further sequences
            if (move.Type == MoveType.Spawn)
                GenerateMoveSequencesRecursive(newState, currentSequence, currentPlayer, remainingMoves, moveSequences); // Spawning does not cost action points
            else
                GenerateMoveSequencesRecursive(newState, currentSequence, currentPlayer, remainingMoves - 1, moveSequences);

            // Backtrack to explore other options
            currentSequence.RemoveAt(currentSequence.Count - 1);
        }
    }

    public GameState SimulateMoveSequence(GameState gameState, List<Move> moves) // Only simulate the move sequence, does not change the current game state
    {
        GameState newState = DeepCopyUtility.DeepCopy(gameState);
        foreach (Move move in moves)
        {
            newState.ApplyMove(move);
        }
        return newState;
    }

    public void ApplyMoveSequence(List<Move> moves) // Actually change the current game state
    {
        foreach (Move move in moves)
        {
            gameState.ApplyMove(move);
        }
    }

}
