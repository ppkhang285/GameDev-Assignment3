using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    public int DeckSize { get; private set; }
    public int BoardSize { get; private set; }
    public int NumPlayer { get; private set; }
    private BaseCharacter[][] pieces;
    private GameState gameState;
    private int maxAP;
    private bool[] defeated;

    public GameplayManager(int deckSize, int numPlayer, int boardSize)
    {
        // TODO: implement constructor
    }

    public BaseCharacter GetPiece(int player, int index)
    {
        return pieces[player][index];
    }

    public int GetNextPlayer(int currentPlayer)
    {
        return (currentPlayer + 1) % NumPlayer; // TODO: Change logic later when a player is defeated;
    }

    // Check if player has won
    public bool HasWon(int player)
    {
        if (defeated[player]) return false;
        bool hasWon = true;

        // Other players are defeated
        for (int i = 0; i < NumPlayer; i++)
        {
            if (i != player && !defeated[i])
                hasWon = false;
        }

        return hasWon;
    }

    // Check if player is defeated
    public bool IsDefeated(int player)
    {
        return defeated[player];
    }

    // Return possible chess spawn
    public List<Move> GetLegalSpawn(GameState gameState, int currentPlayer)
    {
        if (defeated[currentPlayer]) return new List<Move>(); // If player is defeated, cannot do anything
        List<Move> moves = new List<Move>();

        // Iterate through spawning locations
        foreach (Vector2Int spawnLocation in gameState.GetSpawnLocation(currentPlayer))
        {
            if (gameState.GetCell(spawnLocation.x, spawnLocation.y).Item1 != -1) // Spawning location is not occupied by other chess 
            {
                continue;
            }
            for (int i = 0; i < DeckSize; i++)
            {
                BaseCharacter chessPiece = pieces[currentPlayer][i];
                if (!chessPiece.Spawned && chessPiece.AP <= gameState.GetEnergy(currentPlayer)) // Chess is not spawned and enough energy to spawn
                {
                    moves.Add(new Move(spawnLocation, spawnLocation, MoveType.Spawn));
                }
            }
        }

        return moves;
    }

    public List<Move> GetLegalMoves(GameState gameState, int currentPlayer)
    {
        if (defeated[currentPlayer]) return new List<Move>(); // If player is defeated, cannot do anything
        List<Move> moves = new List<Move>();

        // Doing nothing is also an option
        moves.Add(new Move(new Vector2Int(-1, -1), new Vector2Int(-1, -1), MoveType.Idle)); 

        // Spawn chess
        moves.AddRange(GetLegalSpawn(gameState, currentPlayer));

        // Move or attack
        for (int i = 0; i < DeckSize; i++)
        {
            BaseCharacter chessPiece = pieces[currentPlayer][i];
            if (chessPiece.Spawned && !chessPiece.Dead) // Chess is spawned and not dead
            {
                Vector2Int location = chessPiece.Location;
                int movementRange = chessPiece.MovementRange;
                int attackRange = chessPiece.AttackRange;

                // Possible cells to move
                for (int j = -movementRange; j <= movementRange; j++)
                {
                    for (int k = -movementRange; k <= movementRange; k++)
                    {
                        if (location.x + j < 0 || location.x + j >= BoardSize || location.y + k < 0 || location.y + k >= BoardSize) continue;
                        if (j != 0 && k != 0) continue;
                        if (gameState.GetCell(i, j).Item1 == -1) // Cell is empty
                        {
                            Vector2Int target = new Vector2Int(j, k);
                            Move newMove = new Move(location, target, MoveType.CharMove);
                            moves.Add(newMove);
                        }
                    }
                }

                // Possible cells to attack
                for (int j = -attackRange; j <= attackRange; j++)
                {
                    for (int k = -attackRange; k <= attackRange; k++)
                    {
                        if (location.x + j < 0 || location.x + j >= BoardSize || location.y + k < 0 || location.y + k >= BoardSize) continue;
                        if (j != 0 && k != 0) continue;
                        if (gameState.GetCell(i, j).Item1 != -1 && gameState.GetCell(i, j).Item1 != currentPlayer) // Cell is not empty and the piece in the cell is of enemy team 
                        {
                            Vector2Int target = new Vector2Int(j, k);
                            Move newMove = new Move(location, target, MoveType.CharAttack);
                            moves.Add(newMove);
                        }
                    }
                }
            }
        }
        return moves;
    }

    public List<List<Move>> GetMoveSequences(GameState gameState, int currentPlayer)
    {
        List<List<Move>> moveSequences = new List<List<Move>>();

        GenerateMoveSequencesRecursive(gameState, new List<Move>(), currentPlayer, maxAP, moveSequences);

        return moveSequences;
    }

    private void GenerateMoveSequencesRecursive(GameState currentGameState, List<Move> currentSequence, int currentPlayer, int remainingMoves, List<List<Move>> moveSequences)
    {
        // If out of action point, can still spawn chess
        if (remainingMoves == 0) 
        {
            List<Move> legalSpawns = GetLegalSpawn(currentGameState, currentPlayer);

            // If cannot spawn, then out of move options
            if (legalSpawns.Count == 0)
            {
                moveSequences.Add(currentSequence);
                return;
            }

            foreach(Move spawn in legalSpawns)
            {
                currentSequence.Add(spawn);
                GameState nextGameState = SimulateMove(currentGameState, spawn, currentPlayer);
                GenerateMoveSequencesRecursive(nextGameState, currentSequence, currentPlayer, remainingMoves, moveSequences); // Spawning does not cost action points
                currentSequence.RemoveAt(currentSequence.Count - 1);
            }
            
            return;
        }

        List<Move> legalMoves = GetLegalMoves(currentGameState, currentPlayer);
        foreach (Move move in legalMoves)
        {
            // Add the move to the current sequence
            currentSequence.Add(move);

            // Apply the move to simulate the next board state
            GameState nextGameState = SimulateMove(currentGameState, move, currentPlayer);

            // Recurse to generate further sequences
            if (move.Type == MoveType.Spawn)
                GenerateMoveSequencesRecursive(nextGameState, currentSequence, currentPlayer, remainingMoves, moveSequences); // Spawning does not cost action points
            else
                GenerateMoveSequencesRecursive(nextGameState, currentSequence, currentPlayer, remainingMoves - 1, moveSequences);

            // Backtrack to explore other options
            currentSequence.RemoveAt(currentSequence.Count - 1);
        }
    }

    public GameState SimulateMove(GameState gameState, Move move, int currentPlayer) // Simulate to get the game state after player's move, not actually change the game state
    {
        if (move.Type == MoveType.Idle) // Doing nothing does not change the game state
            return gameState;

        return null;
    }

    public GameState SimulateMoveSequence(GameState gameState, List<Move> moves, int currentPlayer)
    {
        GameState newState = gameState.ShallowCopy();
        // Apply the moves sequentially
        foreach (Move move in moves)
        {
            newState = SimulateMove(newState, move, currentPlayer);
        }
        return newState;
    }

    public void ApplyMoveSequence(List<Move> move, int currentPlayer) // Actually change the current game state
    {
        gameState = SimulateMoveSequence(gameState, move, currentPlayer);
    }

}
