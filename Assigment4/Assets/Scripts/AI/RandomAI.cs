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

        //List<Move> legalMoves = gameState.GetLegalMoves(currentPlayer, dropChance);
        //int randomIndex = Random.Range(0, legalMoves.Count);
        //List<Move> moves = new List<Move>();
        //moves.Add(legalMoves[randomIndex]);
        //return moves;
    }
}
