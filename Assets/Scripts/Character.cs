using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterInputController))]
public class Character : MonoBehaviour
{
    // Const variables
    private const float DefaultVerticalSpeed = -1f;
    private const float DefaultJumpSpeed = -1;

    // Serializable fields
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
    private float maxJumpSpeed = 7.5f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float maxVerticalSpeed = 50f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float horizontalAcceleration = 10f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float jumpAcceleration = 40f;

    [SerializeField]
    [Tooltip("In meters/second, [0, Infinity)")]
    private float gravityAcceleration = 25f;

    [SerializeField]
    [Tooltip("How fast the character rotates around the Y axis. Value of 0 disables Rotation Smoothing")]
    private float rotationSmoothing = 15f;

    [SerializeField]
    [Tooltip("Normalized percentage, [0, 1]")]
    private float airControl = 1f;

    [SerializeField]
    [HideInInspector] // Hidden in the inspector by default, because the property is shown by an editor script
    [Tooltip("Should the character be oriented his rotation to movement? The character can't orient it's rotation to movement and use control rotation at the same time.")]
    private bool orientRotationToMovement = true;

    [SerializeField]
    [HideInInspector] // Hidden in the inspector by default, because the property is shown by an editor script
    [Tooltip("Should the character use control rotation? The character can't use control rotation and orient it's rotation to movement at the same time.")]
    private bool useControlRotation = false;

    // Private fields
    private bool isWalking;
    private bool isJogging;
    private bool isSprinting;
    private Vector3 moveVector;
    private float maxHorizontalSpeed; // In meters per second
    private float targetHorizontalSpeed; // In meters per second
    private float currentHorizontalSpeed; // In meters per second
    private float currentVerticalSpeed; // In meters per second
    private float currentJumpSpeed; // In meters per second
    private Quaternion controlRotation;
    private CharacterController controller;

    protected virtual void Awake()
    {
        // Ensure that the entered values are correct through the setters
        this.HorizontalAcceleration = this.HorizontalAcceleration;
        this.GravityAcceleration = this.GravityAcceleration;
        this.JumpAcceleration = this.JumpAcceleration;
        this.MaxVerticalSpeed = this.MaxVerticalSpeed;
        this.MaxJumpSpeed = this.MaxJumpSpeed;
        this.WalkSpeed = this.WalkSpeed;
        this.JogSpeed = this.JogSpeed;
        this.SprintSpeed = this.SprintSpeed;
        this.RotationSmoothing = this.RotationSmoothing;
        this.AirControl = this.AirControl;

        // Configure the character
        this.IsJogging = true;
        this.currentVerticalSpeed = Character.DefaultVerticalSpeed;
        this.currentJumpSpeed = Character.DefaultJumpSpeed;

        this.controller = this.GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        this.ApplyGravity();
        this.ApplyJumpForce();
        this.UpdateHorizontalSpeed();
    }

    protected virtual void OnEnable()
    {
        CharacterInputController.OnMouseRotationInput += this.SetControlRotation;
        CharacterInputController.OnMoveInput += this.Move;
        CharacterInputController.OnJumpInput += this.Jump;
        CharacterInputController.OnSprintInput += this.SetSprintState;
        CharacterInputController.OnToggleWalkInput += this.ToggleWalk;
    }

    protected virtual void OnDisable()
    {
        CharacterInputController.OnMouseRotationInput -= this.SetControlRotation;
        CharacterInputController.OnMoveInput -= this.Move;
        CharacterInputController.OnJumpInput -= this.Jump;
        CharacterInputController.OnSprintInput -= this.SetSprintState;
        CharacterInputController.OnToggleWalkInput -= this.ToggleWalk;
    }

    public float WalkSpeed
    {
        get
        {
            return this.walkSpeed;
        }
        set
        {
            this.walkSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
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
            this.jogSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
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
            this.sprintSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
        }
    }

    public float MaxVerticalSpeed
    {
        get
        {
            return this.maxVerticalSpeed;
        }
        set
        {
            this.maxVerticalSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
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
            this.maxJumpSpeed = Mathf.Clamp(value, 0f, float.MaxValue);
        }
    }

    public float HorizontalAcceleration
    {
        get
        {
            return this.horizontalAcceleration;
        }
        set
        {
            this.horizontalAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
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
            this.jumpAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
        }
    }

    public float GravityAcceleration
    {
        get
        {
            return this.gravityAcceleration;
        }
        set
        {
            this.gravityAcceleration = Mathf.Clamp(value, 0f, float.MaxValue);
        }
    }

    public float RotationSmoothing
    {
        get
        {
            return this.rotationSmoothing;
        }
        set
        {
            this.rotationSmoothing = Mathf.Clamp(value, 0f, float.MaxValue);
        }
    }

    public float AirControl
    {
        get
        {
            return this.airControl;
        }
        set
        {
            this.airControl = Mathf.Clamp(value, 0f, 1f);
        }
    }

    /// <summary>
    /// If set to true, this automatically sets UseControlRotation to false
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
    /// If set to true, this automatically sets OrientRotationToMovement to false
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
        private set
        {
            this.controlRotation = value;
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
                this.maxHorizontalSpeed = this.WalkSpeed;
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
                this.maxHorizontalSpeed = this.JogSpeed;
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
                this.maxHorizontalSpeed = this.SprintSpeed;
                this.IsWalking = false;
                this.IsJogging = false;
            }
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

    public float HorizontalSpeed
    {
        get
        {
            return new Vector3(this.Velocity.x, 0f, this.Velocity.z).magnitude;
        }
    }

    public float VerticalSpeed
    {
        get
        {
            return this.Velocity.y;
        }
    }

    private void Move(Vector3 moveVector)
    {
        this.OrientRotationToMoveVector(moveVector);

        float moveSpeed = moveVector.magnitude * this.maxHorizontalSpeed;
        if (moveSpeed < Mathf.Epsilon)
        {
            moveVector = this.moveVector;
            this.targetHorizontalSpeed = 0f;
        }
        else if (moveSpeed > 0f && moveSpeed <= this.WalkSpeed)
        {
            this.targetHorizontalSpeed = this.WalkSpeed;
        }
        else if (moveSpeed > this.WalkSpeed && moveSpeed <= this.JogSpeed)
        {
            this.targetHorizontalSpeed = this.JogSpeed;
        }
        else if (moveSpeed > this.JogSpeed)
        {
            this.targetHorizontalSpeed = this.SprintSpeed;
        }

        if (moveSpeed > 0f)
        {
            moveVector.Normalize();
        }

        // TODO Improve the logic for air control
        float airControl = 1f;
        if (!this.IsGrounded)
        {
            airControl = this.AirControl;
        }

        Vector3 motion = (moveVector * this.currentHorizontalSpeed * airControl + Vector3.up * this.currentVerticalSpeed) * Time.deltaTime;
        this.controller.Move(motion);

        this.moveVector = moveVector;
    }

    private void Jump()
    {
        if (this.IsGrounded)
        {
            // This will cause an update upon the jump speed, bacause the currentJumpSpeed is different than the default one
            this.currentJumpSpeed = Character.DefaultJumpSpeed + 0.1f;
        }
    }

    private void SetSprintState(bool isSprinting)
    {
        if (isSprinting)
        {
            if (!this.IsSprinting)
            {
                this.IsSprinting = true;
            }
        }
        else
        {
            if (!(this.IsWalking || this.IsJogging))
            {
                this.IsJogging = true;
            }
        }
    }

    private void ToggleWalk()
    {
        this.IsWalking = !this.IsWalking;

        if (!(this.IsWalking || this.IsJogging))
        {
            this.IsJogging = true;
        }
    }

    private bool OrientRotationToMoveVector(Vector3 moveVector)
    {
        if (this.OrientRotationToMovement && moveVector.magnitude > 0f)
        {
            Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
            if (this.RotationSmoothing > 0f)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, this.RotationSmoothing * Time.deltaTime);
            }
            else
            {
                this.transform.rotation = rotation;
            }

            return true;
        }

        return false;
    }

    private void SetControlRotation(Quaternion controlRotation)
    {
        this.ControlRotation = controlRotation;

        this.AlignRotationWithControlRotationY();
    }

    private bool AlignRotationWithControlRotationY()
    {
        if (this.UseControlRotation)
        {
            this.transform.rotation = Quaternion.Euler(0f, this.ControlRotation.eulerAngles.y, 0f);
            return true;
        }

        return false;
    }

    private void UpdateHorizontalSpeed()
    {
        // TODO Fix the jittering when the HorizontalAcceleration is too big
        float deltaSpeed = Mathf.Abs(this.currentHorizontalSpeed - this.targetHorizontalSpeed);

        if (deltaSpeed > 0.01f)
        {
            this.currentHorizontalSpeed +=
                this.HorizontalAcceleration * Mathf.Sign(this.targetHorizontalSpeed - this.currentHorizontalSpeed) * Time.deltaTime;
        }

        if (deltaSpeed < 0.1f)
        {
            this.currentHorizontalSpeed = this.targetHorizontalSpeed;
        }

        if (this.targetHorizontalSpeed < 0.001f && this.currentHorizontalSpeed < 0.1f)
        {
            this.currentHorizontalSpeed = 0f;
        }
    }

    private void ApplyGravity()
    {
        if (!this.IsGrounded)
        {
            if (this.currentVerticalSpeed > -this.MaxVerticalSpeed)
            {
                this.currentVerticalSpeed -= this.GravityAcceleration * Time.deltaTime;
            }
            else
            {
                this.currentVerticalSpeed = -this.MaxVerticalSpeed;
            }
        }
        else
        {
            this.currentVerticalSpeed = DefaultVerticalSpeed;
        }
    }

    private void ApplyJumpForce()
    {
        if (this.currentJumpSpeed > Character.DefaultJumpSpeed)
        {
            this.currentJumpSpeed += this.JumpAcceleration * Time.deltaTime;
            this.currentVerticalSpeed = this.currentJumpSpeed;

            if (this.currentJumpSpeed > this.MaxJumpSpeed)
            {
                this.currentJumpSpeed = Character.DefaultJumpSpeed;
                this.currentVerticalSpeed = this.MaxJumpSpeed;
            }
        }
    }
}
