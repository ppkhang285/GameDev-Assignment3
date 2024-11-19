using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public abstract class BaseAI
{
    protected GameplayManager gameplayManager;

    // Optional Machine Learning model for evaluation
    protected MLModel model;
    protected int moveLimit;
    protected float dropChance;

    public BaseAI(int moveLimit = 10, float dropChance = 0.3f, MLModel model = null)
    {
        this.gameplayManager = GameplayManager.Instance;
        this.model = model;
        this.moveLimit = moveLimit;
        this.dropChance = dropChance;
    }

    public abstract List<Move> GetMove(GameState gameState, int currentPlayer);

    public float[] Evaluate(GameState gameState)
    {
        if (model != null)
        {
            // Use ML model to evaluate the board state
            return model.Predict(gameState.ToFeatures());
        }
        else
        {
            // Use default evaluation function
            float[] evaluations = new float[GameplayManager.ExtNumberPlayer];
            for (int i = 0; i < GameplayManager.ExtNumberPlayer; i++)
            {
                evaluations[i] = 0;
            }
            for (int i = 0; i < gameplayManager.BoardSize; i++)
            {
                for (int j = 0; j < gameplayManager.BoardSize; j++)
                {
                    (int, int, int) cell = gameState.Cells[i][j];
                    if (cell.Item1 == -1 || cell.Item2 == -1) continue;
                    CharacterData chess = gameState.Players[cell.Item1].Characters[cell.Item2];
                    evaluations[cell.Item1] += chess.characterStats.cost * chess.CurrentHP / chess.characterStats.hp;
                }
            }
            return evaluations;
        }
    }

    protected GameState SimulateMoveSequence(GameState gameState, List<Move> moves) // Only simulate the move sequence, does not change the current game state
    {
        GameState newState = DeepCopyUtility.Copy(gameState);
        foreach (Move move in moves)
        {
            newState.ApplyMove(move);
        }
        return newState;
    }

    public List<List<Move>> GetMoveSequences(GameState gameState, int currentPlayer)
    {
        List<List<Move>> moveSequences = new List<List<Move>>();
        GameState newState = DeepCopyUtility.Copy(gameState);
        List<Move> currentSequence = new List<Move>();
        GenerateMoveSequencesRecursive(newState, currentSequence, currentPlayer, GameConstants.MaxAP, moveSequences);

        return moveSequences;
    }

    private void GenerateMoveSequencesRecursive(GameState currentGameState, List<Move> currentSequence, int currentPlayer, int remainingMoves, List<List<Move>> moveSequences)
    {
        // If out of action point, can still spawn chess
        if (remainingMoves == 0)
        {
            if (moveSequences.Count >= moveLimit) return;
            List<Move> legalSpawns = currentGameState.GetLegalSpawn(currentPlayer, dropChance);

            // If cannot spawn, then out of move options
            if (legalSpawns.Count == 0)
            {
                List<Move> copySequence = DeepCopyUtility.Copy(currentSequence);
                moveSequences.Add(copySequence);
                return;
            }

            foreach (Move spawn in legalSpawns)
            {
                if (moveSequences.Count >= moveLimit) return;
                currentSequence.Add(spawn);
                GameState newState = DeepCopyUtility.Copy(currentGameState);
                newState.ApplyMove(spawn);
                GenerateMoveSequencesRecursive(newState, currentSequence, currentPlayer, remainingMoves, moveSequences); // Spawning does not cost action points
                currentSequence.RemoveAt(currentSequence.Count - 1);
            }

            return;
        }

        List<Move> legalMoves = currentGameState.GetLegalMoves(currentPlayer, dropChance);
        foreach (Move move in legalMoves)
        {
            if (moveSequences.Count >= moveLimit) return;
            // Add the move to the current sequence
            currentSequence.Add(move);

            // Apply the move to simulate the next board state
            GameState newState = DeepCopyUtility.Copy(currentGameState);
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

    protected void TrainModel(GameState gameState, float[] value)
    {
        if (model == null) return;

        // Convert game state to feature vectors
        List<float> inputFeatures = gameState.ToFeatures();

        // Train the model
        model.Train(inputFeatures, value);
    }
}
