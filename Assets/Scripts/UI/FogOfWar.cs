using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap fogOfWarTileMap;
    [SerializeField] private Tile fogTile;
    [SerializeField] private Tile revealedWallTile;
    [SerializeField] private Tile revealedEmptyTile;
    [SerializeField] private Transform discoverer;

    [Header("Components")]
    [SerializeField] private Vector2 mapSize;
    [SerializeField] private int discoveryRadius = 11;

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelEnter += GenerateFog;
    }


    private void OnDestroy()
    {
        LevelEvents.instance.onLevelEnter -= GenerateFog;
    }

    // Update discovered tiles
    private void LateUpdate()
    {
        // Get location in map
        Vector2Int location = (Vector2Int) fogOfWarTileMap.WorldToCell(discoverer.transform.position);

        // Go through every location around
        for (int i = location.x - discoveryRadius; i <= location.x + discoveryRadius; i++)
        {
            for (int j = location.y - discoveryRadius; j <= location.y + discoveryRadius; j++)
            {

                if ((i - location.x) * (i - location.x) + (j - location.y) * (j - location.y) < discoveryRadius * discoveryRadius)
                {
                    // fogOfWarTileMap.SetTile(new Vector3Int(i, j, 0), null);
                    Reveal(new Vector3Int(i, j, 0));
                }

            }
        }
    }

    public void GenerateFog()
    {
        // Create a fog tile in every location
        for (int i = 0; i < mapSize.x; i++)
        {
            for (int j = 0; j < mapSize.y; j++)
            {
                fogOfWarTileMap.SetTile(new Vector3Int(i, j, 0), fogTile);
            }
        }
    }

    private void Reveal(Vector3Int position)
    {
        // Check if revealed tile is a wall or empty
        if (wallTilemap.HasTile(position))
        {
            // Set to revealed wall
            fogOfWarTileMap.SetTile(position, revealedWallTile);
        }
        else
        {
            // Set to revealed empty
            fogOfWarTileMap.SetTile(position, revealedEmptyTile);
        }
    }
}
