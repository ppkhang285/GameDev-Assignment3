using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseCharacter : MonoBehaviour
{
    // Start is called before the first frame update
    public int Cost { get; private set; }
    public int HP { get; set; }
    public int Damage { get; private set; }
    public int AttackRange { get; private set; }
    public int MovementRange { get; private set; }

    private int _ap;
    public int AP {
        get { return _ap; }
        set { if (value <= 0) _ap = 0; else _ap = 1; }
    }

    public Vector2Int Location { get; set; }
    public bool Spawned { get; set; }
    public bool Dead { get; set; }

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

}
