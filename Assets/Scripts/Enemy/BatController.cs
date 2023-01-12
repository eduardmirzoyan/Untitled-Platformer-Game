using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BatController : MonoBehaviour
{
    private enum BatState { Idle, Aggro };

    [Header("Components")]
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private Collider2D collider2d;
    [SerializeField] private NavMeshAgent agent;

    [Header("Data")]
    [SerializeField] private BatState batState;
    [SerializeField] private Transform targetTransform;

    [Header("Settings")]
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private float aggroRange = 5f;

    private void Awake()
    {
        animationHandler = GetComponent<AnimationHandler>();
        collider2d = GetComponentInChildren<Collider2D>();

        agent = GetComponent<NavMeshAgent>();
        agent.updateRotation = false;
        agent.updateUpAxis = false;
    }

    private void Start()
    {
        // Set start state
        batState = BatState.Idle;
        animationHandler.ChangeAnimation("Idle");
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
                    // Change animation
                    animationHandler.ChangeAnimation("Aggro");

                    // Change state
                    batState = BatState.Aggro;
                }

                break;
            case BatState.Aggro:

                // Chase the target
                ChaseTarget();

                break;
            default:
                throw new System.Exception("STATE NOT IMPLEMENTED.");
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, aggroRange);
    }

    private void SearchForTarget()
    {
        // Search for anything in range
        var hit = Physics2D.OverlapCircle(transform.position, aggroRange, targetLayer);

        if (hit)
        {
            // Set target
            targetTransform = hit.transform;
        }
    }

    private void ChaseTarget()
    {
        // Follow the target
        agent.SetDestination(targetTransform.position);
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        // If the target can be damaged
        if (other.transform.root.TryGetComponent(out DamageHandler damageHandler))
        {
            // Damage it
            damageHandler.Hurt();
        }
    }
}
