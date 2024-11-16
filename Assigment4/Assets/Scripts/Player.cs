using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// Attached to the Lord game object of the player
public class Player : MonoBehaviour
{
    public PlayerData Data { get; set; }
    private Character[] characters;
    public Vector2Int Location { get; private set; }

    public void Initialize(int playerNo, int deckNo, PlayerType type, Vector2Int location)
    {
        CharacterData[] characterData = new CharacterData[GameConstants.DeckSize];
        characters = new Character[GameConstants.DeckSize];
        for (int i = 0; i < GameConstants.DeckSize; i ++) // Initialize all characters in the deck
        {
            GameObject character = new GameObject("DefaultObject");
            characters[i] = character.AddComponent<Character>();

            int assetNo = 2 * i + deckNo; // Deck 0 includes even-number characters, Deck 1 includes odd-number characters
            characters[i].Initialize(assetNo.ToString(), playerNo);
            characterData[i] = characters[i].Data;
        }
        Location = location;

        Data = new PlayerData(playerNo, type, characterData, location);
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
        Data.CharMove(location, index); // Move logically
        characters[index].CharMove();
    }

    public void CharAttack(int index)
    {
        Data.CharAttack(index); // Attack logically
        characters[index].CharAttack();
    }

    public void CharTakeDmg(int dmg, int index)
    {
        Data.CharTakeDmg(dmg, index);
        characters[index].TakeDmg();
    }

    public void SpawnChar(Vector2Int location, int index)
    {
        Data.SpawnChar(location, index);
        characters[index].Spawn();
    }

    public void TakeDmg(int dmg)
    {
        Data.LordHP -= dmg;
    }

}
