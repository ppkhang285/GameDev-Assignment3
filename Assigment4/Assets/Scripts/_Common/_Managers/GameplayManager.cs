using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

public class GameplayManager : MonoBehaviour

{
    public BattleHandler battleHandler;
    public static GameplayManager Instance { get; private set; }
    // Adding Player Info through this
    public static int ExtNumberPlayer = 2;
    public static int ExtChosenDeck;
    public static int ExtNumberOfAI  = 0;
    public static bool isPVP = false;
    

    public static string ExtLevel = "Easy"; 
    public int DeckSize { get; private set; }
    public int BoardSize { get; private set; }
    public int NumPlayer { get; private set; }
    public GameObject[] players;
    //private Character [][] pieces;
    public GameState gameState;
    //private bool[] defeated;
    public const int MyPlayer = 0;
    public BaseAI AI;

    
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject); 
        }

        DeckSize = GameConstants.DeckSize;
        BoardSize = GameConstants.BoardSize;
        NumPlayer = 2; // TODO: receive num player from other scenes
        players = new GameObject[NumPlayer];
        Vector2Int[] locations = new Vector2Int[NumPlayer];

        if (NumPlayer < 2 || NumPlayer > 4)
        {
            throw new System.Exception("Invalid number of players");
        } else {
            locations[0] = new Vector2Int(0, 0);
            if (NumPlayer == 2)
            {
                locations[1] = new Vector2Int(BoardSize - 1, BoardSize - 1);
            } else if (NumPlayer == 3)
            {
                locations[1] = new Vector2Int(BoardSize - 1, 0);
                locations[2] = new Vector2Int(0, BoardSize - 1);
            } else
            {
                locations[1] = new Vector2Int(BoardSize - 1, 0);
                locations[2] = new Vector2Int(0, BoardSize - 1);
                locations[3] = new Vector2Int(BoardSize - 1, BoardSize - 1);
            }
        }
        for (int i = 0; i < NumPlayer; i++)
        {
            // GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Lords/Lord_" + i.ToString() + ".prefab");
            GameObject prefab = Resources.Load<GameObject>("Lords/Lord_" + i.ToString());
            players[i] = Instantiate(prefab);
            Player player = players[i].GetComponent<Player>();
            player.Initialize(i, i, PlayerType.Human, locations[i]);
            prefab.transform.position = battleHandler.Spawner.GetWorldPosition(locations[i]);
        }

        string level = "Easy";
        if (level == "Easy")
            AI = new RandomAI(100, 1.0f);
        if (level == "Normal")
            AI = new MinimaxAI(3, 10, 0.0f, 30.0f);

        (int, int, int)[][] cells = new (int, int, int)[BoardSize][];
        for (int i = 0; i < BoardSize; i++)
        {
            cells[i] = new (int, int, int)[BoardSize];
            for (int j = 0; j < BoardSize; j++)
            {
                cells[i][j] = (-1, -1, 0);
                for (int k = 0; k < players.Length; k++)
                {
                    Vector2Int lordLocation = locations[k];
                    if (lordLocation.x == j && lordLocation.y == i)
                        cells[i][j] = (k, -1, GameConstants.LordHP);
                }
            }
        }

        bool[] defeated = new bool[NumPlayer];
        for (int i = 0; i < NumPlayer; i++)
        {
            defeated[i] = false;
        }

        gameState = new GameState(1, cells, players.Select(p => p.GetComponent<Player>().Data).ToArray(), defeated);
    }

    private void Start()
    {
        // Init here
        NumPlayer = 2;

        battleHandler.StartGameLoop();
    }

    public void ChangeTurn()
    {
        gameState.Turn += 1;
        int currentPlayer = battleHandler.GetCurrentPlayer();
        gameState.ChangeTurn(currentPlayer, GameConstants.EnergyPerTurn);
    }

    public int GetNextPlayer(int currentPlayer)
    {
        return battleHandler.GetNextPlayer(currentPlayer);
    }


    public void ApplyMove(Move move)
    {
        if (move.Type == MoveType.CharMove)
            ApplyCharMove(move);
        else if (move.Type == MoveType.CharAttack)
            ApplyCharAttack(move);
        else if (move.Type == MoveType.Spawn)
            ApplySpawn(move);
        else;
        gameState.ApplyMove(move);
    }

    public void ApplyCharMove(Move move)
    {
        Vector3 start = battleHandler.Spawner.GetWorldPosition(move.Source);
        Vector3 destination = battleHandler.Spawner.GetWorldPosition(move.Target);
        int player = gameState.Cells[move.Source.y][move.Source.x].Item1; // Get the player that does the move
        int charIndex = gameState.Cells[move.Source.y][move.Source.x].Item2;
        GameObject character = players[player].GetComponent<Player>().characters[charIndex]; // Get the character that needs to move
        bool direction = destination.x > start.x;
        character.GetComponent<Character>().CharMove(direction);
        character.transform.position = destination;
        // TODO: add moving anim
    }

    public void ApplyCharAttack(Move move)
    {
        Vector3 start = battleHandler.Spawner.GetWorldPosition(move.Source);
        Vector3 destination = battleHandler.Spawner.GetWorldPosition(move.Target);
        int player = gameState.Cells[move.Source.y][move.Source.x].Item1; // Get the player that does the attack
        int charIndex = gameState.Cells[move.Source.y][move.Source.x].Item2;
        GameObject character = players[player].GetComponent<Player>().characters[charIndex]; // Get the character that needs to attack
        bool direction = destination.x > start.x;
        character.GetComponent<Character>().CharAttack(direction);

        int targetPlayer = gameState.Cells[move.Target.y][move.Target.x].Item1; // Get the player that receives the attack
        int targetIndex = gameState.Cells[move.Target.y][move.Target.x].Item2;
        if (targetIndex != -1)
        {
            GameObject targetCharacter = players[targetPlayer].GetComponent<Player>().characters[targetIndex]; // Get the character that receives the attack
            if (targetCharacter.GetComponent<Character>().Data.CurrentHP <= character.GetComponent<Character>().Data.characterStats.damage)
            {
                targetCharacter.GetComponent<SpriteRenderer>().enabled = false;
            }
        }
        // TODO: add attacking anim
    }

    public void ApplySpawn(Move move)
    {
        Vector3 destination = battleHandler.Spawner.GetWorldPosition(move.Target); // The position to spawn the character
        int player = move.Source.x; // Get the player that does the spawn
        int charIndex = move.Source.y;
        GameObject character = players[player].GetComponent<Player>().characters[charIndex]; // Get the character that needs to spawn
        character.transform.position = destination;
        character.GetComponent<SpriteRenderer>().enabled = true;
        // TODO: add spawn anim
    }


}
