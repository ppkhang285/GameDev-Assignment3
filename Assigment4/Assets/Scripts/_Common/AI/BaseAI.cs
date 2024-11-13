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

    public float Evaluate(GameState gameState)
    {
        if (model != null)
        {
            // Use ML model to evaluate the board state
            return model.Predict(gameState.ToFeatures());
        }
        else
        {
            // Use fallback evaluation function
            return gameplayManager.Evaluate(gameState);
        }
    }
}
