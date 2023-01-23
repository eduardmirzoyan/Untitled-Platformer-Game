using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Run, Rise, Fall, Crouch, Crouchwalk, Wallslide, Wallhang, Mantle, Exit, Enter, Dead, Invisible, Roll };

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
            playerState = PlayerState.Enter;
            animationHandler.ChangeAnimation(playerState.ToString());
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

            // Stop sound
            AudioManager.instance.Stop("Run");

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

                // Handle rolling
                HandleRolling();

                // Check for interacting
                if (interactionHandler.CanExit())
                {
                    // Stop moving
                    movementHandler.Stop();

                    // Don't get hit
                    damageHandler.SetInvincible(true);

                    // Trigger event
                    LevelManager.instance.ExitLevel();

                    // Change states
                    playerState = PlayerState.Exit;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for running
                if (movementHandler.IsRunning())
                {
                    // Play particles
                    juiceHandler.ToggleRunning(true);

                    // Play sound
                    AudioManager.instance.Play("Run");

                    // Change states
                    playerState = PlayerState.Run;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for crouching
                if (movementHandler.IsCrouching())
                {
                    // Enable crouch
                    movementHandler.StartCrouch();;

                    // Change states
                    playerState = PlayerState.Crouch;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for jumping
                if (!movementHandler.IsGrounded())
                {
                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for rolling
                if (movementHandler.IsRolling())
                {
                    // Make invicible
                    damageHandler.SetInvincible(true);

                    // Play sound
                    AudioManager.instance.Play("Roll");

                    // Change states
                    playerState = PlayerState.Roll;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
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

                // Handle rolling
                HandleRolling();

                // Check for interacting
                if (interactionHandler.CanExit())
                {
                    // Stop moving
                    movementHandler.Stop();

                    // Don't get hit
                    damageHandler.SetInvincible(true);
                    
                    // Stop particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change states
                    playerState = PlayerState.Exit;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());

                    // Trigger event
                    LevelManager.instance.ExitLevel();
                }

                // Check for idling
                if (!movementHandler.IsRunning())
                {
                    // Stop particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for crouch walk
                if (movementHandler.IsCrouching())
                {
                    // Stop particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change states
                    playerState = PlayerState.Crouchwalk;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for jump
                if (movementHandler.IsRising())
                {
                    // Stop particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Stop particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for rolling
                if (movementHandler.IsRolling())
                {
                    // Stop particles
                    juiceHandler.ToggleRunning(false);

                    // Stop sound
                    AudioManager.instance.Stop("Run");

                    // Make invicible
                    damageHandler.SetInvincible(true);

                    // Play sound
                    AudioManager.instance.Play("Roll");

                    // Change states
                    playerState = PlayerState.Roll;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Rise:

                // Handle running
                HandleRunning();

                // Handle jumping
                HandleJumping();

                // Handle jump cancel
                if (inputHandler.GetJumpInputUp()) movementHandler.EndJumpEarly();

                // Check for landing
                if (movementHandler.IsGrounded())
                {
                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
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

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for coyote jumps
                if (movementHandler.IsRising())
                {
                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for ledge
                if (movementHandler.IsTouchingLedge())
                {
                    // Play sound
                    AudioManager.instance.Play("Grab Ledge");

                    // Change states
                    playerState = PlayerState.Wallhang;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Handle wall sliding
                if (movementHandler.IsWallSliding())
                {
                    // Play particles
                    juiceHandler.ToggleSlide(true);

                    // Change states
                    playerState = PlayerState.Wallslide;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Crouch:

                // Handle running
                HandleRunning();

                // Handle crouching
                HandleCrouching();

                // Handle dropping
                HandleDropping();

                // Check for idling
                if (!movementHandler.IsCrouching())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for crouch walk
                if (movementHandler.IsRunning())
                {
                    // Change states
                    playerState = PlayerState.Crouchwalk;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for dropping
                if (movementHandler.IsFalling())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Crouchwalk:

                // Handle running
                HandleRunning();

                // Handle crouching
                HandleCrouching();

                // Handle dropping
                HandleDropping();

                // Check for crouch
                if (!movementHandler.IsRunning())
                {
                    // Change states
                    playerState = PlayerState.Crouch;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
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

                    // Change states
                    playerState = PlayerState.Run;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for falling
                if (movementHandler.IsFalling())
                {
                    // Disable crouch
                    movementHandler.EndCrouch();

                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
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
                    // Stop particles
                    juiceHandler.ToggleSlide(false);

                    // Play sound
                    AudioManager.instance.Play("Jump");

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for fall
                if (!movementHandler.IsWallSliding())
                {
                    // Stop particles
                    juiceHandler.ToggleSlide(false);

                    // Change states
                    playerState = PlayerState.Fall;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for idle
                if (movementHandler.IsGrounded())
                {
                    // Stop particles
                    juiceHandler.ToggleSlide(false);

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
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

                    // Change states
                    playerState = PlayerState.Rise;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                // Check for mantle
                if (movementHandler.IsMantling())
                {
                    // Change states
                    playerState = PlayerState.Mantle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Mantle:

                // Handle running
                HandleRunning();

                // Wait til animation is over
                if (animationHandler.IsFinished())
                {
                    // Move model
                    movementHandler.PerformMantle();

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Roll:

                // TODO

                // Wait til animation is over
                if (animationHandler.IsFinished())
                {
                    // Disable invicible
                    damageHandler.SetInvincible(false);

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Exit:

                // When animation is over
                if (animationHandler.IsFinished())
                {
                    // Change states
                    playerState = PlayerState.Invisible;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
                }

                break;
            case PlayerState.Enter:

                // When animation is over...
                if (animationHandler.IsFinished())
                {
                    // Trigger event
                    LevelEvents.instance.TriggerOnLockEntrance();

                    // Change states
                    playerState = PlayerState.Idle;

                    // Change animation
                    animationHandler.ChangeAnimation(playerState.ToString());
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

    private void HandleDropping()
    {
        if (inputHandler.GetCrouchKey() && inputHandler.GetJumpInputDown()) movementHandler.Drop();
    }

    private void HandleRolling()
    {
        if (inputHandler.GetRollKeyDown()) movementHandler.Roll();
    }
}
