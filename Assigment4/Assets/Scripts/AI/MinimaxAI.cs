using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MinimaxAI : BaseAI
{
    private int maxDepth;
    private string method;

    public MinimaxAI(int depth, MLModel model = null, string method = "minimax" ) : base(model)
    {
        maxDepth = depth;
        this.method = method;
    }

    public override List<Move> GetMove(GameState gameState, int currentPlayer)
    {
        if (method == "minimax")
        {
            return Minimax(gameState, maxDepth, currentPlayer).Item2;
        } else
        {
            float[] alpha = new float[gameplayManager.NumPlayer].Select(_ => float.NegativeInfinity).ToArray();
            float[] beta = new float[gameplayManager.NumPlayer].Select(_ => float.PositiveInfinity).ToArray();
            return AlphaBeta(gameState, maxDepth, currentPlayer, alpha, beta).Item2;
        }
    }

    private (float[] bestOutcome, List<Move> bestSequence) Minimax(GameState gameState, int depth, int currentPlayer)
    {
        // If maximum depth is reached or the game is over for the current player
        if (depth == 0 || gameState.HasWon(currentPlayer) || gameState.HasLost(currentPlayer))
        {
            return (Evaluate(gameState), null); 
        }

        float bestValue = float.NegativeInfinity;
        float[] outcome = new float[gameplayManager.NumPlayer];
        List<Move> bestSequence = null;

        List<List<Move>> moveSequences = GetMoveSequences(gameState, currentPlayer);
        foreach (List<Move> sequence in moveSequences)
        {
            GameState newState = SimulateMoveSequence(gameState, sequence);
            float[] value = Minimax(newState, depth - 1, gameplayManager.GetNextPlayer(currentPlayer)).Item1;

            if (model)
                TrainModel(newState, value);

            if (value[currentPlayer] > bestValue)
            {
                bestValue = value[currentPlayer];
                outcome = value;
                bestSequence = sequence;
            }
        }

        return (outcome, bestSequence);
    }

    private (float[] bestOutcome, List<Move> bestSequence) AlphaBeta(GameState gameState, int depth, int currentPlayer, float[] alpha, float[] beta)
    {
        if (depth == 0 || gameState.HasWon(currentPlayer) || gameState.HasLost(currentPlayer))
        {
            return (Evaluate(gameState), null);
        }

        float[] bestOutcome = new float[gameplayManager.NumPlayer];
        List<Move> bestSequence = null;

        List<List<Move>> moveSequences = GetMoveSequences(gameState, currentPlayer);
        foreach (List<Move> sequence in moveSequences)
        {
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
            }

            // Prune the branch if another player can ensure a worse outcome for this player
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

            // Update beta for other players
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
