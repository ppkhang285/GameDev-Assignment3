using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    CharAttack, // Source is the location of the character, target is the location of the target
    CharMove, // Source is the before location of the character, target is the after location of the character
    Spawn, // Source is the player, index of the character, target is the spawning location
    Idle // Source = target = (-1, -1)
}

public class Move
{
    // Start is called before the first frame update
    public Vector2Int Source { get; private set; }
    public Vector2Int Target { get; private set; }
    public MoveType Type { get; private set; }

    public Move(Vector2Int source, Vector2Int target, MoveType moveType)
    {
        Source = source;
        Target = target;
        Type = moveType;
    }
}
