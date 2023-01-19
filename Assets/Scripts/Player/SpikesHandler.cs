using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private DamageHandler damageHandler;

    private void Awake()
    {
        // Get refs
        movementHandler = GetComponent<MovementHandler>();
        damageHandler = GetComponent<DamageHandler>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // When you collide with spikes
        
        // Make sure you are falling
        if (movementHandler.IsFalling() && other.tag == "Spikes")
        {
            // Hurt character
            damageHandler.Hurt();
        }
    }

    
}
