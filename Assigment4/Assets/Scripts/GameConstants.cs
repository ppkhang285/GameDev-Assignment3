using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GameConstants
{
    // Energy
    public const int StartEnergy = 5;
    public const int EnergyPerTurn = 3;
    public const int TurnReceiveEnergy = 10;
    public const int MaxEnergy = StartEnergy + EnergyPerTurn * TurnReceiveEnergy;
    
    // Game config
    public const int MaxPlayer = 4;
    public const int BoardSize = 10;
    public const int MaxAP = 3;
    public const int MaxCharacter = 5;
}
