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
    private float rotationSmoothing = 20f;

    [SerializeField]
    private bool bOrientRotationToMovement = true;

    private bool isWalking;
    private bool isJogging;
    private bool isSprinting;
    private float maxMoveSpeed; // In meters per second
    private Rigidbody rigidBody;

    protected virtual void Awake()
    {
        this.WalkSpeed = this.WalkSpeed;
        this.JogSpeed = this.JogSpeed;
        this.SprintSpeed = this.SprintSpeed;
        this.maxMoveSpeed = this.JogSpeed;

        this.rigidBody = this.GetComponent<Rigidbody>();
        this.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
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

    public bool B_OrientRotationToMovement
    {
        get
        {
            return this.bOrientRotationToMovement;
        }
        set
        {
            this.bOrientRotationToMovement = value;
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

    public float HorizontalSpeed
    {
        get
        {
            Vector3 horizontalVelocity = new Vector3(this.rigidBody.velocity.x, 0f, this.rigidBody.velocity.z);
            return horizontalVelocity.magnitude;
        }
    }

    public float VerticalSpeed
    {
        get
        {
            Vector3 verticalVelocity = new Vector3(0f, this.rigidBody.velocity.y, 0f);
            return verticalVelocity.magnitude;
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

    private void OrientRotationToMovement(Vector3 moveVector)
    {
        if (this.B_OrientRotationToMovement && moveVector.magnitude != 0.0f)
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
        }
    }
}
