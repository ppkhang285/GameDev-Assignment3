using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class BattleHandler : MonoBehaviour
{
    public BoardSpawner Spawner;
    private enum GameplayState { Start, Playing, GameOver };
    private enum PlayerTurn { Player1Turn, Player2Turn, Player3Turn, Player4Turn }

    private GameplayState currentState;
    private PlayerTurn[] playerPool;
    private PlayerTurn currentPlayer;

    //Game Object for menu
    public GameObject charDmg;
    public GameObject charHP;
    public GameObject charAttackRange;
    public GameObject charMovementRange;

    public Button endTurnButton;
    private bool turnEndRequested = false;

    private void Awake()
    {
        // Set up the button listener
        if (endTurnButton != null)
        {
            endTurnButton.onClick.AddListener(OnEndTurnButtonClick);
        }
        else
        {
            Debug.LogError("End Turn Button not assigned to BattleHandler!");
        }
    }

    private void OnDestroy()
    {
        // Clean up the button listener
        if (endTurnButton != null)
        {
            endTurnButton.onClick.RemoveListener(OnEndTurnButtonClick);
        }
    }

    private void OnEndTurnButtonClick()
    {
        turnEndRequested = true;
    }

    public void StartGameLoop()
    {
        Setup();
        StartCoroutine(GameLoop());
    }

    private void Setup()
    {
        int playerNum = GameplayManager.ExtNumberPlayer;
        Spawner.SpawnBoard();

        // Initialize playerPool based on playerNum
        playerPool = new PlayerTurn[playerNum];
        for (int i = 0; i < playerNum; i++)
        {
            playerPool[i] = (PlayerTurn)i;
        }

        currentState = GameplayState.Start;
        currentPlayer = playerPool[0]; // Start with Player 1
    }

    private IEnumerator GameLoop()
    {
        while (currentState != GameplayState.GameOver)
        {
            if (currentState == GameplayState.Start)
            {
               
                currentState = GameplayState.Playing;
                yield return null;
            }

            //Debug.Log("Current Player: " + currentPlayer);

            // Handle the current player's turn
            PlayerType playerType = GameplayManager.Instance.players[System.Array.IndexOf(playerPool, currentPlayer)].GetComponent<Player>().Data.Type;
            if (playerType == PlayerType.Human) 
            {
                if (endTurnButton != null)
                    endTurnButton.gameObject.SetActive(true);

                yield return StartCoroutine(HandlePlayerTurn());

                // Disable the end turn button after the turn
                if (endTurnButton != null)
                    endTurnButton.gameObject.SetActive(false);
            }
            else
                yield return StartCoroutine(HandleAITurn());

            // After the player's turn, move to the next player
            ChangeTurn();
        }
        if (!GameplayManager.isPVP)
        {
            if (GameplayManager.Instance.gameState.Defeated[0])
            {
                if (GameplayManager.ExtNumberPlayer == 2)
                    GameplayManager.Instance.SelectedGameOverCanvas = "Defeat Scene";
                else if (GameplayManager.ExtNumberPlayer == 3)
                    GameplayManager.Instance.SelectedGameOverCanvas = "Defeat Scene 1 vs 1 vs 1";
                else
                    GameplayManager.Instance.SelectedGameOverCanvas = "Defeat Scene 1 vs 1 vs 1 vs 1";
            }
            else
            {
                if (GameplayManager.ExtNumberPlayer == 2)
                    GameplayManager.Instance.SelectedGameOverCanvas = "Victory Scene 1 vs 1";
                else if (GameplayManager.ExtNumberPlayer == 3)
                    GameplayManager.Instance.SelectedGameOverCanvas = "Victory Scene 1 vs 1 vs 1";
                else
                    GameplayManager.Instance.SelectedGameOverCanvas = "Victory Scene 1 vs 1 vs 1 vs 1";
            }
        } 

        SceneManager.LoadScene("GameOverMenu");
    }

    private IEnumerator HandleAITurn()
    {
        Debug.Log(currentPlayer + " is taking their turn.");
        List<Move> sequence = GameplayManager.Instance.AI.GetMove(GameplayManager.Instance.gameState, System.Array.IndexOf(playerPool, currentPlayer));
        foreach (Move move in sequence)
        {
            // Debug.Log(move.ToString());
            GameplayManager.Instance.ApplyMove(move);
        }

        yield return null;
    }

    private IEnumerator HandlePlayerTurn()
    {
        // Simulate waiting for the player's actions (replace with real input logic)
        Debug.Log(currentPlayer + " is taking their turn.");

        PlayerData currentPlayerData = GameplayManager.Instance.gameState.Players[GetCurrentPlayer()];
        Vector2Int? sellectedCell= null; // Set nulllabe state
        Vector2Int? spawnCell = null;
        int AP = currentPlayerData.AP;

        turnEndRequested = false;

        while (!turnEndRequested)
        {
            for (int keyval = 0; keyval <= 9; keyval++){
                KeyCode key = keyval + KeyCode.Alpha0;
                if (Input.GetKey(key)){
                    if (spawnCell != null)
                    {
                        int charCost = currentPlayerData.Characters[keyval].characterStats.cost;
                        if (currentPlayerData.Energy < charCost)
                        {
                            Debug.Log("Illegal spawn: Not enough energy or character");
                        }
                        else if (currentPlayerData.Characters[keyval].Spawned)
                        {
                            Debug.Log("Illegal spawn: Character already spawned");
                        }
                        else
                        {
                            Vector2Int prevCell = spawnCell.Value;
                            spawnCell = null;
                            Move NewMove = new Move(new Vector2Int(GetCurrentPlayer(), keyval), prevCell, MoveType.Spawn);
                            GameplayManager.Instance.ApplyMove(NewMove);
                        }
                    }
                    
                }
            }

            if (Input.GetMouseButtonDown(0)) // Left mouse click
            {
                int curPlayer = GetCurrentPlayer();
                Vector2Int clickedCell = Spawner.GetClickedCell();
                if (clickedCell.x == -1 || clickedCell.y == -1) yield break;
                (int,int,int) cell = GameplayManager.Instance.gameState.Cells[clickedCell.y][clickedCell.x];
                if (cell.Item1 != -1 ){
                    // None-empty cell:
                }
                if (sellectedCell == null && spawnCell == null){
                    // Handle select own unit
                    if (cell.Item1 == curPlayer){
                        if (cell.Item2 == -1){
                            // TODO: display Lord stats
                            SetCharacterInfo(GameConstants.LordDmg, GameConstants.LordHP, -1, 0);
                            DisplayCharaterInfo(true);
                        }
                        else {
                            sellectedCell = clickedCell;
                            CharacterData data = GameplayManager.Instance.gameState.Players[cell.Item1].Characters[cell.Item2];
                            CharacterStats stats = data.characterStats; // all the base stats are in here, display them
                            SetCharacterInfo(stats.hp, stats.damage, stats.attackRange, stats.movementRange);
                            DisplayCharaterInfo(true);

                        }
                    } else if (cell.Item1 == -1){
                        DisplayCharaterInfo(false);
                        foreach ( Vector2Int spawnLocation in currentPlayerData.SpawnLocations){
                            if (clickedCell == spawnLocation){
                                spawnCell = clickedCell;

                                // TODO: popup for choosing character to spawn

                            }
                        }
                    } else {
                        // Debug.Log("Illegal action: Select enemy unit");
                        DisplayCharaterInfo(false);
                    }
                } else if (sellectedCell.HasValue) {
                    Vector2Int prevCell= sellectedCell.Value;
                    int mappedChar = GameplayManager.Instance.gameState.Cells[prevCell.y][prevCell.x].Item2;
                    sellectedCell = null; 
                    CharacterData CharData = GameplayManager.Instance.gameState.Players[curPlayer].Characters[mappedChar];
                    if (cell.Item1 == -1){
                        DisplayCharaterInfo(false);
                        int Distannce= Utils.ManhattanDistance(prevCell, clickedCell);
                        if (Distannce > CharData.characterStats.movementRange){
                        } else {
                            if (CharData.AP > 0 && AP > 0)
                            {
                                Move NewMove = new Move(prevCell, clickedCell, MoveType.CharMove);
                                GameplayManager.Instance.ApplyMove(NewMove);
                                AP -= 1;
                            }
                        }
                    } else if (cell.Item1 != curPlayer){
                        DisplayCharaterInfo(false);
                        int Distannce= Utils.ManhattanDistance(prevCell, clickedCell);
                        if (Distannce > CharData.characterStats.attackRange){
                            Debug.Log("Illegal attack: Out of range");
                        } else {
                            if (CharData.AP > 0 && AP > 0)
                            {
                                Move NewMove = new Move(prevCell, clickedCell, MoveType.CharAttack);
                                GameplayManager.Instance.ApplyMove(NewMove);
                                AP -= 1;
                            }
                        }
                    } else {
                        sellectedCell = clickedCell;
                        CharacterData data = GameplayManager.Instance.gameState.Players[cell.Item1].Characters[cell.Item2];
                        CharacterStats stats = data.characterStats; // all the base stats are in here, display them
                        SetCharacterInfo(stats.hp, stats.damage, stats.attackRange, stats.movementRange);
                        DisplayCharaterInfo(true);
                    }
                }
 
            }

            // Wait until the next frame to check again
            
            yield return null;
        }
        
    }

    public int GetNextPlayer(int currentIdx)
    {
        bool[] defeated = GameplayManager.Instance.gameState.Defeated;
        for (int i = 1; i < playerPool.Length; i++)
        {
            if (!defeated[(currentIdx + i) % playerPool.Length]) return (currentIdx + i) % playerPool.Length;
        }
        return currentIdx;
    }

    public int GetCurrentPlayer()
    {
        return System.Array.IndexOf(playerPool, currentPlayer);
    }

    private void ChangeTurn()
    {
        GameplayManager.Instance.ChangeTurn();
        int currentIdx = GetCurrentPlayer();
        int nextIdx = GetNextPlayer(currentIdx);
        if (nextIdx == currentIdx)
        {
            Debug.Log("Player " + nextIdx.ToString() + " has won");
            currentState = GameplayState.GameOver;
            return;
        }
        currentPlayer = playerPool[nextIdx];
    }

    private void DisplayCharaterInfo(bool display)
    {
        charDmg.SetActive(display);
        charHP.SetActive(display);
        charAttackRange.SetActive(display);
        charMovementRange.SetActive(display);
    }

    private void SetCharacterInfo(int hp, int damage, int attackRange, int movementRange)
    {
        charDmg.GetComponent<Text>().text = "Damage: " + damage;
        charHP.GetComponent<Text>().text = "HP: " + hp;
        charAttackRange.GetComponent<Text>().text = "Attack Range: " + attackRange;
        charMovementRange.GetComponent<Text>().text = "Movement Range: " + movementRange;
    }
}
