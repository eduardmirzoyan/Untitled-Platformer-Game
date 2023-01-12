using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectibleHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D collider2d;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        collider2d = GetComponentInChildren<Collider2D>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If a player collected this
        if (other.tag == "Player")
        {
            // Debug
            if (debugMode) print("Collected!");

            // Trigger event
            LevelEvents.instance.TriggerOnCollectCollectible();

            // Destroy this collectible
            Destroy(gameObject);
        }
    }
}
