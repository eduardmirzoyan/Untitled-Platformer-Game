using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpikesHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private DamageHandler damageHandler;
    
    [Header("Settings")]
    [SerializeField] private float checkThickness = 0.1f;
    [SerializeField] private LayerMask spikesLayer;

    private void Awake()
    {
        // Get refs
        collider2d = GetComponentInChildren<Collider2D>();
        movementHandler = GetComponent<MovementHandler>();
        damageHandler = GetComponent<DamageHandler>();
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // When you collide with spikes

        // Make sure you are falling
        if (movementHandler.IsFalling())
        {
            // Hurt character
            damageHandler.Hurt();


            // // Bottom of your hitbox is touching
            // var position = collider2d.bounds.center - Vector3.up * collider2d.bounds.extents.y;
            // var size = new Vector3(collider2d.bounds.size.x, checkThickness);
            // var hit = Physics2D.OverlapBox(position, size, 0f, spikesLayer);

            // // If you did contact spikes
            // if (hit)
            // {
                
            // }

        }
    }

    
}
