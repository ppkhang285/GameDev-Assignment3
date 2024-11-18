using UnityEngine;

public class GameOverManager : MonoBehaviour
{
    public GameObject[] canvases; // Assign all canvases via the Inspector

    void Start()
    {
        // Get the selected canvas name from the GameManager
        string selectedCanvas = GameplayManager.Instance.SelectedGameOverCanvas;

        // Activate the correct canvas
        foreach (GameObject canvas in canvases)
        {
            canvas.SetActive(canvas.name == selectedCanvas);
        }
    }
}
