using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelHandler : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private int collectiblesLeft;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        // Search for all collectibles
        var numCollectibles = GameObject.FindObjectsOfType<CollectibleHandler>().Length;
        
        // Debug
        if (debugMode) print("Found " + numCollectibles + " collectibles.");

        // Set num collectibles
        collectiblesLeft = numCollectibles;
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onCollectCollectible += DecrementNumCollectibles;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onCollectCollectible -= DecrementNumCollectibles;
    }

    public void DecrementNumCollectibles()
    {
        if (debugMode) print("Collectible count decreased!");

        // Reduce num collectibles left
        collectiblesLeft--;

        // If you reach 0
        if (collectiblesLeft <= 0)
        {
            // Open exit
            // Trigger event
            LevelEvents.instance.TriggerOnUnlockExit();
        }
    }
}
