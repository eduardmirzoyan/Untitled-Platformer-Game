using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Run, Rise, Fall, Crouch, Crouchwalk, Wallslide, Walljump, Wallhang, Mantle };

    [Header("Components")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private AnimationHandler animationHandler;

    [Header("Data")]
    [SerializeField] private PlayerState playerState;

    private void Awake()
    {
        // Get refs
        inputHandler = GetComponent<InputHandler>();
        movementHandler = GetComponent<MovementHandler>();
        animationHandler = GetComponent<AnimationHandler>();

        // Set starting state
        playerState = PlayerState.Idle;
    }

    // Update is called once per frame
    void Update()
    {
        switch (playerState)
        {
            case PlayerState.Idle:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Handle crouching
                HandleCrouching();

                // Check for running
                if (movementHandler.IsRunning())
                {
                    // Change animation
                    animationHandler.changeAnimation("Run");

                    // Change states
                    playerState = PlayerState.Run;
                }

                // Check for crouching
                if (inputHandler.GetCrouchKey())
                {
                    // Enable crouch
                    movementHandler.StartCrouch();

                    // Change animation
                    animationHandler.changeAnimation("Crouch");

                    // Change states
                    playerState = PlayerState.Crouch;
                }

                // Check for jumping
                if (!movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.changeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change animation
                    animationHandler.changeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                break;
            case PlayerState.Run:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Handle crouching
                HandleCrouching();

                // Check for idling
                if (!movementHandler.IsRunning())
                {
                    // Change animation
                    animationHandler.changeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                // Check for crouch walk
                if (movementHandler.IsCrouching())
                {
                    // Enable crouch
                    // movementHandler.StartCrouch();

                    // Change animation
                    animationHandler.changeAnimation("Crouch Walk");

                    // Change states
                    playerState = PlayerState.Crouchwalk;
                }

                if (!movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.changeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change animation
                    animationHandler.changeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                break;
            case PlayerState.Rise:

                // Handle running
                HandleRunning();

                // Handle jump cancel
                if (inputHandler.GetJumpInputUp()) movementHandler.EndJumpEarly();

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change animation
                    animationHandler.changeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                // Check for ledge
                if (movementHandler.IsTouchingLedge())
                {
                    // Change animation
                    animationHandler.changeAnimation("Hang");

                    // Change states
                    playerState = PlayerState.Wallhang;
                }

                break;
            case PlayerState.Fall:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Check for idling
                if (movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.changeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                // Check for coyote jumps
                if (movementHandler.IsRising())
                {
                    // Change animation
                    animationHandler.changeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for ledge
                if (movementHandler.IsTouchingLedge())
                {
                    // Change animation
                    animationHandler.changeAnimation("Hang");

                    // Change states
                    playerState = PlayerState.Wallhang;
                }

                // Handle wall sliding
                if (movementHandler.IsWallSliding() && inputHandler.GetMoveInput())
                {
                    // Change animation
                    animationHandler.changeAnimation("Wallslide");

                    // Change states
                    playerState = PlayerState.Wallslide;
                }

                break;
            case PlayerState.Crouch:

                // Handle running
                HandleRunning();

                // Handle crouching
                HandleCrouching();

                // Check for idling
                if (!movementHandler.IsCrouching())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change animation
                    animationHandler.changeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                // Check for crouch walk
                if (movementHandler.IsRunning())
                {
                    // Change animation
                    animationHandler.changeAnimation("Crouch Walk");

                    // Change states
                    playerState = PlayerState.Crouchwalk;
                }
                
                break;
            case PlayerState.Crouchwalk:

                // Handle running
                HandleRunning();

                // Handle crouching
                HandleCrouching();

                // Check for crouch
                if (!movementHandler.IsRunning())
                {
                    // Change animation
                    animationHandler.changeAnimation("Crouch");

                    // Change states
                    playerState = PlayerState.Crouch;
                }

                // Check for running
                if (!movementHandler.IsCrouching())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change animation
                    animationHandler.changeAnimation("Run");

                    // Change states
                    playerState = PlayerState.Run;
                }

                // Check for falling
                if (!movementHandler.IsGrounded())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change animation
                    animationHandler.changeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                break;
            case PlayerState.Wallslide:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Check for jump
                if (movementHandler.IsRising())
                {
                    // Change animation
                    animationHandler.changeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for fall
                if (!inputHandler.GetMoveInput())
                {
                    // Change animation
                    animationHandler.changeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }
                
                // Check for idle
                if (movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.changeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                break;
            case PlayerState.Walljump:
                // TODO?

                break;
            case PlayerState.Wallhang:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Check for jump
                if (movementHandler.IsRising())
                {
                    // Change animation
                    animationHandler.changeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                if (movementHandler.CanMantle())
                {
                    // Change animation
                    animationHandler.changeAnimation("Mantle");

                    // Change states
                    playerState = PlayerState.Mantle;
                }

                // TODO
                break;
            case PlayerState.Mantle:
                // TODO

                if (animationHandler.IsFinished())
                {
                    // Move model
                    movementHandler.PerformMantle();

                    // Change animation
                    animationHandler.changeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                break;
            default:
                // Throw error
                throw new System.Exception("STATE NOT IMPLEMENTED.");
        }
    }

    private void HandleRunning()
    {
        if (inputHandler.GetLeftInput()) movementHandler.MoveLeft();
        else if (inputHandler.GetRightInput()) movementHandler.MoveRight();
        else movementHandler.Stop();
    }

    private void HandleJumping()
    {
        if (inputHandler.GetJumpInputDown()) movementHandler.Jump();
    }

    public void HandleCrouching()
    {
        if (inputHandler.GetCrouchKey()) movementHandler.StartCrouch();
        else movementHandler.EndCrouch();
    }
}
