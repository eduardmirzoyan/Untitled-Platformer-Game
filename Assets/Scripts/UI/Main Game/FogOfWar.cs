using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class FogOfWar : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Tilemap wallTilemap;
    [SerializeField] private Tilemap platformTilemap;
    [SerializeField] private Tilemap spikesTilemap;
    [SerializeField] private Tilemap fogOfWarTileMap;
    [SerializeField] private Tile fogTile;
    [SerializeField] private Tile revealedWallTile;
    [SerializeField] private Tile revealedEmptyTile;
    [SerializeField] private Tile revealedPlatformTile;
    [SerializeField] private Tile revealedSpikesTile;

    [Header("Data")]
    [SerializeField] private Transform discoverer;

    [Header("Settings")]
    [SerializeField] private int discoveryRadius = 5;

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelSetup += GenerateFog;
        LevelEvents.instance.onLevelEnter += SetDiscoverer;
    }


    private void OnDestroy()
    {
        LevelEvents.instance.onLevelSetup -= GenerateFog;
        LevelEvents.instance.onLevelEnter -= SetDiscoverer;
    }

    // Update discovered tiles
    private void LateUpdate()
    {
        // Make sure a discoverer is known
        if (discoverer == null) return;

        // Get location in map
        Vector2Int location = (Vector2Int) fogOfWarTileMap.WorldToCell(discoverer.transform.position);

        // Go through every location around
        for (int i = location.x - discoveryRadius; i <= location.x + discoveryRadius; i++)
        {
            for (int j = location.y - discoveryRadius; j <= location.y + discoveryRadius; j++)
            {

                if ((i - location.x) * (i - location.x) + (j - location.y) * (j - location.y) < discoveryRadius * discoveryRadius)
                {
                    // Reveal the tile
                    RevealFog(new Vector3Int(i, j, 0));
                }

            }
        }
    }

    private void GenerateFog(Vector2Int mapSize, List<CollectibleHandler> collectibles)
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

    private void SetDiscoverer(Transform playerTransform)
    {
        // Set discoverer
        this.discoverer = playerTransform;
    }

    private void RevealFog(Vector3Int position)
    {
        // Check if revealed tile is a wall or empty
        if (wallTilemap.HasTile(position))
        {
            // Set to revealed wall
            fogOfWarTileMap.SetTile(position, revealedWallTile);
        }
        else if (platformTilemap.HasTile(position))
        {
            // Set to revealed platform
            fogOfWarTileMap.SetTile(position, revealedPlatformTile);
        }
        else if (spikesTilemap.HasTile(position))
        {
            // Set to revealed spikes
            fogOfWarTileMap.SetTile(position, revealedSpikesTile);
        }
        else
        {
            // Set to revealed empty
            fogOfWarTileMap.SetTile(position, revealedEmptyTile);
        }
    }
}
