using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class LevelEvents : MonoBehaviour
{
    public static LevelEvents instance;
    private void Awake()
    {
        // Singleton Logic
        if (LevelEvents.instance != null)
        {
            Destroy(gameObject);
            return;
        }

        instance = this;
    }

    public event Action<Vector2Int, List<CollectibleHandler>> onLevelSetup;
    public event Action<Transform> onLevelEnter;
    public event Action onLockEntrance;
    public event Action<CollectibleHandler> onCollect;
    public event Action onPlayerDeath;
    public event Action<Transform> onUnlockExit;
    public event Action<Transform> onLevelExit;

    public void TriggerOnLevelSetup(Vector2Int mapSize, List<CollectibleHandler> collectibles)
    {
        if (onLevelSetup != null)
        {
            onLevelSetup(mapSize, collectibles);
        }
    }

    public void TriggerOnLevelEnter(Transform playerTransform)
    {
        if (onLevelEnter != null)
        {
            onLevelEnter(playerTransform);
        }
    }

    public void TriggerOnLockEntrance()
    {
        if (onLockEntrance != null)
        {
            onLockEntrance();
        }
    }

    public void TriggerOnCollect(CollectibleHandler collectibleHandler)
    {
        if (onCollect != null)
        {
            onCollect(collectibleHandler);
        }
    }

    public void TriggerOnPlayerDeath()
    {
        if (onPlayerDeath != null)
        {
            onPlayerDeath();
        }
    }

    public void TriggerOnUnlockExit(Transform transform)
    {
        if (onUnlockExit != null)
        {
            onUnlockExit(transform);
        }
    }

    public void TriggerOnLevelExit(Transform playerTransform)
    {
        if (onLevelExit != null)
        {
            onLevelExit(playerTransform);
        }
    }
}
