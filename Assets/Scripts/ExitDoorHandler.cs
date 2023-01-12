using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExitDoorHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Transform exitLocationTransform;

    [Header("Data")]
    [SerializeField] private bool unlocked;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        animationHandler = GetComponentInChildren<AnimationHandler>();
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onUnlockExit += Unlock;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onUnlockExit += Unlock;
    }

    private void Unlock()
    {
        // If not unlocked before...
        if (!unlocked)
        {
            // Debug
            if (debugMode) print("Exit unlocked!");

            // Play animation
            animationHandler.ChangeAnimation("Unlock");

            // Change state
            unlocked = true;
        }
    }

    public bool IsUnlocked()
    {
        return unlocked;
    }

    public Vector3 GetExitLocation()
    {
        return exitLocationTransform.position;
    }
}
