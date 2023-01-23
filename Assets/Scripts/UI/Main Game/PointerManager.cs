using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PointerManager : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private GameObject pointerPrefab;

    [Header("Settings")]
    [SerializeField] private Color collectibleArrowColor;
    [SerializeField] private Color exitArrowColor;

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelSetup += CreateArrowsToCollectibles;
        LevelEvents.instance.onUnlockExit += CreateArrowsToExit;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLevelSetup -= CreateArrowsToCollectibles;
        LevelEvents.instance.onUnlockExit -= CreateArrowsToExit;
    }

    private void CreateArrowsToCollectibles(Vector2Int mapSize, List<CollectibleHandler> collectibles)
    {
        foreach (var collectible in collectibles)
        {
            // Create a pointer to each collectible
            var pointer = Instantiate(pointerPrefab, transform).GetComponent<ArrowPointer>();
            var playerTransform = LevelManager.instance.GetPlayerTransform();
            pointer.Initialize(playerTransform, collectible.transform, collectibleArrowColor);
        }
    }

    private void CreateArrowsToExit(Transform exitTransform)
    {
        // Create a pointer to each collectible
        var pointer = Instantiate(pointerPrefab, transform).GetComponent<ArrowPointer>();
        var playerTransform = LevelManager.instance.GetPlayerTransform();
        pointer.Initialize(playerTransform, exitTransform, exitArrowColor);
    }
}
