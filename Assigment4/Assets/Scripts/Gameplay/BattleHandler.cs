using System.Collections;
using System.Collections.Generic;
using UnityEngine;
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
    public GameObject playerName;
    public GameObject playerHP;
    public GameObject playerAttackRange;
    public GameObject playerMovementRange;

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
        int playerNum = GameplayManager.Instance.NumPlayer;
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
            // Handling spawn action (require valid spawn key to unlock)
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
                            // Clear buffered click
                            Vector2Int prevCell = spawnCell.Value;
                            spawnCell = null;
                            Move NewMove = new Move(new Vector2Int(GetCurrentPlayer(), keyval), prevCell, MoveType.Spawn);
                            // Debug.Log("Spawn action: " + NewMove.ToString());
                            GameplayManager.Instance.ApplyMove(NewMove);
                        }
                    }
                    
                }
            }

            if (Input.GetMouseButtonDown(0)) // Left mouse click
            {
                int curPlayer = GetCurrentPlayer();
                Vector2Int clickedCell = Spawner.GetClickedCell();
                if (clickedCell.x == -1 || clickedCell.y == -1) yield break; // Clicked outside the board will cancel everything
                // Debug.Log($"Clicked cell: Row {clickedCell.y}, Column {clickedCell.x}");
                (int,int,int) cell = GameplayManager.Instance.gameState.Cells[clickedCell.y][clickedCell.x];
                if (cell.Item1 != -1 ){
                    // None-empty cell:
                }
                if (sellectedCell == null && spawnCell == null){
                    // Handle select own unit
                    if (cell.Item1 == curPlayer){
                        if (cell.Item2 == -1){
                            // Debug.Log("Selected Lord at: Row " + clickedCell.y + ", Column " + clickedCell.x);
                            // TODO: display Lord stats
                            
                        }
                        else {
                            sellectedCell = clickedCell;
                            // Debug.Log($"Selected unit at: Row {sellectedCell.Value.y}, Column {sellectedCell.Value.x}");

                            // TODO: display unit stats
                            CharacterData data = GameplayManager.Instance.gameState.Players[cell.Item1].Characters[cell.Item2];
                            CharacterStats stats = data.characterStats; // all the base stats are in here, display them
                            playerName.GetComponent<Text>().text = stats.name;
                            playerHP.GetComponent<Text>().text = "HP: " + stats.hp;
                            playerAttackRange.GetComponent<Text>().text = "Attack Range: " + stats.attackRange;
                            playerMovementRange.GetComponent<Text>().text = "Movement Range: " + stats.movementRange;

                        }
                    } else if (cell.Item1 == -1){
                        // Handle spawn action
                        foreach( Vector2Int spawnLocation in currentPlayerData.SpawnLocations){
                            if (clickedCell == spawnLocation){
                                spawnCell = clickedCell;
                                // Debug.Log($"Selected spawn location at: Row {spawnCell.Value.y}, Column {spawnCell.Value.x}");

                                // TODO: popup for choosing character to spawn
                                playerName.SetActive(true);
                                playerHP.SetActive(true);
                                playerAttackRange.SetActive(true);
                                playerMovementRange.SetActive(true);
                            }
                        }
                    } else {
                        // Debug.Log("Illegal action: Select enemy unit");
                    }
                } else if (sellectedCell.HasValue) {
                    // Clear buffered click
                    Vector2Int prevCell= sellectedCell.Value;
                    int mappedChar = GameplayManager.Instance.gameState.Cells[prevCell.y][prevCell.x].Item2;
                    sellectedCell = null; 
                    CharacterData CharData = GameplayManager.Instance.gameState.Players[curPlayer].Characters[mappedChar];
                    if (cell.Item1 == -1){
                        // Move action
                        int Distannce= Utils.ManhattanDistance(prevCell, clickedCell);
                        if (Distannce > CharData.characterStats.movementRange){
                            // Debug.Log("Illegal move: Out of range");
                        } else {
                            if (CharData.AP > 0 && AP > 0)
                            {
                                Move NewMove = new Move(prevCell, clickedCell, MoveType.CharMove);
                                // Debug.Log("Move action: " + NewMove.ToString());
                                GameplayManager.Instance.ApplyMove(NewMove);
                                AP -= 1;
                            } else
                            {
                                // Debug.Log("Out of action points");
                            }
                        }
                    } else if (cell.Item1 != curPlayer){
                        // Attack action
                        int Distannce= Utils.ManhattanDistance(prevCell, clickedCell);
                        if (Distannce > CharData.characterStats.attackRange){
                            Debug.Log("Illegal attack: Out of range");
                        } else {
                            if (CharData.AP > 0 && AP > 0)
                            {
                                Move NewMove = new Move(prevCell, clickedCell, MoveType.CharAttack);
                                // Debug.Log("Attack action: " + NewMove.ToString());
                                GameplayManager.Instance.ApplyMove(NewMove);
                                AP -= 1;
                            } else
                            {
                                // Debug.Log("Out of action points");
                            }
                        }
                    } else {
                        // Select another unit 
                        sellectedCell = clickedCell;
                        // Debug.Log($"Selected another unit at: Row {sellectedCell.Value.y}, Column {sellectedCell.Value.x}");

                        // TODO: display unit info
                        CharacterData data = GameplayManager.Instance.gameState.Players[cell.Item1].Characters[cell.Item2];
                        CharacterStats stats = data.characterStats; // all the base stats are in here, display them
                        playerName.GetComponent<Text>().text = stats.name;
                        playerHP.GetComponent<Text>().text = "HP: " + stats.hp;
                        playerAttackRange.GetComponent<Text>().text = "Attack Range: " + stats.attackRange;
                        playerMovementRange.GetComponent<Text>().text = "Movement Range: " + stats.movementRange;
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
}
