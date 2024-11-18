using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utils
{
    public static int ManhattanDistance(Vector2Int source, Vector2Int target)
    {
        return Mathf.Abs(source.x - target.x) + Mathf.Abs(source.y - target.y);
    }
}
