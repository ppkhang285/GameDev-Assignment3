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
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Characters/Character_" + assetNo.ToString() + ".prefab");
            characters[i] = Instantiate(prefab);
            Character character = characters[i].GetComponent<Character>();
            character.Initialize(assetNo.ToString(), playerNo);
            //SpriteRenderer renderer = prefab.GetComponent<SpriteRenderer>();
            //renderer.sprite = character.Data.characterStats.sprite;
            //renderer.enabled = false;
        }
        Location = location;

        Data = new PlayerData(playerNo, type, characters.Select(p => p.GetComponent<Character>().Data).ToArray(), location);
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void CharMove(Vector2Int location, int index)
    {

    }

    public void CharAttack(int index)
    {

    }

    public void CharTakeDmg(int dmg, int index)
    {

    }

    public void SpawnChar(Vector2Int location, int index)
    {

    }

}
