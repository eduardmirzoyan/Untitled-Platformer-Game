using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DamageHandler : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private bool isHurt;

    [Header("Debugging")]
    [SerializeField] private bool debugMode;

    public void Hurt()
    {
        // Debug
        if (debugMode) print("HURT!");

        // Set flag
        isHurt = true;
    }

    public bool IsHurt() => isHurt;
}
