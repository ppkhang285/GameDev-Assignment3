using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Players : MonoBehaviour
{
    public int PlayerNo { get; set; }
    public Character[] Characters { get; set; } //All characters
    public int AP {get; set;} 
    public int Energy {get; set;}
    public int LordHP {get; set;}
    List<Vector2Int> spawnLocations {get; set;} // Locations for spawning chess of all player 
    // TODO: Adding constructor/ awake  


    public void NewTurn(){ // Invoke when new turn start
        Energy += GameConstants.EnergyPerTurn;
        if (Energy > GameConstants.MaxEnergy){
            Energy = GameConstants.MaxEnergy;
        }
        AP = GameConstants.MaxAP;

    };
    public void TakeDamage(int damage){
        LordHP -= damage;
    }

    public bool Summon(int cost){
        if (cost > Energy) return false;
        Energy -= cost;
        return true; 
    }

    public bool Act(){
        if (AP <= 0) return false;
        --AP;
        return true;
    }

    public bool IsDead(){
        return (LordHP <= 0);
    }

    public bool StillCanSummon()
    {
        if (spawnLocations == null || spawnLocations.Count == 0) return false;
        for (int i = 0; i < Characters.Length; i++){
            if (Characters[i].Spawned == false){
                if (Characters[i].characterStats.cost <= Energy){
                    return true ;
            }
        }
        return false;
        }
    }

    public bool LostSpawnLocationAt(Vector2Int location){
        for (int i = 0; i < spawnLocations.Count; i++){
            if (spawnLocations[i] == location){
                spawnLocations.RemoveAt(i);
                return true;
            }
        }
        return false;
    }

}
