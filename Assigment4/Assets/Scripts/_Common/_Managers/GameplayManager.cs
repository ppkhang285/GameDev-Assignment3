using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameplayManager : MonoBehaviour

{
    public BattleHandler battleHandler;
    public static GameplayManager Instance { get; private set; }

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
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>("Assets/Prefabs/Lords/Lord_" + i.ToString() + ".prefab");
            players[i] = Instantiate(prefab);
            Player player = players[i].GetComponent<Player>();
            player.Initialize(i, i, PlayerType.AI, locations[i]);
        }

        string level = "Normal";
        if (level == "Easy")
            AI = new RandomAI(100, 0.0f);
        if (level == "Normal")
            AI = new MinimaxAI(5, 30, 0.0f, 30.0f);

        (int, int, int)[][] cells = new (int, int, int)[BoardSize][];
        for (int i = 0; i < BoardSize; i++)
        {
            cells[i] = new (int, int, int)[BoardSize];
            for (int j = 0; j < BoardSize; j++)
            {
                for (int k = 0; k < players.Length; k++)
                {
                    Player pl = players[k].GetComponent<Player>();
                    Vector2Int lordLocation = pl.Location;
                    if (lordLocation.x == i && lordLocation.y == j)
                        cells[i][j] = (k, -1, pl.Data.LordHP);
                    else
                        cells[i][j] = (-1, -1, 0);
                }
            }
        }

        PlayerData[] data = new PlayerData[NumPlayer];
        for (int i = 0; i < NumPlayer; i++)
        {
            data[i] = players[i].GetComponent<Player>().Data;
        }

        gameState = new GameState(1, cells, data);
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
        if ((gameState.Turn - 1 - currentPlayer) / NumPlayer < GameConstants.TurnReceiveEnergy) 
        {
            PlayerData data = players[currentPlayer].GetComponent<Player>().Data;
            data.AP += GameConstants.EnergyPerTurn;
        }

    }

    public int GetNextPlayer(int currentPlayer)
    {
        return battleHandler.GetNextPlayer(currentPlayer);
    }
    
    public void ApplyMoveSequence(List<Move> moves) // Actually change the current game state
    {
        foreach (Move move in moves)
        {
            gameState.ApplyMove(move);
        }
    }


    
}
