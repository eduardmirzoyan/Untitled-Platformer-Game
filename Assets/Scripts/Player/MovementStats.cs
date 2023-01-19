using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu]
public class MovementStats : ScriptableObject
{
    [Header("Runing")]
    public float maxRunSpeed = 5f;
    public float acceleration = 60f;
    public float groundDeceleration = 60f;
    public float airDeceleration = 30f;

    [Header("Jumping")]
    public float jumpPower = 14f;
    public float earlyCancelFactor = 2.5f;
    public float coyoteTime = 0.1f;
    public float jumpBuffer = 0.15f;

    [Header("Crouching")]
    public float maxCrouchWalkSpeed = 2f;

    [Header("Wallsliding")]
    public float maxWallSlideSpeed = 2f;
    public float wallSlideAcceleration = 30f;
    public Vector2 wallJumpPower = new Vector2(10, 10);

    [Header("Gravity")]
    public float risingGravity = 40f;
    public float fallingGravity = 60f;
    public float maxFallSpeed = 40;

    [Header("Rolling")]
    public float rollDuration = 0.5f;
    public float maxRollSpeed = 6f;
    
    [Header("Extras")]
    public float deathDeceleration = 30f;
}
