using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RandomAI : BaseAI
{
    public RandomAI(MLModel model = null) : base(model) { }

    public override List<Move> GetMove(GameState gameState, int currentPlayer)
    {
        List<List<Move>> moveSequences = GetMoveSequences(gameState, currentPlayer);
        int randomIndex = Random.Range(0, moveSequences.Count);
        return moveSequences[randomIndex];
    }
}
