using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class NetworkBattleHandler : BattleHandler
{
    public bool IsLocalPlayerTurn()
    {
        int currentIdx = GetCurrentPlayer();
        int localPlayerIdx = PhotonNetwork.LocalPlayer.ActorNumber - 1; // Assuming the actor number matches player order
        // Debug.LogError("localplayerIdx:"+localPlayerIdx+"\n"+"currentidx:"+currentIdx);

        return currentIdx == localPlayerIdx;
    }
    public override void StartGameLoop()
    {
        endTurnButton.gameObject.SetActive(false);
        Setup();
        // if (PhotonNetwork.IsMasterClient) {
        StartCoroutine(GameLoop());
        Debug.LogError("start game loop");
 
    }
    protected override  void OnEndTurnButtonClick()
    {
        Debug.LogError("i call end turn");
        turnEndRequested=true;
        photonView.RPC("RPC_EndTurn",RpcTarget.Others);
    }
    protected override IEnumerator GameLoop()
    {
        Debug.LogError("gameloop started");

        while (currentState != GameplayState.GameOver)
        {
            if (currentState == GameplayState.Start)
            {
                currentState = GameplayState.Playing;
                yield return null;
            }

            Debug.LogError("Processing turn for player index: " + GetCurrentPlayer());

            if (IsLocalPlayerTurn())
            {
                Debug.LogError("Local player turn started");

                if (endTurnButton != null)
                    endTurnButton.gameObject.SetActive(true);

                yield return StartCoroutine(HandlePlayerTurn());

                if (endTurnButton != null)
                    endTurnButton.gameObject.SetActive(false);

            }
            else if (PhotonNetwork.IsMasterClient)
            {
                // Wait for the other player to finish their turn
                while (!turnEndRequested)
                {
                    yield return null;
                }
            }else
            {
                yield return null;
            }

            if (PhotonNetwork.IsMasterClient){
                Debug.LogError("master client call change turn");
                ChangeTurn();
            }
        }

        Debug.LogError("Game Over!");
    }
    [PunRPC]
    public void RPC_EndTurn() {
        Debug.LogError("i receive end turn.");
        turnEndRequested = true;
    }
    protected override void ChangeTurn()
    {
        base.ChangeTurn(); // Call the base method to switch turns

        // Notify other players that it's the next player's turn
        Debug.LogError("new player turn is: "+currentPlayer.ToString());
        photonView.RPC("RPC_UpdateTurn", RpcTarget.Others, currentPlayer.ToString());
    }
    [PunRPC]
    private void RPC_UpdateTurn(string playerTurn)
    {
        currentPlayer = (PlayerTurn)System.Enum.Parse(typeof(PlayerTurn), playerTurn);
    }

    
    // Handle spawn action and move actions from the network perspective
    protected override  IEnumerator HandlePlayerTurn()
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
                            photonView.RPC("RPC_SpawnCharacter", RpcTarget.All, GetCurrentPlayer(), keyval, prevCell.x, prevCell.y);
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
                            
                        }
                    } else if (cell.Item1 == -1){
                        // Handle spawn action
                        foreach( Vector2Int spawnLocation in currentPlayerData.SpawnLocations){
                            if (clickedCell == spawnLocation){
                                spawnCell = clickedCell;
                                // Debug.Log($"Selected spawn location at: Row {spawnCell.Value.y}, Column {spawnCell.Value.x}");

                                // TODO: popup for choosing character to spawn
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
                                
                                // Debug.Log("Move action: " + NewMove.ToString());
                                photonView.RPC("RPC_MoveCharacter", RpcTarget.All, GetCurrentPlayer(), prevCell.x,prevCell.y, clickedCell.x, clickedCell.y);
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
                                photonView.RPC("RPC_CharAttack", RpcTarget.All, GetCurrentPlayer(), prevCell.x,prevCell.y, clickedCell.x, clickedCell.y);

                                // Debug.Log("Attack action: " + NewMove.ToString());
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
                    }
                }
 
            }

            // Wait until the next frame to check again
            
            yield return null;
        }
        
    }
    [PunRPC]

    public void RPC_SpawnCharacter(int playerIdx, int characterIdx, int spawnX, int spawnY)
    {
        // Convert the x and y values back to Vector2Int
        Vector2Int spawnLocation = new Vector2Int(spawnX, spawnY);
        Move NewMove = new Move(new Vector2Int(GetCurrentPlayer(), characterIdx), spawnLocation, MoveType.Spawn);
        
        GameplayManager.Instance.ApplyMove(NewMove);
        // You may also want to add some visual effect here to show the character has spawned

        Debug.Log($"Player {playerIdx} spawned character {characterIdx} at {spawnLocation}");
    }
    [PunRPC]

    public void RPC_MoveCharacter(int playerIdx, int preX,int preY, int clickX, int clickY)
    {
        // Convert the x and y values back to Vector2Int
        Move NewMove = new Move(new Vector2Int(preX,preY), new Vector2Int(clickX,clickY), MoveType.CharMove);
        
        GameplayManager.Instance.ApplyMove(NewMove);
        // You may also want to add some visual effect here to show the character has spawned

        Debug.Log($"Player {playerIdx} move characte");
    }
    public void RPC_CharAttack(int playerIdx, int preX,int preY, int clickX, int clickY)
    {
        // Convert the x and y values back to Vector2Int
        Move NewMove = new Move(new Vector2Int(preX,preY), new Vector2Int(clickX,clickY), MoveType.CharMove);
        
        GameplayManager.Instance.ApplyMove(NewMove);
        // You may also want to add some visual effect here to show the character has spawned

        Debug.Log($"Player {playerIdx} attack characte");
    }


}
