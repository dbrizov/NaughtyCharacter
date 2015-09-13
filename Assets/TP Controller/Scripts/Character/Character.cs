using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(CharacterInputController))]
public class Character : MonoBehaviour
{
    // Serialize fields
    [SerializeField]
    private MovementSettings movementSettings;

    [SerializeField]
    private GravitySettings gravitySettings;

    [SerializeField]
    [HideInInspector]
    private RotationSettings rotationSettings;

    // Private fields
    private Vector3 moveVector;
    private Quaternion controlRotation;
    private CharacterController controller;
    private bool isWalking;
    private bool isJogging;
    private bool isSprinting;
    private float maxHorizontalSpeed; // In meters/second
    private float targetHorizontalSpeed; // In meters/second
    private float currentHorizontalSpeed; // In meters/second
    private float currentVerticalSpeed; // In meters/second
    private float groundedVerticalSpeed; // In meters/second
    private float currentJumpSpeed; // In meters/second
    private bool bApplyJumpForce;
    private bool bApplyGravity;

    #region Unity Methods

    protected virtual void Awake()
    {
        // Configure the character
        this.IsJogging = true;
        this.groundedVerticalSpeed = -(this.MovementSettings.SprintSpeed + 2.5f);
        this.currentVerticalSpeed = this.groundedVerticalSpeed;
        this.currentJumpSpeed = 0f;
        this.bApplyJumpForce = false;
        this.bApplyGravity = false;

        this.controller = this.GetComponent<CharacterController>();
    }

    protected virtual void Update()
    {
        this.UpdateHorizontalSpeed();
        this.UpdateVerticalSpeed();

        this.ApplyMotion();
    }

    protected virtual void OnEnable()
    {
        CharacterInputController.OnMouseRotationInput += this.SetControlRotation;
        CharacterInputController.OnMoveInput += this.SetMoveVector;
        CharacterInputController.OnJumpInput += this.Jump;
        CharacterInputController.OnSprintInput += this.SetSprintState;
        CharacterInputController.OnToggleWalkInput += this.ToggleWalk;
    }

    protected virtual void OnDisable()
    {
        CharacterInputController.OnMouseRotationInput -= this.SetControlRotation;
        CharacterInputController.OnMoveInput -= this.SetMoveVector;
        CharacterInputController.OnJumpInput -= this.Jump;
        CharacterInputController.OnSprintInput -= this.SetSprintState;
        CharacterInputController.OnToggleWalkInput -= this.ToggleWalk;
    }

    #endregion Unity Methods

    public MovementSettings MovementSettings
    {
        get
        {
            return this.movementSettings;
        }
        set
        {
            this.movementSettings = value;
        }
    }

    public GravitySettings GravitySettings
    {
        get
        {
            return this.gravitySettings;
        }
        set
        {
            this.gravitySettings = value;
        }
    }

    public RotationSettings RotationSettings
    {
        get
        {
            return this.rotationSettings;
        }
        set
        {
            this.rotationSettings = value;
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
                this.maxHorizontalSpeed = this.MovementSettings.WalkSpeed;
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
                this.maxHorizontalSpeed = this.MovementSettings.JogSpeed;
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
                this.maxHorizontalSpeed = this.MovementSettings.SprintSpeed;
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

    public Vector3 HorizontalVelocity
    {
        get
        {
            return new Vector3(this.Velocity.x, 0f, this.Velocity.z);
        }
    }

    public Vector3 VerticalVelocity
    {
        get
        {
            return new Vector3(0f, this.Velocity.y, 0f);
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

    private void SetMoveVector(Vector3 moveVector)
    {
        this.OrientRotationToMoveVector(moveVector);

        float moveSpeed = moveVector.magnitude * this.maxHorizontalSpeed;
        if (moveSpeed < Mathf.Epsilon)
        {
            moveVector = this.moveVector;
            this.targetHorizontalSpeed = 0f;
        }
        else if (moveSpeed > 0f && moveSpeed <= this.MovementSettings.WalkSpeed)
        {
            this.targetHorizontalSpeed = this.MovementSettings.WalkSpeed;
        }
        else if (moveSpeed > this.MovementSettings.WalkSpeed && moveSpeed <= this.MovementSettings.JogSpeed)
        {
            this.targetHorizontalSpeed = this.MovementSettings.JogSpeed;
        }
        else if (moveSpeed > this.MovementSettings.JogSpeed)
        {
            this.targetHorizontalSpeed = this.MovementSettings.SprintSpeed;
        }

        if (moveSpeed > 0f)
        {
            moveVector.Normalize();
        }

        this.moveVector = moveVector;
    }

    private void ApplyMotion()
    {
        Vector3 motion = this.moveVector * this.currentHorizontalSpeed + Vector3.up * this.currentVerticalSpeed;
        this.controller.Move(motion * Time.deltaTime);
    }

    private void Jump()
    {
        if (this.IsGrounded)
        {
            this.bApplyJumpForce = true;
            this.currentVerticalSpeed = 0f;
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
        if (this.RotationSettings.OrientRotationToMovement && moveVector.magnitude > 0f)
        {
            Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
            if (this.RotationSettings.RotationSmoothing > 0f)
            {
                this.transform.rotation = Quaternion.Slerp(this.transform.rotation, rotation, this.RotationSettings.RotationSmoothing * Time.deltaTime);
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
        if (this.RotationSettings.UseControlRotation)
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
                this.MovementSettings.Acceleration * Mathf.Sign(this.targetHorizontalSpeed - this.currentHorizontalSpeed) * Time.deltaTime;
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

    private void UpdateVerticalSpeed()
    {
        this.ApplyGravity();
        this.ApplyJumpForce();
    }

    private void ApplyGravity()
    {
        if (!this.IsGrounded && !this.bApplyGravity)
        {
            this.bApplyGravity = true;
            this.currentVerticalSpeed = 0f;
        }

        if (this.bApplyJumpForce)
        {
            // Don't apply gravity before max jump speed is reached
            return;
        }

        if (!this.IsGrounded && this.bApplyGravity)
        {
            if (this.currentVerticalSpeed > -this.GravitySettings.MaxFallSpeed)
            {
                this.currentVerticalSpeed -= this.GravitySettings.GravityStrength * Time.deltaTime;
            }
            else
            {
                this.currentVerticalSpeed = -this.GravitySettings.MaxFallSpeed;
            }
        }
        else if (this.IsGrounded)
        {
            bApplyGravity = false;
            this.currentVerticalSpeed = this.groundedVerticalSpeed;
        }
    }

    private void ApplyJumpForce()
    {
        if (this.bApplyJumpForce)
        {
            this.currentJumpSpeed += this.MovementSettings.JumpAcceleration * Time.deltaTime;
            this.currentVerticalSpeed = this.currentJumpSpeed;

            if (this.currentJumpSpeed > this.MovementSettings.MaxJumpSpeed)
            {
                this.currentJumpSpeed = 0f;
                this.currentVerticalSpeed = this.MovementSettings.MaxJumpSpeed;
                this.bApplyJumpForce = false;
            }
        }
    }
}
