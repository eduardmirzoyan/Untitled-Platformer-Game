using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Collider2D groundHurtbox;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Data")]
    [SerializeField] private float moveDirection;
    [SerializeField] private float currentSpeed;
    [SerializeField] private bool isFacingRight;

    [Header("Other Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private Transform groundCheck;
    [SerializeField] private Vector2 checkSize;

    [SerializeField] private Transform wallCheck;

    [Header("Run Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float deceleration = 60f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float fallMultiplier = 2.5f;
    [SerializeField] private float cutFactor = 3f;


    private void Awake()
    {
        // Get refs
        body = GetComponentInChildren<Rigidbody2D>();
        groundHurtbox = GetComponentInChildren<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Set default state
        isFacingRight = true;
    }

    private void Update()
    {
        // Calculate target speed
        // float targetSpeed = moveDirection * moveSpeed;
        // Accelerate current speed
        // currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        

        // Deceleration
        // if (!HorizontalInputPressed)
        // {
        //     var deceleration = _grounded ? _stats.GroundDeceleration : _stats.AirDeceleration;
        //     _speed.x = Mathf.MoveTowards(_speed.x, 0, deceleration * Time.fixedDeltaTime);
        // }
        // // Regular Horizontal Movement
        // else
        // {
        //     // Prevent useless horizontal speed buildup when against a wall
        //     if (_hittingWall.collider && Mathf.Abs(_rb.velocity.x) < 0.01f && !_isLeavingWall) _speed.x = 0;

        //     var xInput = _frameInput.Move.x * (ClimbingLadder ? _stats.LadderShimmySpeedMultiplier : 1);
        //     _speed.x = Mathf.MoveTowards(_speed.x, xInput * _stats.MaxSpeed, _currentWallJumpMoveMultiplier * _stats.Acceleration * Time.fixedDeltaTime);
        // }

        // Horizontal movement
        if (Mathf.Abs(moveDirection) > 0.1f)
        {
            // Calculate target speed
            float targetSpeed = moveDirection * moveSpeed;
            // Accelerate
            currentSpeed = Mathf.MoveTowards(currentSpeed, targetSpeed, acceleration * Time.deltaTime);
        }
        else 
        {
            // Decelerate
            currentSpeed = Mathf.MoveTowards(currentSpeed, 0, deceleration * Time.deltaTime);
        }

        // Set speed
        body.velocity = new Vector2(currentSpeed, body.velocity.y);

        if (body.velocity.y < 0)
        {
            body.velocity += Vector2.up * Physics2D.gravity.y * body.gravityScale * fallMultiplier * Time.deltaTime; 
        }
    }

    public void Stop()
    {
        moveDirection = 0f;
    }

    public void MoveRight()
    {
        moveDirection = 1f;

        FlipFace(moveDirection);
    }


    public void MoveLeft()
    {
        moveDirection = -1f;

        FlipFace(moveDirection);
    }

    public void Jump()
    {
        // Give upward velocity
        body.velocity = new Vector2(body.velocity.x, jumpPower); 
    }

    public void CancelJump()
    {
        body.velocity = new Vector2(body.velocity.x, body.velocity.y / cutFactor);
    }

    private void FlipFace(float direction)
    {
        if (direction > 0 && !isFacingRight)
        {
            // Flip model
            transform.rotation = Quaternion.identity;
            isFacingRight = true;
        }
        else if (direction < 0 && isFacingRight)
        {
            // Flip model
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
    }

    // public bool IsGrounded() => Physics2D.BoxCast(transform.position, groundHurtbox.bounds.size, 0f, Vector2.down, 0.1f, groundLayer) && body.velocity.y < 0.1f;
    public bool IsRunning() => Mathf.Abs(body.velocity.x) > 0.1f;
    public bool IsGrounded() => Physics2D.OverlapBox(groundCheck.position, checkSize, 0, groundLayer) && body.velocity.y < 0.1f;
    public bool IsTouchingWall() => Physics2D.OverlapBox(wallCheck.position, checkSize, 0, groundLayer);
    public bool IsFalling() => body.velocity.y < -0.1f;
    public bool IsRising() => body.velocity.y > 0.1f;

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireCube(groundCheck.position, checkSize);

        Gizmos.color = Color.blue;
        Gizmos.DrawWireCube(wallCheck.position, checkSize);
    }
}
