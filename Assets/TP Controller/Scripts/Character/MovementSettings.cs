using System;
using UnityEngine;

[Serializable]
public class MovementSettings
{
    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float acceleration = 10f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float walkSpeed = 2f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float jogSpeed = 4f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float sprintSpeed = 6f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float jumpAcceleration = 40f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float maxJumpSpeed = 7.5f;
    
    public float Acceleration
    {
        get
        {
            return this.acceleration;
        }
        set
        {
            this.acceleration = value;
        }
    }

    public float WalkSpeed
    {
        get
        {
            return this.walkSpeed;
        }
        set
        {
            this.walkSpeed = value;
        }
    }

    public float JogSpeed
    {
        get
        {
            return this.jogSpeed;
        }
        set
        {
            this.jogSpeed = value;
        }
    }

    public float SprintSpeed
    {
        get
        {
            return this.sprintSpeed;
        }
        set
        {
            this.sprintSpeed = value;
        }
    }

    public float JumpAcceleration
    {
        get
        {
            return this.jumpAcceleration;
        }
        set
        {
            this.jumpAcceleration = value;
        }
    }

    public float MaxJumpSpeed
    {
        get
        {
            return this.maxJumpSpeed;
        }
        set
        {
            this.maxJumpSpeed = value;
        }
    }
}
