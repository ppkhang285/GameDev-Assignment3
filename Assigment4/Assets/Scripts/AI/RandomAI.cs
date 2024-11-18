using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAI : BaseAI
{
    public RandomAI(int moveLimit = 10, float dropChance = 0.3f, MLModel model = null) : base(moveLimit, dropChance, model) { }

    public override List<Move> GetMove(GameState gameState, int currentPlayer)
    {
        List<List<Move>> moveSequences = GetMoveSequences(gameState, currentPlayer);
        int randomIndex = Random.Range(0, moveSequences.Count);
        return moveSequences[randomIndex];
    }
}
