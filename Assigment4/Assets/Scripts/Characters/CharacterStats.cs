using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New CharacterStats", menuName = "CharacterStats")]
public class CharacterStats : ScriptableObject
{
    public int cost;
    public int hp;
    public int damage;
    public int attackRange;
    public int movementRange;
    public Sprite sprite;
}
