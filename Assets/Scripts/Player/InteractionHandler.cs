using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractionHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D collider2d;

    [Header("Data")]
    [SerializeField] private Vector3 exitLocation;

    [Header("Settings")]
    [SerializeField] private LayerMask interactionLayer;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        collider2d = GetComponentInChildren<Collider2D>();

        // Set a null value
        exitLocation = Vector3.back;
    }

    private void Update()
    {
        if (exitLocation != Vector3.back) exitLocation = Vector3.back;
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

                    // Save exit
                    exitLocation = exitDoor.GetExitLocation();

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

        exitLocation = Vector3.back;
    }

    public Vector3 GetExit() => exitLocation;
}
