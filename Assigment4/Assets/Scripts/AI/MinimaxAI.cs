using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using System.Diagnostics;

public class MinimaxAI : BaseAI
{
    private int maxDepth;
    private string method;
    private float timeLimit;
    private Stopwatch stopwatch;
    private List<Move> currentBestSequence; // Track the best sequence found so far

    public MinimaxAI(int depth, int moveLimit = 10, float dropChance = 0.3f, float timeLimit = 30.0f, MLModel model = null, string method = "minimax") : base(moveLimit, dropChance, model)
    {
        maxDepth = depth;
        this.method = method;
        this.timeLimit = timeLimit;
        stopwatch = new Stopwatch();
    }

    public override List<Move> GetMove(GameState gameState, int currentPlayer)
    {
        stopwatch.Restart();
        currentBestSequence = null;

        if (method == "minimax")
        {
            var result = Minimax(gameState, maxDepth, currentPlayer);
            stopwatch.Stop();
            return result.Item2 ?? GetFallbackMove(gameState, currentPlayer);
        }
        else
        {
            float[] alpha = new float[GameplayManager.ExtNumberPlayer].Select(_ => float.NegativeInfinity).ToArray();
            float[] beta = new float[GameplayManager.ExtNumberPlayer].Select(_ => float.PositiveInfinity).ToArray();
            var result = AlphaBeta(gameState, maxDepth, currentPlayer, alpha, beta);
            stopwatch.Stop();
            return result.Item2 ?? GetFallbackMove(gameState, currentPlayer);
        }
    }

    private List<Move> GetFallbackMove(GameState gameState, int currentPlayer)
    {
        // Return the best sequence found so far if available
        if (currentBestSequence != null)
        {
            return currentBestSequence;
        }

        // If no sequence was found at all, return the first valid move
        var moves = GetMoveSequences(gameState, currentPlayer);
        return moves.Count > 0 ? moves[0] : new List<Move>();
    }

    private bool IsTimeUp()
    {
        return stopwatch.ElapsedMilliseconds > timeLimit * 1000;
    }

    private (float[] bestOutcome, List<Move> bestSequence) Minimax(GameState gameState, int depth, int currentPlayer)
    {
        if (depth == 0 || IsTimeUp() || gameState.HasWon(currentPlayer) || gameState.HasLost(currentPlayer))
        {
            return (Evaluate(gameState), null);
        }

        float bestValue = float.NegativeInfinity;
        float[] outcome = new float[GameplayManager.ExtNumberPlayer];
        List<Move> bestSequence = null;

        List<List<Move>> moveSequences = GetMoveSequences(gameState, currentPlayer);
        foreach (List<Move> sequence in moveSequences)
        {
            if (IsTimeUp())
                break;

            GameState newState = SimulateMoveSequence(gameState, sequence);
            float[] value = Minimax(newState, depth - 1, gameplayManager.GetNextPlayer(currentPlayer)).Item1;

            if (model)
                TrainModel(newState, value);

            if (value[currentPlayer] > bestValue)
            {
                bestValue = value[currentPlayer];
                outcome = value;
                bestSequence = sequence;

                // Update the current best sequence at the root level
                if (depth == maxDepth)
                {
                    currentBestSequence = sequence;
                }
            }
        }

        return (outcome, bestSequence);
    }

    private (float[] bestOutcome, List<Move> bestSequence) AlphaBeta(GameState gameState, int depth, int currentPlayer, float[] alpha, float[] beta)
    {

        if (depth == 0 || IsTimeUp() || gameState.HasWon(currentPlayer) || gameState.HasLost(currentPlayer))
        {
            return (Evaluate(gameState), null);
        }

        float[] bestOutcome = new float[GameplayManager.ExtNumberPlayer];
        List<Move> bestSequence = null;

        List<List<Move>> moveSequences = GetMoveSequences(gameState, currentPlayer);
        foreach (List<Move> sequence in moveSequences)
        {
            if (IsTimeUp())
                break;

            GameState newState = SimulateMoveSequence(gameState, sequence);
            float[] value = AlphaBeta(
                newState,
                depth - 1,
                gameplayManager.GetNextPlayer(currentPlayer),
                (float[])alpha.Clone(),
                (float[])beta.Clone()
            ).Item1;

            if (model)
                TrainModel(newState, value);

            if (bestOutcome == null || value[currentPlayer] > alpha[currentPlayer])
            {
                bestOutcome = value;
                bestSequence = sequence;
                alpha[currentPlayer] = value[currentPlayer];

                // Update the current best sequence at the root level
                if (depth == maxDepth)
                {
                    currentBestSequence = sequence;
                }
            }

            bool shouldPrune = false;
            for (int i = 0; i < alpha.Length; i++)
            {
                if (i != currentPlayer && alpha[i] >= beta[i])
                {
                    shouldPrune = true;
                    break;
                }
            }

            if (shouldPrune)
            {
                break;
            }

            for (int i = 0; i < beta.Length; i++)
            {
                if (i != currentPlayer)
                {
                    beta[i] = Mathf.Min(beta[i], value[i]);
                }
            }
        }

        return (bestOutcome, bestSequence);
    }
}