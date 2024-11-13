using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinimaxAI : BaseAI
{
    private int maxDepth;

    public MinimaxAI(GameplayManager gameplayManager, int depth, MLModel model = null) : base(gameplayManager, model)
    {
        maxDepth = depth;
    }

    public override List<Move> GetMove(GameState gameState, int currentPlayer)
    {
        float bestValue = float.NegativeInfinity;
        List<Move> bestSequence = null;

        foreach (List<Move> sequence in gameplayManager.GetMoveSequences(gameState, currentPlayer))
        {
            GameState newState = gameplayManager.SimulateMoveSequence(gameState, sequence, currentPlayer);
            List<float> value = Minimax(newState, maxDepth - 1, gameplayManager.GetNextPlayer(currentPlayer));

            if (value[currentPlayer] > bestValue)
            {
                bestValue = value[currentPlayer];
                bestSequence = sequence;
            }
        }

        return bestSequence;
    }

    private List<float> Minimax(GameState gameState, int depth, int currentPlayer)
    {
        if (depth == 0 || gameState.IsGameOver(currentPlayer))
        {
            return Evaluate(gameState);
        }

        float bestValue = float.NegativeInfinity;
        List<float> outcome = new List<float>();

        foreach (List<Move> sequence in gameplayManager.GetMoveSequences(gameState, currentPlayer))
        {
            GameState newState = gameplayManager.SimulateMoveSequence(gameState, sequence, currentPlayer);
            List<float> value = Minimax(newState, depth - 1, gameplayManager.GetNextPlayer(currentPlayer));

            TrainModel(newState, value);

            if (value[currentPlayer] > bestValue)
            {
                bestValue = value[currentPlayer];
                outcome = value;
            }
        }

        return outcome;
    }

    private List<float> AlphaBetaSearch(GameState gameState, int depth, float alpha, float beta, int currentPlayer)
    {
        if (depth == 0 || gameplayManager.IsGameOver(currentPlayer))
        {
            return Evaluate(gameState);
        }

        List<float> bestOutcome = new List<float>();
        bestOutcome.AddRange(Enumerable.Repeat(float.NegativeInfinity, gameplayManager.NumPlayer));

        foreach (List<Move> sequence in gameplayManager.GetMoveSequences(gameState, currentPlayer))
        {
            GameState newState = gameplayManager.SimulateMoveSequence(gameState, sequence, currentPlayer);
            List<float> value = AlphaBetaSearch(newState, depth - 1, alpha, beta, gameplayManager.GetNextPlayer(currentPlayer));

            if (value[currentPlayer] > bestOutcome[currentPlayer])
            {
                bestOutcome[currentPlayer] = value[currentPlayer];
            }

            // Alpha-beta pruning
            alpha = Mathf.Max(alpha, bestOutcome[currentPlayer]);
            if (alpha >= beta)
            {
                break;
            }
        }

        return bestOutcome;
    }
}
