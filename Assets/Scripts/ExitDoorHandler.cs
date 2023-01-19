using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class ExitDoorHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Transform exitLocationTransform;
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private Light2D lightSource;

    [Header("Data")]
    [SerializeField] private bool unlocked;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        animationHandler = GetComponentInChildren<AnimationHandler>();
        lightSource = GetComponentInChildren<Light2D>();
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onUnlockExit += Unlock;
        LevelEvents.instance.onLevelExit += ExitLevel;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onUnlockExit += Unlock;
        LevelEvents.instance.onLevelExit -= ExitLevel;
    }

    private void Unlock(Transform transform)
    {
        // If this was unlocked...
        if (this.transform == transform)
        {
            // If not unlocked before...
            if (!unlocked)
            {
                // Debug
                if (debugMode) print("Exit unlocked!");

                // Play animation
                animationHandler.ChangeAnimation("Unlock");

                // Show light
                lightSource.enabled = true;

                // Play sound
                audioSource.Play();

                // Change state
                unlocked = true;
            }
        }
    }

    private void ExitLevel(Transform playerTransform)
    {
        // Relocate player
        playerTransform.position = exitLocationTransform.position;
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }
}
