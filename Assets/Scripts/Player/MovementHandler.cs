using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementHandler : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private Rigidbody2D body;
    [SerializeField] private Collider2D hurtBox;
    [SerializeField] private SpriteRenderer spriteRenderer;

    [Header("General Data")]
    [SerializeField] private Vector2 currentVelocity;
    [SerializeField] private bool isFacingRight;

    [Header("Run Data")]
    [SerializeField] private int moveRequest;

    [Header("Jump Data")]
    [SerializeField] private bool isJumping;
    [SerializeField] private bool endJumpRequest;
    [SerializeField] private float jumpInputTime = -1f;
    [SerializeField] private float timeSinceGrounded = -1f;
    [SerializeField] private bool groundedThisFrame;
    [SerializeField] private bool groundedLastFrame;


    [Header("Wallslide Data")]
    [SerializeField] private int onWallThisFrame;
    [SerializeField] private int onWallLastFrame;
    [SerializeField] private bool isWallSliding;

    [Header("Walljump Data")]
    [SerializeField] private bool walljumpRequest;

    [Header("Wallhang Data")]
    [SerializeField] private bool onLedgeThisFrame;
    [SerializeField] private bool onLedgeLastFrame;
    [SerializeField] private bool grabbingLedge;

    [Header("Mantle Data")]
    [SerializeField] private bool canMantle;

    [Header("Other Settings")]
    [SerializeField] private LayerMask groundLayer;
    [SerializeField] private float checkThickness = 0.02f;

    [Header("Run Settings")]
    [SerializeField] private float maxRunSpeed = 4f;
    [SerializeField] private float acceleration = 0.1f;
    [SerializeField] private float groundDeceleration = 60f;
    [SerializeField] private float airDeceleration = 30f;

    [Header("Jump Settings")]
    [SerializeField] private float jumpPower = 5f;
    [SerializeField] private float earlyCancelFactor = 3f;
    [SerializeField] private float coyoteTime = 0.1f;
    [SerializeField] private float jumpBuffer = 0.1f;

    [Header("Crouch Settings")]
    [SerializeField] private bool crouchRequest;
    [SerializeField] private float maxCrouchwalkSpeed = 2f;

    [Header("Wallslide Settings")]
    [SerializeField] private float maxWallslideSpeed = 2f;
    [SerializeField] private float wallSlideAcceleration = 30f;

    [Header("Wallslide Settings")]
    [SerializeField] private Vector2 walljumpPower = new Vector2(10, 8);

    [Header("Gravity Settings")]
    [SerializeField] private float risingGravity = 20f;
    [SerializeField] private float fallingGravity = 40f;
    [SerializeField] private float maxFallSpeed = 40;

    private void Awake()
    {
        // Get refs
        body = GetComponentInChildren<Rigidbody2D>();
        hurtBox = GetComponentInChildren<Collider2D>();
        spriteRenderer = GetComponentInChildren<SpriteRenderer>();

        // Set default state
        isFacingRight = true;
    }

    private void Update()
    {
        CheckCollisions();

        CheckWalls();

        HandleJumping();

        HandleHorizontalMovement();

        HandleVerticalMovement();

        // Set velocity
        FlipModel(currentVelocity.x);
        body.velocity = currentVelocity;
        

        // If was grounded and now not
        if (groundedLastFrame && !groundedThisFrame)
        {
            // Save time
            timeSinceGrounded = Time.time;
        }

        // Update grounded state
        groundedLastFrame = groundedThisFrame;

        // Update wall state
        onWallLastFrame = onWallThisFrame;

        // Update ledge state
        onLedgeLastFrame = onLedgeThisFrame;
    }

    public void Stop()
    {
        moveRequest = 0;
    }

    public void MoveRight()
    {
        moveRequest = 1;

        // FlipModel(moveRequest);
    }


    public void MoveLeft()
    {
        moveRequest = -1;

        // FlipModel(moveRequest);
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

        // Check if wallsliding rn
        if (onWallThisFrame != 0)
        {
            // Initate walljump
            walljumpRequest = true;
        }
    }

    public void EndJumpEarly()
    {
        // Set flag
        endJumpRequest = true;
    }

    public bool IsRunning() => Mathf.Abs(currentVelocity.x) > 0.1f;
    public bool IsGrounded() => groundedThisFrame;
    public bool IsWallSliding() => isWallSliding;
    public bool IsTouchingLedge() => grabbingLedge;
    public bool IsRising() => body.velocity.y > 0.1f;
    public bool IsFalling() => body.velocity.y < -0.1f;
    public bool IsMantle() => canMantle;
    public bool IsCrouching() => crouchRequest;

    public void PerformMantle()
    {
        // If you are mantling
        if (canMantle)
        {
            // Move model to top

            // Get corner of ledge check based on which wall u are on
            var corner = hurtBox.bounds.center + new Vector3(onWallThisFrame * hurtBox.bounds.extents.x, hurtBox.bounds.extents.y, 0f);

            // Get offset
            var offset = new Vector3(onWallThisFrame * hurtBox.bounds.extents.x, hurtBox.bounds.extents.y, 0f);

            transform.position = corner + offset;
            
            // Disable flag
            grabbingLedge = false;
            canMantle = false;

            // Flip model
            FlipModel(-moveRequest);
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
        // Make sure you are not grounded
        if (!groundedThisFrame)
        {
            // If you are on a ledge
            if (grabbingLedge)
            {
                // You are moving in the same direction as the wall and jumping
                if (WithinBufferTime() && onWallThisFrame == moveRequest)
                {
                    // Allow mantle
                    canMantle = true;
                }
            }
            

            // On wall but no ledge last frame
            if (onWallLastFrame != 0 && !onLedgeLastFrame)
            {
                // You are on wall now, but no ledge
                if (onWallThisFrame != 0 && onLedgeThisFrame)
                {
                    // Grab ledge
                    grabbingLedge = true;

                    // Stop wall sliding
                    isWallSliding = false;
                }
            }

            if (!grabbingLedge && onWallThisFrame != 0 && onWallThisFrame == moveRequest && onLedgeThisFrame)
            {
                // Start wall sliding
                isWallSliding = true;
            }
            else
            {
                isWallSliding = false;
            }
        }
    }

    private void HandleHorizontalMovement()
    {
        // Check if player is on wall
        if (isWallSliding || grabbingLedge || canMantle)
        {
            currentVelocity.x = 0f;
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
                targetSpeed = moveRequest * maxCrouchwalkSpeed;
            }
            else 
            {
                // Calculate target speed
                targetSpeed = moveRequest * maxRunSpeed;
            }

            // Accelerate
            currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, targetSpeed, acceleration * Time.deltaTime);
        }
        else
        {
            // Check whether to use ground friction or air friction
            if (groundedThisFrame)
            {
                // Decelerate to 0
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0, groundDeceleration * Time.deltaTime);
            }
            else
            {
                // Decelerate to 0
                currentVelocity.x = Mathf.MoveTowards(currentVelocity.x, 0, airDeceleration * Time.deltaTime);
            }

        }
    }

    private void HandleJumping()
    {
        // Make sure you are within buffer time
        if (!canMantle && WithinBufferTime())
        {
            // Debug
            // print(Time.time - jumpInputTime);

            // Make sure you are grounded OR within coyote time
            if (groundedThisFrame || WithinCoyoteTime())
            {
                // Debug
                // print(Time.time - timeSinceGrounded);

                // Add jumpower
                currentVelocity.y = jumpPower;

                // Reset times
                jumpInputTime = -1f;
                timeSinceGrounded = -1f;

                // Set flag
                isJumping = true;
                grabbingLedge = false;
                isWallSliding = false;
            }
            // If you are on a wall, then wall jump instead
            else if (onWallThisFrame != 0 || WithinCoyoteTime())
            {
                // Wall jump
                currentVelocity = Vector2.Scale(walljumpPower, new Vector2(-onWallThisFrame, 1));

                // Debug
                print("WALLJUMP: " + currentVelocity);

                // Reset times
                jumpInputTime = -1f;
                timeSinceGrounded = -1f;

                // Reset
                // onWallThisFrame = 0;

                // Set flag
                isJumping = true;
                grabbingLedge = false;
                isWallSliding = false;
            }
        }

        // Check if jump is ended early
        if (endJumpRequest && isJumping)
        {
            // Reduce current velocity by a factor
            currentVelocity.y /= earlyCancelFactor;

            // Set flags
            endJumpRequest = false;
            isJumping = false;
        }
    }

    private void HandleVerticalMovement()
    {
        // If you are airborne
        if (!groundedThisFrame)
        {
            // If you are grabbing ledge
            if (grabbingLedge) 
            {
                // Don't move
                currentVelocity.y = 0;

                return;
            }

            // Apply different gravity based on state
            if (IsRising())
            {
                // Apply weaker gravity
                currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, -maxFallSpeed, risingGravity * Time.deltaTime);
            }
            // If you are falling
            else 
            {
                // Check if you are moving into the same side as the wall
                if (isWallSliding)
                {
                    // Start wallsliding
                    currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, -maxWallslideSpeed, wallSlideAcceleration * Time.deltaTime);
                }
                else 
                {
                    // Apply stronger gravity
                    currentVelocity.y = Mathf.MoveTowards(currentVelocity.y, -maxFallSpeed, fallingGravity * Time.deltaTime);
                }
            }
        }

        // If you were airborne and now are not
        if (!groundedLastFrame && groundedThisFrame)
        {
            // Remove y vel
            currentVelocity.y = 0f;
        }
    }

    private bool WithinBufferTime() => Time.time - jumpInputTime <= jumpBuffer;
    private bool WithinCoyoteTime() => Time.time - timeSinceGrounded <= coyoteTime;

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
        // if (wallHangThisFrame) return true;

        // Look for ledge
        var position = hurtBox.bounds.center + new Vector3(0f, hurtBox.bounds.extents.y, 0f);
        var size = new Vector2(hurtBox.bounds.size.x + checkThickness, checkThickness);
        var hitLedge = Physics2D.OverlapBox(position, size, 0, groundLayer);

        return hitLedge;
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
    }
}
