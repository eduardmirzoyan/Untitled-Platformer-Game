using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("Time Data")]
    [SerializeField] private float timeSinceGrounded;
    [SerializeField] private float timeSinceWall;
    [SerializeField] private float jumpInputTime;

    [Header("Frame Data")]
    [SerializeField] private bool groundedThisFrame;
    [SerializeField] private bool groundedLastFrame;
    [SerializeField] private int onWallThisFrame;
    [SerializeField] private int onWallLastFrame;
    [SerializeField] private bool onLedgeThisFrame;
    [SerializeField] private bool onLedgeLastFrame;

    [Header("States")]
    [SerializeField] private bool isFacingRight;
    [SerializeField] private int moveRequest;
    [SerializeField] private bool isJumping;
    [SerializeField] private bool isWallJumping;
    [SerializeField] private bool endJumpRequest;
    [SerializeField] private bool crouchRequest;
    [SerializeField] private bool isWallSliding;
    [SerializeField] private bool isWallHanging;
    [SerializeField] private bool isMantling;
    [SerializeField] private bool isDead;

    [Header("Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkThickness = 0.02f;

    [Header("Debugging")]
    [SerializeField] private Vector2 currentVelocity;

    [Header("Stats")]
    [SerializeField] private MovementStats stats;

    private void Awake()
    {
        // Get refs
        body = GetComponentInChildren<Rigidbody2D>();
        hurtBox = GetComponentInChildren<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Set default states
        isFacingRight = true;
        jumpInputTime = -1f;
        timeSinceGrounded = -1f;
        timeSinceWall = -1f;
    }

    #region Input

    public void Die()
    {
        // Disable any other states
        isMantling = false;
        isWallHanging = false;
        isWallSliding = false;

        // Set flag
        isDead = true;
    }

    public void Stop()
    {
        moveRequest = 0;
    }

    public void MoveRight()
    {
        moveRequest = 1;
    }

    public void MoveLeft()
    {
        moveRequest = -1;
    }

    public void StartCrouch()
    {
        crouchRequest = true;
    }

    public void EndCrouch()
    {
        crouchRequest = false;
    }

    public void Jump()
    {
        // Store time
        jumpInputTime = Time.time;
    }

    public void EndJumpEarly()
    {
        // If you are mid jump (not wall jump)
        if (isJumping && !isWallJumping)
        {
            // Set flag
            endJumpRequest = true;

            // Disable flag
            isJumping = false;
        }
        
    }

    #endregion

    private void Update()
    {
        CheckCollisions();

        CheckWalls();

        HandleJumping();

        HandleHorizontalMovement();

        HandleVerticalMovement();

        ApplyVelocity();
        
        UpdatePreviousStates();
    }

    public bool IsRunning() => Mathf.Abs(currentVelocity.x) > 0.1f;
    public bool IsGrounded() => groundedThisFrame;
    public bool IsWallSliding() => isWallSliding;
    public bool IsTouchingLedge() => isWallHanging;
    public bool IsRising() => !groundedThisFrame && body.velocity.y > 0.1f;
    public bool IsFalling() => !groundedThisFrame && body.velocity.y < -0.1f;
    public bool IsMantling() => isMantling;
    public bool IsCrouching() => crouchRequest;

    public void PerformMantle()
    {
        // Must be mantling before hand
        if (isMantling)
        {
            // Get corner of ledge check based on which wall u are on
            var corner = hurtBox.bounds.center + new Vector3(onWallThisFrame * hurtBox.bounds.extents.x, hurtBox.bounds.extents.y, 0f);

            // Get offset
            var offset = new Vector3(onWallThisFrame * hurtBox.bounds.extents.x, hurtBox.bounds.extents.y + checkThickness, 0f);

            // Relocate transform
            transform.position = corner + offset;

            // Flip model to face away of the wall they climbed
            FlipModel(-onWallThisFrame);
            
            // Disable flag
            isMantling = false;
        }
    }
    
    private void CheckCollisions()
    {
        // Set this frame state
        groundedThisFrame = CalcTouchGround();
        onWallThisFrame = CalcTouchWall();
        onLedgeThisFrame = CalcTouchLedge();
    }

    private void CheckWalls()
    {
        CheckMantling();

        CheckWallHanging();

        CheckWallSliding();
    }

    private void CheckMantling()
    {
        // You must previously be hanging then moving in the same direction as the wall you are on and then jump
        if (isWallHanging &&  onWallThisFrame == moveRequest && JumpWithinBuffer())
        {
            // Toggle mantling
            isMantling = true;

            // Disable wall hanging
            isWallHanging = false;
        }
    }

    private void CheckWallHanging()
    {   
        // On wall but no ledge LAST frame => now on wall and on ledge THIS frame
        if (IsFalling() && onWallLastFrame != 0 && !onLedgeLastFrame && onWallThisFrame != 0 && onLedgeThisFrame)
        {
            // Toggle hanging
            isWallHanging = true;
        }
    }

    private void CheckWallSliding()
    {
        // You must be falling, on ledge and wall, and moving into the wall
        if (!isWallHanging && IsFalling() && onLedgeThisFrame && onWallThisFrame != 0 && onWallThisFrame == moveRequest)
        {
            isWallSliding = true;
        }
        // Else stop sliding
        else
        {
            isWallSliding = false;
        }
    }

    private void HandleHorizontalMovement()
    {
        // Check if player is dead
        if (isDead)
        {
            // Decelerate to 0, slowly
            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0f, stats.deathDeceleration * Time.deltaTime);

            // Finish
            return;
        }

        // Check if player is on wall
        if (isWallSliding || isWallHanging || isMantling)
        {
            // Don't move horizontally
            currentVelocity.x = 0f;

            // Finish
            return;
        }

        // If player wants to move horizontally
        if (moveRequest != 0)
        {
            // Calculate target speed
            float targetSpeed = 0f;

            // Check if player is crouching
            if (crouchRequest)
            {
                // Calculate target speed using crouchwalk
                targetSpeed = moveRequest * stats.maxCrouchWalkSpeed;
            }
            else 
            {
                // Calculate target speed
                targetSpeed = moveRequest * stats.maxRunSpeed;
            }

            // Accelerate
            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetSpeed, stats.acceleration * Time.deltaTime);
        }
        else
        {
            // Check whether to use ground friction or air friction
            if (groundedThisFrame)
            {
                // Decelerate to 0
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0, stats.groundDeceleration * Time.deltaTime);
            }
            else
            {
                // Decelerate to 0
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0, stats.airDeceleration * Time.deltaTime);
            }
        }
    }

    private void HandleJumping()
    {
        // Make sure you are within buffer time
        if (!isMantling && JumpWithinBuffer())
        {
            // Make sure you are grounded OR within coyote time
            if (groundedThisFrame || JumpWithinCoyote())
            {
                // Add jumpower
                currentVelocity.y = stats.jumpPower;

                // Reset timers
                jumpInputTime = -1f;
                timeSinceGrounded = -1f;

                // Set flag
                isJumping = true;
            }
            // If you are on a wall sliding or wallhaning, then wall jump instead
            else if (onWallThisFrame != 0 || WallJumpWithinCoyote())
            {
                // Wall jump
                currentVelocity = Vector2.Scale(stats.wallJumpPower, new Vector2(-onWallThisFrame, 1));

                // Reset timers
                jumpInputTime = -1f;
                timeSinceWall = -1f;

                // Stop wall hanging or sliding
                isWallHanging = false;
                isWallSliding = false;

                // Set flag
                isJumping = true;
                isWallJumping = true;
            }
        }

        // Check if jump is ended early
        if (endJumpRequest)
        {
            // Reduce current velocity by a factor
            currentVelocity.y /= stats.earlyCancelFactor;

            // Set flags
            endJumpRequest = false;
        }
    }

    private void HandleVerticalMovement()
    {
        // If you are hanging or mantling
        if (isWallHanging || isMantling)
        {
            // Don't fall
            currentVelocity.y = 0;

            // Finish
            return;
        }

        // If you JUST landed
        if (!groundedLastFrame && groundedThisFrame)
        {
            // Remove y vel
            currentVelocity.y = 0f;

            // Stop
            isJumping = false;
            isWallJumping = false;  

            // Finish
            return;
        }

        // If you are airborne
        if (!groundedThisFrame)
        {
            // Apply different gravity based on state
            if (IsRising())
            {
                // Check if you hit your head on the ceiling
                if (TouchHead())
                {
                    // Set y vel to 0
                    currentVelocity.y = 0;
                }

                // Apply weaker gravity
                currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, -stats.maxFallSpeed, stats.risingGravity * Time.deltaTime);
            }
            // If you are falling
            else 
            {
                // Check if you are moving into the same side as the wall
                if (isWallSliding)
                {
                    // Start wallsliding
                    currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, -stats.maxWallSlideSpeed, stats.wallSlideAcceleration * Time.deltaTime);
                }
                else 
                {
                    // Apply stronger gravity
                    currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, -stats.maxFallSpeed, stats.fallingGravity * Time.deltaTime);
                }
            }
        }
    }

    private bool JumpWithinBuffer() => Time.time - jumpInputTime <= stats.jumpBuffer;
    private bool JumpWithinCoyote() => Time.time - timeSinceGrounded <= stats.coyoteTime;
    private bool WallJumpWithinCoyote() => Time.time - timeSinceWall <= stats.coyoteTime;

    private bool CalcTouchGround()
    {
        var position = hurtBox.bounds.center - new Vector3(0f, hurtBox.bounds.extents.y, 0f);
        var size = new Vector2(0.9f * hurtBox.bounds.size.x, checkThickness);

        return Physics2D.OverlapBox(position, size, 0, groundLayer) && body.velocity.y < 0.1f;
    }

    private int CalcTouchWall()
    {
        var size = new Vector2(checkThickness, hurtBox.bounds.extents.y);

        // Left side
        var position = hurtBox.bounds.center + new Vector3(-hurtBox.bounds.extents.x, -hurtBox.bounds.extents.y / 4, 0f);
        var hitLeft = Physics2D.OverlapBox(position, size, 0, groundLayer);

        // Right side
        position = hurtBox.bounds.center + new Vector3(hurtBox.bounds.extents.x, -hurtBox.bounds.extents.y / 4, 0f);
        var hitRight = Physics2D.OverlapBox(position, size, 0, groundLayer);

        if (hitLeft)
        {
            // Touching wall on left
            return -1;
        }
        else if (hitRight)
        {
            // Touching wall on right
            return 1;
        }

        // Not touching any walls
        return 0;
    }

    private bool CalcTouchLedge()
    {
        // Look for ledge
        var position = hurtBox.bounds.center + new Vector3(0f, hurtBox.bounds.extents.y, 0f);
        var size = new Vector2(hurtBox.bounds.size.x + checkThickness, checkThickness);
        return Physics2D.OverlapBox(position, size, 0, groundLayer);
    }

    private bool TouchHead()
    {
        var position = hurtBox.bounds.center + new Vector3(0f, hurtBox.bounds.extents.y, 0f);
        var size = new Vector2(0.9f * hurtBox.bounds.size.x, checkThickness);
        return Physics2D.OverlapBox(position, size, 0, groundLayer);
    }

    private void FlipModel(float direction)
    {
        if (direction > 0.1f && !isFacingRight)
        {
            // Flip model
            transform.rotation = Quaternion.identity;
            isFacingRight = true;
        }
        else if (direction < -0.1f && isFacingRight)
        {
            // Flip model
            transform.rotation = Quaternion.Euler(0, 180, 0);
            isFacingRight = false;
        }
    }

    private void ApplyVelocity()
    {
        // Set velocity
        body.velocity = currentVelocity;

        // Flip model if neccissary
        FlipModel(currentVelocity.x);
    }

    private void UpdatePreviousStates()
    {
        // If was grounded and now not
        if (groundedLastFrame && !groundedThisFrame)
        {
            // Save time
            timeSinceGrounded = Time.time;
        }

        // If was on wall and now not
        if (onWallLastFrame != 0 && onWallThisFrame != onWallLastFrame)
        {
            // Save time
            timeSinceWall = Time.time;
        }

        // Update grounded state
        groundedLastFrame = groundedThisFrame;

        // Update wall state
        onWallLastFrame = onWallThisFrame;

        // Update ledge state
        onLedgeLastFrame = onLedgeThisFrame;
    }

    private void OnDrawGizmosSelected()
    {
        // Ground check
        Gizmos.color = Color.red;
        var position = hurtBox.bounds.center - new Vector3(0f, hurtBox.bounds.extents.y, 0f);
        var size = new Vector2(0.9f * hurtBox.bounds.size.x, checkThickness);
        Gizmos.DrawWireCube(position, size);

        // Wall check
        Gizmos.color = Color.blue;
        // Left side
        position = hurtBox.bounds.center + new Vector3(-hurtBox.bounds.extents.x, -hurtBox.bounds.extents.y / 4, 0f);
        size = new Vector2(checkThickness, hurtBox.bounds.extents.y);
        Gizmos.DrawWireCube(position, size);
        
        // Right side
        position = hurtBox.bounds.center + new Vector3(hurtBox.bounds.extents.x, -hurtBox.bounds.extents.y / 4, 0f);
        size = new Vector2(checkThickness, hurtBox.bounds.extents.y);
        Gizmos.DrawWireCube(position, size);

        // Ledge check
        Gizmos.color = Color.yellow;
        position = hurtBox.bounds.center + new Vector3(0f, hurtBox.bounds.extents.y, 0f);
        size = new Vector2(hurtBox.bounds.size.x + checkThickness, checkThickness);
        Gizmos.DrawWireCube(position, size);

        // Head check
        Gizmos.color = Color.magenta;
        position = hurtBox.bounds.center + new Vector3(0f, hurtBox.bounds.extents.y, 0f);
        size = new Vector2(0.9f * hurtBox.bounds.size.x, checkThickness);
        Gizmos.DrawWireCube(position, size);
    }
}
