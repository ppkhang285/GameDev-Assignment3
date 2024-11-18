using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


// Attached to the Lord game object of the player
public class Player : MonoBehaviour
{
    public PlayerData Data { get; set; }
    public GameObject[] characters;
    public Vector2Int Location { get; private set; }

    public void Initialize(int playerNo, int deckNo, PlayerType type, Vector2Int location)
    {
        characters = new GameObject[GameConstants.DeckSize];
        for (int i = 0; i < GameConstants.DeckSize; i ++) // Initialize all characters in the deck
        {
            int assetNo = 2 * i + deckNo; // Deck 0 includes even-number characters, Deck 1 includes odd-number characters
            // GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Character_" + assetNo.ToString() + ".prefab");
            GameObject prefab = Resources.Load<GameObject>("Characters/Character_" + assetNo.ToString());
            characters[i] = Instantiate(prefab);
            Character character = characters[i].GetComponent<Character>();
            character.Initialize(assetNo.ToString(), playerNo);
        }
        Location = location;

        Data = new PlayerData(playerNo, type, characters.Select(p => p.GetComponent<Character>().Data).ToArray(), location);
    }

    public void RestoreAP()
    {
        foreach (GameObject character in characters)
        {
            character.GetComponent<Character>().UpdateStats();
        }
    }
}
