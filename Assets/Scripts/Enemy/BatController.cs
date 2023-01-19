using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BatController : MonoBehaviour
{
    private enum BatState { Idle, Aggro, Return };

    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private NavMeshAgent agent;
    [SerializeField] private Animator indicatorAnimator;
    [SerializeField] private SpriteRenderer aggroRangeDisplay;

    [Header("Data")]
    [SerializeField] private BatState batState;
    [SerializeField] private Transform targetTransform;
    [SerializeField] private Vector3 homePosition;

    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float aggroRange = 5f;
    [SerializeField] private float aggroRingRotateSpeed = 10f;
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float deAggroRange = 10f;
    [SerializeField] private float returnRange = 0.1f;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        collider2d = GetComponentInChildren<Collider2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;

        // Set size based on range
        aggroRangeDisplay.transform.localScale = Vector3.one * aggroRange;
    }

    private void Start()
    {
        // Set start state
        batState = BatState.Idle;
        animationHandler.ChangeAnimation("Idle");

        // Find a perching spot from where it was spawned in
        FindPerchingSpot();
    }

    private void Update()
    {
        switch (batState)
        {
            case BatState.Idle:

                // Search for any targets
                SearchForTarget();

                // If a target was found
                if (targetTransform != null)
                {
                    // Hide ring
                    aggroRangeDisplay.enabled = false;

                    // Show indicator
                    indicatorAnimator.Play("Show Red");

                    // Play sound
                    AudioManager.instance.Play("Enemy Aggro");

                    // Change animation
                    animationHandler.ChangeAnimation("Aggro");

                    // Change state
                    batState = BatState.Aggro;
                }

                break;
            case BatState.Aggro:

                // Chase the target
                ChaseTarget();

                // Check to see if target is out of range
                if (Vector2.Distance(homePosition, targetTransform.position) >= deAggroRange)
                {
                    // Drop aggro
                    targetTransform = null;

                    // Show indicator
                    indicatorAnimator.Play("Show Blue");

                    // Change state
                    batState = BatState.Return;
                }

                break;
            case BatState.Return:
                
                // Travel home
                agent.SetDestination(homePosition);

                // If reached home
                if (Vector2.Distance(transform.position, homePosition) <= returnRange)
                {
                    // Show ring
                    aggroRangeDisplay.enabled = true;

                    // Hide indicator
                    indicatorAnimator.Play("Idle");

                    // Play animation
                    animationHandler.ChangeAnimation("Idle");

                    // Change state
                    batState = BatState.Idle;
                }

                break;
            default:
                throw new System.Exception("STATE NOT IMPLEMENTED.");
        }
    }

    private void FindPerchingSpot()
    {
        var hit = Physics2D.Raycast(transform.position, Vector2.up, float.PositiveInfinity, groundLayer);

        // If a ceiling was hit
        if (hit)
        {
            // Calculate new position
            var newPosition = hit.point - new Vector2(0f, collider2d.bounds.extents.y);

            // Relocate
            transform.position = newPosition;

            // Set home here
            homePosition = newPosition;
        }
    }

    private void SearchForTarget()
    {
        // Search for anything in range
        var hit = Physics2D.OverlapCircle(transform.position, aggroRange, targetLayer);

        if (hit)
        {
            // Get direction
            var direction = (hit.transform.position - transform.position).normalized;

            // Now check for line of sight
            var visionHit = Physics2D.Raycast(transform.position, direction, aggroRange, groundLayer);

            // If NOT obstructed
            if (!visionHit)
            {
                // Set target
                targetTransform = hit.transform;
            }

        }

        // Rotate ring
        RotateAggroRing();
    }

    private void RotateAggroRing()
    {
        // Rotate ring
        aggroRangeDisplay.transform.Rotate(Vector3.forward * (aggroRingRotateSpeed * Time.deltaTime));
    }

    private void ChaseTarget()
    {
        // Follow the target
        agent.SetDestination(targetTransform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the target can be damaged
        if (other.transform.parent.TryGetComponent(out DamageHandler damageHandler))
        {
            // Damage it
            damageHandler.Hurt();
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(homePosition, deAggroRange);

        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(homePosition, returnRange);
    }
}
