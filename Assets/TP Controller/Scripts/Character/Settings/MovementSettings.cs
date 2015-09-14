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
    [Tooltip("Force impulse, [0, Infinity)")]
    private float jumpForce = 10f;
    
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

    public float JumpForce
    {
        get
        {
            return this.jumpForce;
        }
        set
        {
            this.jumpForce = value;
        }
    }
}
