using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class Card : MonoBehaviour
{
    public int cardValue; // Store the card's value (e.g., 1 to 10)
    public Button cardButton;
    public TextMeshProUGUI cardText; // Reference to the TextMeshPro component
    public CardManager cardManager;
    void Start()
    {
        // Ensure the button has an OnClick listener linked to this script
        if (cardButton != null)
        {
            cardButton.onClick.AddListener(OnCardClick);
        }
    }
    // You can add additional properties like card suit or other info here

    // This method can be used to update the card's visual representation (e.g., text)
    public void SetCardValue(int value)
    {
        cardValue = value;
        if (cardText != null)
        {
            cardText.text = value.ToString(); // Set the card's value on the TextMeshPro component
        }
    }
    public void OnCardClick()
    {
        if (cardManager != null)
        {
            cardManager.OnCardUsed(cardValue); // Notify the CardManager
        }
        else
        {
            Debug.LogError("CardManager reference is missing!");
        }
    }
}
