using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EntranceDoorHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Transform playerStartLocation;

    private void Awake()
    {
        animationHandler = GetComponentInChildren<AnimationHandler>();
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelEnter += RelocatePlayer;
        LevelEvents.instance.onLockEntrance += Close;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLevelEnter -= RelocatePlayer;
        LevelEvents.instance.onLockEntrance -= Close;
    }

    private void RelocatePlayer(Transform playerTransform)
    {
        // Move the player to the start location
        playerTransform.position = playerStartLocation.position;
    }

    private void Close()
    {
        animationHandler.ChangeAnimation("Close");
    }
}
