using UnityEngine;
using System.Collections;

[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterInputController))]
public class Character : MonoBehaviour
{
    public const float MinMoveSpeed = 0f;
    public const float MaxMoveSpeed = 10f;
    public const float MinRotationSmoothing = 0f;
    public const float MaxRotationSmoothing = 30f;

    [SerializeField]
    [Range(1f, 30f)]
    [Tooltip("How fast the character accelerates")]
    private float speedAcceleration = 10f;

    [SerializeField]
    [Range(MinMoveSpeed, MaxMoveSpeed)]
    [Tooltip("In meters/second")]
    private float walkSpeed = 2f; // In meters per second

    [SerializeField]
    [Range(MinMoveSpeed, MaxMoveSpeed)]
    [Tooltip("In meters/second")]
    private float jogSpeed = 4f; // In meters per second

    [SerializeField]
    [Range(MinMoveSpeed, MaxMoveSpeed)]
    [Tooltip("In meters/second")]
    private float sprintSpeed = 6f; // In meters per seconds

    [SerializeField]
    [Range(MinRotationSmoothing, MaxRotationSmoothing)]
    [Tooltip("How fast the character rotates around the Y axis")]
    private float rotationSmoothing = 15f;

    private bool orientRotationToMovement;
    private bool useControlRotation;
    private bool isWalking;
    private bool isJogging;
    private bool isSprinting;
    private bool isJumping;
    private Vector3 moveVector;
    private float maxMoveSpeed; // In meters per second
    private float targetMoveSpeed; // In meters per second
    private float currentMoveSpeed; // In meters per second
    private CharacterController controller;
    private Quaternion controlRotation;
    private Quaternion controlRotationX;
    private Quaternion controlRotationY;

    protected virtual void Awake()
    {
        this.WalkSpeed = this.WalkSpeed;
        this.JogSpeed = this.JogSpeed;
        this.SprintSpeed = this.SprintSpeed;
        this.IsJogging = true;
        this.OrientRotationToMovement = true;

        this.controller = this.GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        this.AccelerateMoveSpeed();
        this.AlignRotationWithControlRotationY();
    }

    public float WalkSpeed
    {
        get
        {
            return this.walkSpeed;
        }
        set
        {
            this.walkSpeed = Mathf.Clamp(value, MinMoveSpeed, MaxMoveSpeed);
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
            this.jogSpeed = Mathf.Clamp(value, MinMoveSpeed, MaxMoveSpeed);
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
            this.sprintSpeed = Mathf.Clamp(value, MinMoveSpeed, MaxMoveSpeed);
        }
    }

    /// <summary>
    /// If set to true, this automatically sets B_UseControlRotation to false
    /// </summary>
    public bool OrientRotationToMovement
    {
        get
        {
            return this.orientRotationToMovement;
        }
        set
        {
            this.orientRotationToMovement = value;
            if (this.orientRotationToMovement)
            {
                this.useControlRotation = false;
            }
        }
    }

    /// <summary>
    /// If set to true, this automatically sets B_OrientRotationToMovement to false
    /// </summary>
    public bool UseControlRotation
    {
        get
        {
            return this.useControlRotation;
        }
        set
        {
            this.useControlRotation = value;
            if (this.useControlRotation)
            {
                this.orientRotationToMovement = false;
            }
        }
    }

    public Quaternion ControlRotation
    {
        get
        {
            return this.controlRotation;
        }
        set
        {
            this.controlRotation = value;
        }
    }

    public Quaternion ControlRotationX
    {
        get
        {
            return this.controlRotationX;
        }
        set
        {
            this.controlRotationX = value;
        }
    }

    public Quaternion ControlRotationY
    {
        get
        {
            return this.controlRotationY;
        }
        set
        {
            this.controlRotationY = value;
        }
    }

    public bool IsWalking
    {
        get
        {
            return this.isWalking;
        }
        set
        {
            this.isWalking = value;
            if (this.isWalking)
            {
                this.maxMoveSpeed = this.WalkSpeed;
                this.IsJogging = false;
                this.IsSprinting = false;
            }
        }
    }

    public bool IsJogging
    {
        get
        {
            return this.isJogging;
        }
        set
        {
            this.isJogging = value;
            if (this.isJogging)
            {
                this.maxMoveSpeed = this.JogSpeed;
                this.IsWalking = false;
                this.IsSprinting = false;
            }
        }
    }

    public bool IsSprinting
    {
        get
        {
            return this.isSprinting;
        }
        set
        {
            this.isSprinting = value;
            if (this.isSprinting)
            {
                this.maxMoveSpeed = this.SprintSpeed;
                this.IsWalking = false;
                this.IsJogging = false;
            }
        }
    }

    public bool IsJumping
    {
        get
        {
            return this.isJumping;
        }
        set
        {
            this.isJumping = value;
        }
    }

    public bool IsGrounded
    {
        get
        {
            return this.controller.isGrounded;
        }
    }

    public Vector3 Velocity
    {
        get
        {
            return this.controller.velocity;
        }
    }

    public Vector3 HorizontalVelocity
    {
        get
        {
            return new Vector3(this.Velocity.x, 0f, this.Velocity.z);
        }
    }

    public void Move(Vector3 moveVector)
    {
        this.OrientRotationToMoveVector(moveVector);

        float moveSpeed = moveVector.magnitude * this.maxMoveSpeed;
        if (moveSpeed < float.Epsilon)
        {
            moveVector = this.moveVector;
            this.targetMoveSpeed = 0f;
        }
        else if (moveSpeed <= this.WalkSpeed)
        {
            this.targetMoveSpeed = this.WalkSpeed;
        }
        else if (moveSpeed > this.WalkSpeed && moveSpeed <= this.JogSpeed)
        {
            this.targetMoveSpeed = this.JogSpeed;
        }
        else if (moveSpeed > this.JogSpeed)
        {
            this.targetMoveSpeed = this.SprintSpeed;
        }

        if (moveSpeed > 0f && Mathf.Abs(moveSpeed - this.maxMoveSpeed) > 0.1f)
        {
            moveVector.Normalize();
        }

        Vector3 motion = moveVector * this.currentMoveSpeed * Time.deltaTime;
        this.controller.Move(motion);

        this.moveVector = moveVector;
    }

    private bool OrientRotationToMoveVector(Vector3 moveVector)
    {
        if (this.OrientRotationToMovement && moveVector.magnitude > 0f)
        {
            Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
            if (rotationSmoothing > 0f)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, this.rotationSmoothing * Time.deltaTime);
            }
            else
            {
                this.transform.rotation = rotation;
            }

            return true;
        }

        return false;
    }

    private bool AlignRotationWithControlRotationY()
    {
        if (this.UseControlRotation)
        {
            this.transform.rotation = this.ControlRotationY;
            return true;
        }

        return false;
    }

    private void AccelerateMoveSpeed()
    {
        if (Mathf.Abs(this.currentMoveSpeed - this.targetMoveSpeed) > 0.01f)
        {
            this.currentMoveSpeed = Mathf.Lerp(this.currentMoveSpeed, this.targetMoveSpeed, this.speedAcceleration * Time.deltaTime);
        }
    }
}
