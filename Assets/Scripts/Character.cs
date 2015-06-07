using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    public const float MinMoveSpeed = 0f;
    public const float MaxMoveSpeed = 10f;
    public const float MinRotationSmoothing = 0f;
    public const float MaxRotationSmoothing = 30f;

    [SerializeField]
    [Range(MinMoveSpeed, MaxMoveSpeed)]
    [Tooltip("In meters/second")]
    private float walkSpeed = 2f; // In meters per second

    [SerializeField]
    [Range(MinMoveSpeed, MaxMoveSpeed)]
    [Tooltip("In meters/second")]
    private float jogSpeed = 3f; // In meters per second

    [SerializeField]
    [Range(MinMoveSpeed, MaxMoveSpeed)]
    [Tooltip("In meters/second")]
    private float sprintSpeed = 5f; // In meters per seconds

    [SerializeField]
    [Range(MinRotationSmoothing, MaxRotationSmoothing)]
    [Tooltip("How fast the character rotates around the Y axis")]
    private float rotationSmoothing = 15f;

    private bool bOrientRotationToMovement;
    private bool bUseControlRotation;
    private bool bIsWalking;
    private bool bIsJogging;
    private bool bIsSprinting;
    private float maxMoveSpeed; // In meters per second
    private Rigidbody rigidBody;
    private Quaternion controlRotation;
    private Quaternion controlRotationX;
    private Quaternion controlRotationY;

    protected virtual void Awake()
    {
        this.B_OrientRotationToMovement = true;
        this.WalkSpeed = this.WalkSpeed;
        this.JogSpeed = this.JogSpeed;
        this.SprintSpeed = this.SprintSpeed;
        this.B_IsJogging = true;

        this.rigidBody = this.GetComponent<Rigidbody>();
        this.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
    }

    protected virtual void Update()
    {
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
    public bool B_OrientRotationToMovement
    {
        get
        {
            return this.bOrientRotationToMovement;
        }
        set
        {
            this.bOrientRotationToMovement = value;
            if (this.bOrientRotationToMovement)
            {
                this.bUseControlRotation = false;
            }
        }
    }

    /// <summary>
    /// If set to true, this automatically sets B_OrientRotationToMovement to false
    /// </summary>
    public bool B_UseControlRotation
    {
        get
        {
            return this.bUseControlRotation;
        }
        set
        {
            this.bUseControlRotation = value;
            if (this.bUseControlRotation)
            {
                this.bOrientRotationToMovement = false;
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

    public bool B_IsWalking
    {
        get
        {
            return this.bIsWalking;
        }
        set
        {
            this.bIsWalking = value;
            if (this.bIsWalking)
            {
                this.maxMoveSpeed = this.WalkSpeed;
                this.B_IsJogging = false;
                this.B_IsSprinting = false;
            }
        }
    }

    public bool B_IsJogging
    {
        get
        {
            return this.bIsJogging;
        }
        set
        {
            this.bIsJogging = value;
            if (this.bIsJogging)
            {
                this.maxMoveSpeed = this.JogSpeed;
                this.B_IsWalking = false;
                this.B_IsSprinting = false;
            }
        }
    }

    public bool B_IsSprinting
    {
        get
        {
            return this.bIsSprinting;
        }
        set
        {
            this.bIsSprinting = value;
            if (this.bIsSprinting)
            {
                this.maxMoveSpeed = this.SprintSpeed;
                this.B_IsWalking = false;
                this.B_IsJogging = false;
            }
        }
    }

    public float HorizontalSpeed
    {
        get
        {
            return this.HorizontalVelocity.magnitude;
        }
    }

    public float VerticalSpeed
    {
        get
        {
            return this.VerticalVelocity.magnitude;
        }
    }

    public Vector3 HorizontalVelocity
    {
        get
        {
            return new Vector3(this.rigidBody.velocity.x, 0f, this.rigidBody.velocity.z);
        }
    }

    public Vector3 VerticalVelocity
    {
        get
        {
            return new Vector3(0f, this.rigidBody.velocity.y, 0f);
        }
    }

    public void Move(Vector3 moveVector)
    {
        if (moveVector.magnitude * this.maxMoveSpeed < this.WalkSpeed)
        {
            return;
        }

        Vector3 newVelocity = new Vector3(moveVector.x, this.rigidBody.velocity.y, moveVector.z) * this.maxMoveSpeed;
        this.rigidBody.velocity = newVelocity;

        this.OrientRotationToMovement(moveVector);
    }

    private bool OrientRotationToMovement(Vector3 moveVector)
    {
        if (this.B_OrientRotationToMovement &&
            moveVector.magnitude * this.maxMoveSpeed >= this.WalkSpeed)
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
        if (this.B_UseControlRotation)
        {
            this.transform.rotation = this.ControlRotationY;
            return true;
        }

        return false;
    }
}
