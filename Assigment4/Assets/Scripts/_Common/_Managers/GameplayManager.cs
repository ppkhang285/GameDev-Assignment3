using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameplayManager : MonoBehaviour
{

    private int deckSize;
    public int NumPlayer { get; private set; }
    private BaseCharacter[][] pieces;
    private GameState gameState;
    private int maxAP;
    private bool[] defeated;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public List<Move> GetLegalMoves(GameState gameState, int currentPlayer)
    {
        if (defeated[currentPlayer]) return new List<Move>();
        List<Move> moves = new List<Move>();
        moves.Add(new Move(new Vector2Int(-1, -1), new Vector2Int(-1, -1), MoveType.Idle));
        int size = gameState.Size;
        for (int i = 0; i < deckSize; i++)
        {
            BaseCharacter chessPiece = pieces[currentPlayer][i];
            if (chessPiece.Spawned && chessPiece.Dead)
            {
                Vector2Int location = chessPiece.Location;
                int movementRange = chessPiece.MovementRange;
                int attackRange = chessPiece.AttackRange;
                for (int j = -movementRange; j <= movementRange; j++)
                {
                    for (int k = -movementRange; k <= movementRange; k++)
                    {
                        if (location.x + j < 0 || location.x + j >= size || location.y + k < 0 || location.y + k >= size) continue;
                        if (j != 0 && k != 0) continue;
                        if (gameState.GetCell(i, j).Item1 == -1)
                        {
                            Vector2Int target = new Vector2Int(j, k);
                            Move newMove = new Move(location, target, MoveType.CharMove);
                            moves.Add(newMove);
                        }
                    }
                }
                for (int j = -attackRange; j <= attackRange; j++)
                {
                    for (int k = -attackRange; k <= attackRange; k++)
                    {
                        if (location.x + j < 0 || location.x + j >= size || location.y + k < 0 || location.y + k >= size) continue;
                        if (j != 0 && k != 0) continue;
                        if (gameState.GetCell(i, j).Item1 != -1 && gameState.GetCell(i, j).Item1 != currentPlayer)
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
        if (remainingMoves == 0)
        {
            moveSequences.Add(new List<Move>(currentSequence));
            return;
        }

        List<Move> legalMoves = GetLegalMoves(currentGameState, currentPlayer);

        foreach (Move move in legalMoves)
        {
            // Apply the move to simulate the next board state
            GameState nextGameState = SimulateMove(currentGameState, move, currentPlayer);

            // Add the move to the current sequence
            currentSequence.Add(move);

            // Recurse to generate further sequences
            if (move.Type == MoveType.Spawn)
                GenerateMoveSequencesRecursive(nextGameState, currentSequence, currentPlayer, remainingMoves, moveSequences);
            else
                GenerateMoveSequencesRecursive(nextGameState, currentSequence, currentPlayer, remainingMoves - 1, moveSequences);

            // Backtrack to explore other options
            currentSequence.RemoveAt(currentSequence.Count - 1);
        }
    }

    public GameState SimulateMove(GameState gameState, Move move, int currentPlayer) // Simulate to get the game state after player's move, not actually change the game state
    {
        return null;
    }

    public GameState SimulateMoveSequence(GameState gameState, List<Move> move, int currentPlayer)
    {
        return null;
    }

    public void ApplyMoveSequence(List<Move> move, int currentPlayer) // Actually change the current game state
    {
        gameState = SimulateMoveSequence(gameState, move, currentPlayer);
    }

    public int GetNextPlayer(int currentPlayer)
    {
        return (currentPlayer + 1) % numPlayer; // change logic later when a player is defeated;
    }
}
