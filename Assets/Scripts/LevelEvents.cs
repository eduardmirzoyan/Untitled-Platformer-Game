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

    public event Action onCollectCollectible;
    public event Action onUnlockExit;

    public void TriggerOnCollectCollectible()
    {
        if (onCollectCollectible != null)
        {
            onCollectCollectible();
        }
    }

    public void TriggerOnUnlockExit()
    {
        if (onUnlockExit != null)
        {
            onUnlockExit();
        }
    }
}
