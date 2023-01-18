using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Collider2D platformerCollider;

    [Header("Data")]
    [SerializeField] private bool isDropping;
    [SerializeField] private float targetHeight;
    [SerializeField] private Collider2D platformCollider;

    [Header("Settings")]
    [SerializeField] private LayerMask platformLayer;
    [SerializeField] private float platformThickness = 0.25f;

    [Header("Debug")]
    [SerializeField] private bool debugMode;

    private void Awake()
    {
        // Get ref
        platformerCollider = GetComponentInChildren<Collider2D>();
    }

    private void Update()
    {
        if (isDropping)
        {
            // Get height of the top of the platformer collider
            float currentHeight = platformerCollider.bounds.center.y + platformerCollider.bounds.extents.y;

            // Check to see if you have passed the target
            if (currentHeight <= targetHeight)
            {
                if (debugMode) print("Stopped dropping!");

                // Resume collision
                Physics2D.IgnoreCollision(platformerCollider, platformCollider, false);

                // Finish dropping
                isDropping = false;
            }
        }
    }

    public void Drop()
    {
        // Debug
        if (debugMode) print("Drop request!");

        var startPosition = platformerCollider.bounds.center - Vector3.up * platformerCollider.bounds.extents.y;

        // Search for a platform
        var hit = Physics2D.OverlapCircle(startPosition, platformThickness, platformLayer);

        // If you find a platform
        if (hit && hit.TryGetComponent(out CompositeCollider2D platformCollider))
        {
            // Ignore collision from the platform
            Physics2D.IgnoreCollision(platformerCollider, platformCollider);

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
        if (platformerCollider != null)
        {
            Gizmos.color = Color.yellow;
            var startPosition = platformerCollider.bounds.center - Vector3.up * platformerCollider.bounds.extents.y;
            Gizmos.DrawWireSphere(startPosition, platformThickness);
        }
    }
}
