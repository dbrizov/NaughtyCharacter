using UnityEngine;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class Character : MonoBehaviour
{
    public static Character Instance { get; private set; }

    // Serialized fields
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

    #region Unity Methods

    protected virtual void Awake()
    {
        if (Instance != null)
        {
            Destroy(this.gameObject);
            return;
        }

        Instance = this;

        this.controller = this.GetComponent<CharacterController>();

        this.GroundedState = new GroundedCharacterState(this);
        this.JumpState = new JumpCharacterState(this);
        this.CurrentState = this.GroundedState;        
        this.IsJogging = true;
    }

    protected virtual void Update()
    {
        this.ApplyGravity();
        this.CurrentState.UpdateState();

        this.UpdateHorizontalSpeed();
        this.ApplyMotion();
    }

    #endregion Unity Methods

    public ICharacterState CurrentState { get; set; }

    public GroundedCharacterState GroundedState { get; private set; }

    public JumpCharacterState JumpState { get; private set; }

    public Vector3 MoveVector
    {
        get
        {
            return this.moveVector;
        }
        set
        {
            float moveSpeed = value.magnitude * this.maxHorizontalSpeed;
            if (moveSpeed < Mathf.Epsilon)
            {
                value = this.moveVector;
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
                value.Normalize();
            }

            this.moveVector = value;
        }
    }

    public CharacterController Controller
    {
        get
        {
            return this.controller;
        }
    }

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
        set
        {
            this.controlRotation = value;
            this.AlignRotationWithControlRotationY();
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
            else
            {
                if (!(this.IsWalking || this.IsJogging))
                {
                    this.IsJogging = true;
                }
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

    public void Jump()
    {
        this.currentVerticalSpeed = this.MovementSettings.JumpForce;
    }

    public void ToggleWalk()
    {
        this.IsWalking = !this.IsWalking;

        if (!(this.IsWalking || this.IsJogging))
        {
            this.IsJogging = true;
        }
    }

    private void UpdateHorizontalSpeed()
    {
        float deltaSpeed = Mathf.Abs(this.currentHorizontalSpeed - this.targetHorizontalSpeed);
        if (deltaSpeed < 0.1f)
        {
            this.currentHorizontalSpeed = this.targetHorizontalSpeed;
            return;
        }

        bool shouldAccelerate = (this.currentHorizontalSpeed < this.targetHorizontalSpeed);

        this.currentHorizontalSpeed +=
            this.MovementSettings.Acceleration * Mathf.Sign(this.targetHorizontalSpeed - this.currentHorizontalSpeed) * Time.deltaTime;

        if (shouldAccelerate)
        {
            this.currentHorizontalSpeed = Mathf.Min(this.currentHorizontalSpeed, this.targetHorizontalSpeed);
        }
        else
        {
            this.currentHorizontalSpeed = Mathf.Max(this.currentHorizontalSpeed, this.targetHorizontalSpeed);
        }
    }

    private void ApplyGravity()
    {
        this.currentVerticalSpeed =
            MathfExtensions.ApplyGravity(this.VerticalSpeed, this.GravitySettings.GravityStrength, this.GravitySettings.MaxFallSpeed);
    }

    private void ApplyMotion()
    {
        this.OrientRotationToMoveVector(this.MoveVector);
        
        Vector3 motion = this.MoveVector * this.currentHorizontalSpeed + Vector3.up * this.currentVerticalSpeed;
        this.controller.Move(motion * Time.deltaTime);
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
}
