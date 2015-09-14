using System;
using UnityEngine;

[Serializable]
public class GravitySettings
{
    [SerializeField]
    [Tooltip("Acceleration, [0, Infinity)")]
    private float gravityStrength = 27f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float maxFallSpeed = 38f;

    [SerializeField]
    [Tooltip("The constant gravity applied to the character when he is grounded, [0, Infinity)")]
    private float groundedGravityForce = 9f;

    public float GravityStrength
    {
        get
        {
            return this.gravityStrength;
        }
        set
        {
            this.gravityStrength = value;
        }
    }

    public float MaxFallSpeed
    {
        get
        {
            return this.maxFallSpeed;
        }
        set
        {
            this.maxFallSpeed = value;
        }
    }

    public float GroundedGravityForce
    {
        get
        {
            return groundedGravityForce;
        }
        set
        {
            this.groundedGravityForce = value;
        }
    }
}
