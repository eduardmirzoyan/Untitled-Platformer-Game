using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D collider2d;

    [Header("Data")]
    [SerializeField] private bool canExit;

    [Header("Settings")]
    [SerializeField] private LayerMask interactionLayer;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        collider2d = GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {
        if (canExit) canExit = false;
    }

    public void InteractWithSurroundings()
    {
        // Look for interactibles
        var hit = Physics2D.OverlapBox(collider2d.bounds.center, collider2d.bounds.size, 0f, interactionLayer);

        // Check if hit something
        if (hit)
        {
            // If it's a door
            if (hit.TryGetComponent(out ExitDoorHandler exitDoor))
            {
                // Check if it's unlocked
                if (exitDoor.IsUnlocked())
                {
                    // Debug
                    if (debugMode) print("Exit is unlocked!");

                    // Set flag
                    canExit = true;

                    // Finish
                    return;
                }
                else
                {
                    // Debug
                    if (debugMode) print("Exit is locked!");
                }
            }
        }

        canExit = false;
    }

    public bool CanExit() => canExit;
}
