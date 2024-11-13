using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum MoveType
{
    CharAttack,
    CharMove,
    Spawn,
    Idle
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
