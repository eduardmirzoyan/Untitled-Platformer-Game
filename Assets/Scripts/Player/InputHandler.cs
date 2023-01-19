using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : MonoBehaviour
{
    [Header("Data")]
    [SerializeField] private Controls controls;

    public bool GetRightInput() => Input.GetKey(controls.moveRightKey);

    public bool GetLeftInput() => Input.GetKey(controls.moveLeftKey);

    public bool GetMoveInput() => GetRightInput() || GetLeftInput();

    public bool GetJumpInputDown() => Input.GetKeyDown(controls.jumpKey);

    public bool GetJumpInputUp() => Input.GetKeyUp(controls.jumpKey);

    public bool GetCrouchKey() => Input.GetKey(controls.crouchKey);
    
    public bool GetInteractKeyDown() => Input.GetKeyDown(controls.interactKey);
    
    public bool GetRollKeyDown() => Input.GetKeyDown(controls.rollKey);
}
