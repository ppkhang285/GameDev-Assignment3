using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;

public class CardManager : MonoBehaviourPunCallbacks
{
    public GameObject cardPrefab; // Prefab for the card
    public Transform handPosition; // Single position for all players' cards
    public int cardsPerPlayer = 5; // Number of cards to deal per player

    private List<int> deck = new List<int>(); // List to represent the deck of cards

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            InitializeDeck();
            ShuffleDeck();
            DealCards();
        }
    }

    void InitializeDeck()
    {
        for (int value = 1; value <= 10; value++)
        {
            deck.Add(value); // Add cards with values 1 to 10
        }
    }

    void ShuffleDeck()
    {
        for (int i = 0; i < deck.Count; i++)
        {
            int randomIndex = Random.Range(0, deck.Count);
            int temp = deck[i];
            deck[i] = deck[randomIndex];
            deck[randomIndex] = temp;
        }
    }

    void DealCards()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            for (int i = 0; i < cardsPerPlayer; i++)
            {
                if (deck.Count > 0)
                {
                    int card = deck[0];
                    deck.RemoveAt(0);
                    photonView.RPC("AssignCardToPlayer", RpcTarget.All, player.ActorNumber, card);
                }
            }
        }
    }

    [PunRPC]
    void AssignCardToPlayer(int playerID, int card)
    {
        if (PhotonNetwork.LocalPlayer.ActorNumber == playerID)
        {
            // Only instantiate the card for the local player
            GameObject newCard = Instantiate(cardPrefab, handPosition.position, Quaternion.identity);

            // Set the card's value
            Card cardScript = newCard.GetComponent<Card>();
            if (cardScript != null)
            {
                cardScript.SetCardValue(card);
                cardScript.cardManager=this;
            }

            // Ensure the card is visible only to the local player
            PhotonView photonView = newCard.GetComponent<PhotonView>();
            if (photonView != null)
            {
                // Set the visibility to only be seen by the local player
                photonView.TransferOwnership(PhotonNetwork.LocalPlayer);
            }

            Debug.Log($"Player {playerID} received card: {card}");
        }
        // No card instantiation for other players
    }
    public void OnCardUsed(int cardValue)
    {
        Debug.Log($"Player {PhotonNetwork.LocalPlayer.ActorNumber} used card: {cardValue}");

        // Send the card usage event to the Master Client
        photonView.RPC("NotifyMasterCardUsed", RpcTarget.MasterClient, PhotonNetwork.LocalPlayer.ActorNumber, cardValue);
    }
    [PunRPC]
    void NotifyMasterCardUsed(int playerID, int cardValue)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            Debug.Log($"Master Client received card usage from Player {playerID}: {cardValue}");

            // Process the card effect on the Master Client
            ProcessCardEffect(playerID, cardValue);

            // Broadcast the card usage to all players
            photonView.RPC("BroadcastCardUsage", RpcTarget.All, playerID, cardValue);
        }
    }
    [PunRPC]
    void BroadcastCardUsage(int playerID, int cardValue)
    {
        Debug.Log($"Broadcasting card usage: Player {playerID} used card {cardValue}");

        // Apply the card effect for everyone (UI updates, animations, etc.)
        HandleCardEffect(playerID, cardValue);
    }
    private void ProcessCardEffect(int playerID, int cardValue)
    {
        // Example: Master Client validates or modifies the effect
        Debug.Log($"Processing effect of card {cardValue} used by Player {playerID}");

        // TODO: Implement game logic here
    }
    private void HandleCardEffect(int playerID, int cardValue)
    {
        // Example: Display the card usage or trigger an animation
        Debug.Log($"Player {playerID}'s card effect applied locally for card {cardValue}");

        // TODO: Add visual or gameplay changes here
    }

}
