using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DebugConstants
{
    [Header("Physics")]
    [Tooltip("Radius of the sphere collider.")]
    public float colliderRadius = 1.1f;
    [Tooltip("Mass of the rigid body.")]
    public float bodyMass = 10f;

    [Tooltip("Gravity speed.")]
    public float gravityVelocity = 80;
    [Tooltip("Maximum gravity.")]
    public float maxGravity = 70;
    [Tooltip("Maximum angle of climbable slopes.")]
    public float maxSlopeAngle = 60f;
    [Tooltip("Amount of friction applied when colliding with a wall.")]
    public float sideFriction = 0.3f;

    [Tooltip("Maximum acceleration when going forward.")]
    public float maxAccelerationForward = 40;
    [Tooltip("Maximum speed when going forward.")]
    public float maxSpeedForward = 90;
    [Tooltip("Maximum acceleration when going in reverse.")]
    public float maxAccelerationReverse = 50;
    [Tooltip("Maximum speed when going in reverse.")]
    public float maxSpeedReverse = 20;
    [Tooltip("How fast the car will brake when the motor goes in the opposite direction.")]
    public float brakeStrength = 50;
    [Tooltip("How much friction to apply when on a slope. The higher this value, the slower you'll climb up slopes and the faster you'll go down. Setting this to zero adds no additional friction.")]
    public float slopeFriction = 0.8f;

    [Header("Steering")]
    [Tooltip("Sharpness of the steering.")]
    public float maxSteering = 100;
    [Tooltip("Multiplier applied to steering when in the air. Setting this to zero makes the car unsteerable in the air.")]
    public float steeringMultiplierInAir = 0.25f;

    [Tooltip("How fast the car stops when releasing the gas.")]
    public float forwardFriction = 15;
    [Tooltip("How much grip the car should have on the road when turning.")]
    public float lateralFriction = 55;
}
