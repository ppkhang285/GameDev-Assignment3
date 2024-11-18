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
        int localPlayerIdx = PhotonNetwork.LocalPlayer.ActorNumber - 1 ; // Assuming the actor number matches player order
        Debug.LogError("localplayerIdx:"+localPlayerIdx+"\n"+"currentidx:"+currentIdx);

        return currentIdx == localPlayerIdx;
    }
    public override void StartGameLoop()
    {
        Setup();
        // if (PhotonNetwork.IsMasterClient) {
            StartCoroutine(GameLoop());
            Debug.LogError("start game loop for masterclient");
        // }
    }
    protected override  void OnEndTurnButtonClick()
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("RPC_EndTurn", RpcTarget.MasterClient);
        }
        else{
            turnEndRequested=true;
        }
    }
    protected override IEnumerator GameLoop()
    {
        Debug.LogError("gameloop started on masterclient");

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
                turnEndRequested = false;
            }else
            {
                yield return null;
            }

            // Only master client changes turns
            if (PhotonNetwork.IsMasterClient)
            {
                ChangeTurn();
            }
        }

        Debug.LogError("Game Over!");
    }
    [PunRPC]
    public void RPC_EndTurn() {
        Debug.LogError("Turn end requested by a client.");
        turnEndRequested = true;
    }
    protected override void ChangeTurn()
    {
        base.ChangeTurn(); // Call the base method to switch turns

        // Notify other players that it's the next player's turn
        photonView.RPC("RPC_UpdateTurn", RpcTarget.AllBuffered, currentPlayer.ToString());
    }
    [PunRPC]
    private void RPC_UpdateTurn(string playerTurn)
    {
        currentPlayer = (PlayerTurn)System.Enum.Parse(typeof(PlayerTurn), playerTurn);
    }

    
    // Handle spawn action and move actions from the network perspective
    protected override IEnumerator HandlePlayerTurn()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            // Allow the current player to act.
            Debug.Log($"{currentPlayer} is taking their turn");

            // Add network-related logic to sync actions
            yield return base.HandlePlayerTurn();
        }
        else
        {
            // If not the master client, wait for the turn to be finalized by master client
            yield return null;
        }
    }

    // Override to handle network initialization (on player joined, etc.)

}
