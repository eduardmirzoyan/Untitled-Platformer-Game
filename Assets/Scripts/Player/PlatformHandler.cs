using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D feetCollider;

    [Header("Data")]
    [SerializeField] private bool isDropping;
    [SerializeField] private float targetHeight;
    [SerializeField] private Collider2D platformCollider;

    [Header("Settings")]
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float platformThickness = 0.25f;

    [Header("Debug")]
    [SerializeField] private bool debugMode;

    private void Update()
    {
        if (isDropping)
        {
            // Get height of the top of the platformer collider
            float currentHeight = feetCollider.bounds.center.y + feetCollider.bounds.extents.y;

            // Check to see if you have passed the target
            if (currentHeight <= targetHeight)
            {
                if (debugMode) print("Stopped dropping!");

                // Resume collision
                Physics2D.IgnoreCollision(feetCollider, platformCollider, false);

                // Remove ref
                platformCollider = null;

                // Reset target
                targetHeight = 0f;

                // Finish dropping
                isDropping = false;
            }
        }
    }

    public bool IsDropping() => isDropping;

    public void Drop()
    {
        var startPosition = feetCollider.bounds.center - Vector3.up * feetCollider.bounds.extents.y;

        // Search for a platform
        var hit = Physics2D.OverlapCircle(startPosition, platformThickness, platformLayer);

        // If you find a platform
        if (hit && hit.TryGetComponent(out CompositeCollider2D platformCollider))
        {
            // Debug
            if (debugMode) print("Dropping!");

            // Ignore collision from the platform
            Physics2D.IgnoreCollision(feetCollider, platformCollider);

            // Set target as under the platform you are currently on
            targetHeight = startPosition.y - platformThickness;

            // Save ref
            this.platformCollider = platformCollider;

            // Set flag
            isDropping = true;
        }
        else { print("Platform not found."); }
    }

    private void OnDrawGizmosSelected()
    {
        if (feetCollider != null)
        {
            Gizmos.color = Color.yellow;
            var startPosition = feetCollider.bounds.center - Vector3.up * feetCollider.bounds.extents.y;
            Gizmos.DrawWireSphere(startPosition, platformThickness);
        }
        else { print("Feet collider not assigned!"); }
    }
}
