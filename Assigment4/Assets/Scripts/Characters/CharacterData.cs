using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

[Serializable]
public class CharacterData
{
    // Start is called before the first frame update
    public CharacterStats characterStats;
    private int _hp;
    public int CurrentHP
    {
        get { return _hp; }
        set { if (value <= 0) _hp = 0; else _hp = 1; }
    }

    private int _ap;
    public int AP {
        get { return _ap; }
        set { if (value <= 0) _ap = 0; else _ap = 1; }
    }

    public Vector2Int Location { get; set; }
    public bool Spawned { get; set; }
    public bool Dead { get; set; }

    // TODO: Add constructor 

}
