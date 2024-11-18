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
    public static bool isPVP {get; set;}
    public static int ExtNumberPlayer {get; set;}
    public static int ExtChosenDeck {get; set;}
    

    public static string ExtLevel; 
    public int DeckSize { get; private set; }
    public int BoardSize { get; private set; }
    public GameObject[] players;
    public GameState gameState;
    //private bool[] defeated;
    // public const int MyPlayer = 0;
    public BaseAI AI;
    public string SelectedGameOverCanvas;

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

        players = new GameObject[ExtNumberPlayer];
        Vector2Int[] locations = new Vector2Int[ExtNumberPlayer];

        SelectedGameOverCanvas = "";
        if (ExtNumberPlayer < 2 || ExtNumberPlayer > 4)
        {
            throw new System.Exception("Invalid number of players");
        } else {
            locations[0] = new Vector2Int(0, 0);
            if (ExtNumberPlayer == 2)
            {
                locations[1] = new Vector2Int(BoardSize - 1, BoardSize - 1);
            } else if (ExtNumberPlayer == 3)
            {
                locations[1] = new Vector2Int(BoardSize - 1, 0);
                locations[2] = new Vector2Int(0, BoardSize - 1);
            } else
            {
                locations[1] = new Vector2Int(BoardSize - 1, 0);
                locations[2] = new Vector2Int(BoardSize - 1, BoardSize - 1);
                locations[3] = new Vector2Int(0, BoardSize - 1);
            }
        }
        if (!isPVP)
        {
            for (int i = 0; i < ExtNumberPlayer; i++)
            {
                GameObject prefab = Resources.Load<GameObject>("Lords/Lord_" + i.ToString());
                players[i] = Instantiate(prefab);
                Player player = players[i].GetComponent<Player>();
                if (i == 0)
                    player.Initialize(i, ExtChosenDeck, PlayerType.Human, locations[i]); // TODO: initialize player with correct deck and correct type
                else
                {
                    bool random = UnityEngine.Random.value >= 0.5;
                    if (random)
                        player.Initialize(i, 0 , PlayerType.AI, locations[i]);
                    else
                        player.Initialize(i, 1, PlayerType.AI, locations[i]);
                }
                prefab.transform.position = battleHandler.Spawner.GetWorldPosition(locations[i]);
            }
        }


        if (ExtLevel == "Easy")
            AI = new RandomAI(100, 1.0f);
        if (ExtLevel == "Normal")
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

        bool[] defeated = new bool[ExtNumberPlayer];
        for (int i = 0; i < ExtNumberPlayer; i++)
        {
            defeated[i] = false;
        }

        gameState = new GameState(1, cells, players.Select(p => p.GetComponent<Player>().Data).ToArray(), defeated);
    }

    private void Start()
    {
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
        character.GetComponent<Character>().Data.AP -= 1;
        character.GetComponent<Character>().CharMove(direction);
        character.GetComponent<Character>().Data.AP += 1;
        character.transform.position = destination;

    }

    public void ApplyCharAttack(Move move)
    {
        Vector3 start = battleHandler.Spawner.GetWorldPosition(move.Source);
        Vector3 destination = battleHandler.Spawner.GetWorldPosition(move.Target);
        int player = gameState.Cells[move.Source.y][move.Source.x].Item1; // Get the player that does the attack
        int charIndex = gameState.Cells[move.Source.y][move.Source.x].Item2;
        GameObject character = players[player].GetComponent<Player>().characters[charIndex]; // Get the character that needs to attack
        bool direction = destination.x > start.x;
        character.GetComponent<Character>().Data.AP -= 1;
        character.GetComponent<Character>().CharAttack(direction);
        character.GetComponent<Character>().Data.AP += 1;

        int targetPlayer = gameState.Cells[move.Target.y][move.Target.x].Item1; // Get the player that receives the attack
        int targetIndex = gameState.Cells[move.Target.y][move.Target.x].Item2;
        if (targetIndex != -1)
        {
            GameObject targetCharacter = players[targetPlayer].GetComponent<Player>().characters[targetIndex]; // Get the character that receives the attack
            if (targetCharacter.GetComponent<Character>().Data.CurrentHP <= character.GetComponent<Character>().Data.characterStats.damage)
            {
                targetCharacter.GetComponent<SpriteRenderer>().enabled = false;
                character.GetComponent<Character>().bar.bar.SetActive(false);
            } else
            {
                targetCharacter.GetComponent<Character>().Data.CurrentHP -= character.GetComponent<Character>().Data.characterStats.damage;
                targetCharacter.GetComponent<Character>().TakeDmg();
                targetCharacter.GetComponent<Character>().Data.CurrentHP += character.GetComponent<Character>().Data.characterStats.damage;
            }
        }
    }

    public void ApplySpawn(Move move)
    {
        Vector3 destination = battleHandler.Spawner.GetWorldPosition(move.Target); // The position to spawn the character
        int player = move.Source.x; // Get the player that does the spawn
        int charIndex = move.Source.y;
        GameObject character = players[player].GetComponent<Player>().characters[charIndex]; // Get the character that needs to spawn
        character.transform.position = destination;
        character.GetComponent<SpriteRenderer>().enabled = true;
        character.GetComponent<Character>().bar.bar.SetActive(true);
    }


}
