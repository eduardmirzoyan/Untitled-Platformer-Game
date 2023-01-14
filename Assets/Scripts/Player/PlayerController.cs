using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Run, Rise, Fall, Crouch, Crouchwalk, Wallslide, Wallhang, Mantle, Exiting, Entering, Dead, Invisible };

    [Header("Components")]
    [SerializeField] private InputHandler inputHandler;
    [SerializeField] private MovementHandler movementHandler;
    [SerializeField] private AnimationHandler animationHandler;
    [SerializeField] private InteractionHandler interactionHandler;
    [SerializeField] private DamageHandler damageHandler;
    [SerializeField] private JuiceHandler juiceHandler;

    [Header("Data")]
    [SerializeField] private PlayerState playerState;

    private void Awake()
    {
        // Get refs
        inputHandler = GetComponent<InputHandler>();
        movementHandler = GetComponent<MovementHandler>();
        animationHandler = GetComponent<AnimationHandler>();
        interactionHandler = GetComponent<InteractionHandler>();
        damageHandler = GetComponent<DamageHandler>();
        juiceHandler = GetComponent<JuiceHandler>();

        // Starting state should be invisible
        playerState = PlayerState.Invisible;
    }

    private void Start()
    {
        // Sub
        LevelEvents.instance.onLevelEnter += EnterLevel;
    }

    private void OnDestroy()
    {
        // Unsub
        LevelEvents.instance.onLevelEnter -= EnterLevel;
    }

    private void EnterLevel(Transform playerTransform)
    {
        if (this.transform == playerTransform)
        {
            // Change states
            playerState = PlayerState.Entering;
            animationHandler.ChangeAnimation("Enter");
        }
        
    }

    // Update is called once per frame
    void Update()
    {
        // Check for death
        if (playerState != PlayerState.Dead && damageHandler.IsHurt())
        {
            // Stop moving
            movementHandler.Die();

            // Change animation
            animationHandler.ChangeAnimation("Dead");

            // Play sound
            AudioManager.instance.Play("Hurt");

            // Trigger event
            LevelEvents.instance.TriggerOnPlayerDeath();

            // Change states
            playerState = PlayerState.Dead;
        }

        switch (playerState)
        {
            case PlayerState.Idle:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Handle crouching
                HandleCrouching();

                // Handle interacting
                HandleInteracting();

                // Check for interacting
                if (interactionHandler.CanExit())
                {
                    // Stop moving
                    movementHandler.Stop();

                    // Don't get hit
                    damageHandler.SetInvincible();

                    // Trigger event
                    LevelManager.instance.ExitLevel();

                    // Change animation
                    animationHandler.ChangeAnimation("Exit");

                    // Change states
                    playerState = PlayerState.Exiting;
                }

                // Check for running
                if (movementHandler.IsRunning())
                {
                    // Play particles
                    juiceHandler.ToggleRunning(true);

                    // Play sound
                    AudioManager.instance.Play("Run");

                    // Change animation
                    animationHandler.ChangeAnimation("Run");

                    // Change states
                    playerState = PlayerState.Run;
                }

                // Check for crouching
                if (movementHandler.IsCrouching())
                {
                    // Enable crouch
                    movementHandler.StartCrouch();

                    // Change animation
                    animationHandler.ChangeAnimation("Crouch");

                    // Change states
                    playerState = PlayerState.Crouch;
                }

                // Check for jumping
                if (!movementHandler.IsGrounded())
                {
                    // Play particles
                    juiceHandler.PlayJump();

                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change animation
                    animationHandler.ChangeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Fall");

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

                // Handle interacting
                HandleInteracting();

                // Check for interacting
                if (interactionHandler.CanExit())
                {
                    // Stop moving
                    movementHandler.Stop();

                    // Don't get hit
                    damageHandler.SetInvincible();
                    
                    // Play particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change animation
                    animationHandler.ChangeAnimation("Exit");

                    // Trigger event
                    LevelManager.instance.ExitLevel();


                    // Change states
                    playerState = PlayerState.Exiting;
                }

                // Check for idling
                if (!movementHandler.IsRunning())
                {
                    // Play particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change animation
                    animationHandler.ChangeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                // Check for crouch walk
                if (movementHandler.IsCrouching())
                {
                    // Play particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change animation
                    animationHandler.ChangeAnimation("Crouch Walk");

                    // Change states
                    playerState = PlayerState.Crouchwalk;
                }

                // Check for jump
                if (movementHandler.IsRising())
                {
                    // Play particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Play particles
                    juiceHandler.PlayJump();

                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change animation
                    animationHandler.ChangeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Play particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change animation
                    animationHandler.ChangeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                break;
            case PlayerState.Rise:

                // Handle running
                HandleRunning();

                // Handle jump cancel
                if (inputHandler.GetJumpInputUp()) movementHandler.EndJumpEarly();

                // Check for landing
                if (movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                break;
            case PlayerState.Fall:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Check for landing
                if (movementHandler.IsGrounded())
                {
                    // Play particles
                    juiceHandler.PlayLand();

                    // Play sound
                    AudioManager.instance.Play("Land");

                    // Change animation
                    animationHandler.ChangeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                // Check for coyote jumps
                if (movementHandler.IsRising())
                {
                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change animation
                    animationHandler.ChangeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for ledge
                if (movementHandler.IsTouchingLedge())
                {
                    // Play sound
                    AudioManager.instance.Play("Grab Ledge");

                    // Change animation
                    animationHandler.ChangeAnimation("Hang");

                    // Change states
                    playerState = PlayerState.Wallhang;
                }

                // Handle wall sliding
                if (movementHandler.IsWallSliding())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Wallslide");

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
                    animationHandler.ChangeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                // Check for crouch walk
                if (movementHandler.IsRunning())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Crouch Walk");

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
                    animationHandler.ChangeAnimation("Crouch");

                    // Change states
                    playerState = PlayerState.Crouch;
                }

                // Check for running
                if (!movementHandler.IsCrouching())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Play particles
                    juiceHandler.ToggleRunning(true);

                    // Play sound
                    AudioManager.instance.Play("Run");

                    // Change animation
                    animationHandler.ChangeAnimation("Run");

                    // Change states
                    playerState = PlayerState.Run;
                }

                // Check for falling
                if (!movementHandler.IsGrounded())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change animation
                    animationHandler.ChangeAnimation("Fall");

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
                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change animation
                    animationHandler.ChangeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for fall
                if (!movementHandler.IsWallSliding())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                // Check for idle
                if (movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                break;
            case PlayerState.Wallhang:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Check for jump
                if (movementHandler.IsRising())
                {
                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change animation
                    animationHandler.ChangeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                // Check for mantle
                if (movementHandler.IsMantling())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Mantle");

                    // Change states
                    playerState = PlayerState.Mantle;
                }

                break;
            case PlayerState.Mantle:

                // Wait til animation is over
                if (animationHandler.IsFinished())
                {
                    // Move model
                    movementHandler.PerformMantle();

                    // Change animation
                    animationHandler.ChangeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                break;
            case PlayerState.Exiting:

                // When animation is over
                if (animationHandler.IsFinished())
                {
                    // Change animation
                    animationHandler.ChangeAnimation("Invisible");

                    // Change states
                    playerState = PlayerState.Invisible;
                }

                break;
            case PlayerState.Entering:

                // When animation is over...
                if (animationHandler.IsFinished())
                {
                    // Trigger event
                    LevelEvents.instance.TriggerOnLockEntrance();

                    // Change animation
                    animationHandler.ChangeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                break;
            case PlayerState.Dead:

                // Do nothing...

                break;
            case PlayerState.Invisible:
                
                // Do nothing...

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

    private void HandleCrouching()
    {
        if (inputHandler.GetCrouchKey()) movementHandler.StartCrouch();
        else movementHandler.EndCrouch();
    }

    private void HandleInteracting()
    {
        if (inputHandler.GetInteractKeyDown()) interactionHandler.InteractWithSurroundings();
    }
}
