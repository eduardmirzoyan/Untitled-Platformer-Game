using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private enum PlayerState { Idle, Run, Rise, Fall };

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
                // Handle movement
                if (inputHandler.GetLeftInput()) movementHandler.MoveLeft();
                else if (inputHandler.GetRightInput()) movementHandler.MoveRight();
                else movementHandler.Stop();

                // Handle jumping
                if (inputHandler.GetJumpInput()) movementHandler.Jump();

                // Check for running
                if (movementHandler.IsRunning())
                {
                    // Change animation
                    animationHandler.changeAnimation("Run");

                    // Change states
                    playerState = PlayerState.Run;
                }

                // Check for jumping
                if (!movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.changeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                break;
            case PlayerState.Run:
                // Handle movement
                if (inputHandler.GetLeftInput()) movementHandler.MoveLeft();
                else if (inputHandler.GetRightInput()) movementHandler.MoveRight();
                else movementHandler.Stop();

                // Handle jumping
                if (inputHandler.GetJumpInput()) movementHandler.Jump();

                // Check for stopping
                if (!movementHandler.IsRunning())
                {
                    // Change animation
                    animationHandler.changeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                if (!movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.changeAnimation("Rise");

                    // Change states
                    playerState = PlayerState.Rise;
                }

                break;
            case PlayerState.Rise:
                // Handle movement
                if (inputHandler.GetLeftInput()) movementHandler.MoveLeft();
                else if (inputHandler.GetRightInput()) movementHandler.MoveRight();
                else movementHandler.Stop();

                if (inputHandler.GetJumpInputUp()) movementHandler.CancelJump();

                if (movementHandler.IsFalling())
                {
                    // Change animation
                    animationHandler.changeAnimation("Fall");

                    // Change states
                    playerState = PlayerState.Fall;
                }

                

                break;
            case PlayerState.Fall:
                // Handle movement
                if (inputHandler.GetLeftInput()) movementHandler.MoveLeft();
                else if (inputHandler.GetRightInput()) movementHandler.MoveRight();
                else movementHandler.Stop();

                if (movementHandler.IsGrounded())
                {
                    // Change animation
                    animationHandler.changeAnimation("Idle");

                    // Change states
                    playerState = PlayerState.Idle;
                }

                break;
            default:
                // Throw error
                throw new System.Exception("STATE NOT FOUND");
        }
    }
}
