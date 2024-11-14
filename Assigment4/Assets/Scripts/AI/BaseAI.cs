using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAI
{
    protected GameplayManager gameplayManager;

    // Optional Machine Learning model for evaluation
    protected MLModel model;

    public BaseAI(GameplayManager gameplayManager, MLModel model = null)
    {
        this.gameplayManager = gameplayManager;
        this.model = model;
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
            float[] evaluations = new float[gameplayManager.NumPlayer];
            for (int i = 0; i < gameplayManager.NumPlayer; i++)
            {
                evaluations[i] = CalculatePoints(gameState, i);
            }
            return evaluations;
        }
    }

    private float CalculatePoints(GameState gameState, int player)
    {
        float points = 0;
        
        // Points equal to Cost of all chess pieces belonging to the player * its percentage of health left
        for (int i = 0; i < gameplayManager.BoardSize; i++)
        {
            for (int j = 0; j < gameplayManager.BoardSize; j++)
            {
                (int, int, int) cell = gameState.GetCell(i, j);
                if (cell.Item1 != player) continue;
                Character chess = gameplayManager.GetPiece(player, cell.Item2);
                points += chess.characterStats.cost * chess.CurrentHP / chess.characterStats.hp;
            }
        }
        return points;
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
