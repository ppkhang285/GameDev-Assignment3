using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using GameBase.Managers;

public class MCTSAI : BaseAI
{
    private int simulations;

    public MCTSAI(Game_Manager gameManager, int simulations, MLModel model = null) : base(gameManager, model)
    {
        this.simulations = simulations;
    }

    public override Move GetMove(GameState gameState, int currentPlayer)
    {
        // Perform MCTS simulations to find the best move
        return PerformMCTS(gameState, currentPlayer);
    }

    private Move PerformMCTS(GameState rootState, int currentPlayer)
    {
        // Example structure for MCTS
        Dictionary<Move, float> moveScores = new Dictionary<Move, float>();
        foreach (Move move in gameManager.GetLegalMoves(rootState, currentPlayer))
        {
            float score = 0f;

            for (int i = 0; i < simulations; i++)
            {
                GameState simulationState = gameManager.ApplyMove(rootState, move, currentPlayer);
                score += Simulate(simulationState, currentPlayer);
            }

            moveScores[move] = score;
        }

        // Return the move with the best average score
        return BestMove(moveScores);
    }

    private float Simulate(GameState state, int currentPlayer)
    {
        if (state.IsGameOver())
        {
            return state.GetGameResultForPlayer(currentPlayer); // 1 for win, -1 for loss, 0 for draw
        }

        // Simulate randomly or use evaluation
        return Evaluate(state, currentPlayer);
    }

    private Move BestMove(Dictionary<Move, float> moveScores)
    {
        float bestScore = float.NegativeInfinity;
        Move bestMove = null;

        foreach (var entry in moveScores)
        {
            if (entry.Value > bestScore)
            {
                bestScore = entry.Value;
                bestMove = entry.Key;
            }
        }

        return bestMove;
    }
}
