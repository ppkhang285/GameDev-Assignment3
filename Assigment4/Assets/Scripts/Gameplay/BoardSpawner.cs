using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BoardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private GameObject field;

    private const int FIELD_SIZE = GameConstants.BoardSize;
    private Vector2 panelSize;
    private Vector2 fieldOrigin;
    private Camera mainCamera;
    private Vector2Int lastClickedCell = new Vector2Int(-1, -1);

    private void Start()
    {
        mainCamera = Camera.main;
    }

    public void SpawnBoard()
    {
        GetSpriteSize();
        SpawnField();
        //CacheFieldOrigin();
    }

    private void GetSpriteSize()
    {
        if (panelPrefab != null)
        {

            SpriteRenderer spriteRenderer = panelPrefab.GetComponent<SpriteRenderer>();

            if (spriteRenderer != null && spriteRenderer.sprite != null)
            {

                Vector2 spriteSizeInPixels = new Vector2(spriteRenderer.sprite.rect.width, spriteRenderer.sprite.rect.height);
                panelSize = spriteSizeInPixels / spriteRenderer.sprite.pixelsPerUnit;
                panelSize = panelSize * panelPrefab.transform.localScale ;

            }
        }
    }
    private void SpawnField()
    {
        Transform fieldTranfrom = field.transform;

        for (int i = 0; i < FIELD_SIZE; i++)
        {
            for(int j = 0; j < FIELD_SIZE; j++)
            {
                Vector3 pos = fieldTranfrom.position;
                pos.x += panelSize.x * i;
                pos.y -= panelSize.y * j;

                GameObject newPanel = Instantiate(panelPrefab, pos, Quaternion.identity, fieldTranfrom);
                newPanel.name = $"{j}_{i}";
           
            }
        }
    }

    private void CacheFieldOrigin()
    {
        fieldOrigin = field.transform.position;
    }

    // This function converts mouse position to grid coordinates
    public Vector2Int GetClickedCell()
    {
        // Convert mouse position to world ray
        Vector3 mousePos = Input.mousePosition;
        Ray ray = mainCamera.ScreenPointToRay(mousePos);
        RaycastHit2D hit = Physics2D.Raycast(ray.origin, ray.direction);

        // Check if we hit something
        if (hit.collider != null && hit.collider.gameObject.transform.parent == field.transform)
        {
            // Get the name of the panel which was set as "row_column"
            string[] coordinates = hit.collider.gameObject.name.Split('_');
            if (coordinates.Length == 2 &&
                int.TryParse(coordinates[0], out int row) &&
                int.TryParse(coordinates[1], out int column))
            {
                Vector2Int cellPos = new Vector2Int(column, row);
                if (IsValidCell(cellPos))
                {
                    return cellPos;
                }
            }
        }

        return new Vector2Int(-1, -1); // Return (-1, -1) if clicked outside the board
    }

    private bool IsValidCell(Vector2Int cell)
    {
        return cell.x >= 0 && cell.x < FIELD_SIZE &&
               cell.y >= 0 && cell.y < FIELD_SIZE;
    }

    public Vector3 GetWorldPosition(Vector2Int gridPosition)
    {
        if (!IsValidCell(gridPosition))
        {
            return field.transform.position; // Return field origin for invalid positions
        }

        Vector3 worldPosition = field.transform.position;
        worldPosition.x += panelSize.x * gridPosition.x;
        worldPosition.y -= panelSize.y * gridPosition.y;

        return worldPosition;
    }
}
