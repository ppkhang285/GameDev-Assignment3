using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class BoardSpawner : MonoBehaviour
{
    [SerializeField] private GameObject panelPrefab;
    [SerializeField] private GameObject field;

    private const int FIELD_SIZE = 10;
    private Vector2 panelSize;
    public void SpawnBoard()
    {
        GetSpriteSize();
        SpawnField();
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

    
}
