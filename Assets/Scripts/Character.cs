using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    [SerializeField]
    private float walkSpeed = 1.6f; // In meters per second

    [SerializeField]
    private float jogSpeed = 5.7f; // In meters per second

    [SerializeField]
    private float sprintSpeed = 8.0f; // In meters per seconds

    private bool isWalking;
    private bool isJogging;
    private bool isSprinting;
    private float maxMoveSpeed; // In meters per second
    private Rigidbody rigidBody;
    private bool shouldOrientRotationToMovement;

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
                this.maxMoveSpeed = this.walkSpeed;
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
                this.maxMoveSpeed = this.jogSpeed;
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
                this.maxMoveSpeed = this.sprintSpeed;
                this.IsWalking = false;
                this.IsJogging = false;
            }
        }
    }

    public float HorizontalSpeed
    {
        get
        {
            Vector3 horizontalVelocity = new Vector3(this.rigidBody.velocity.x, 0.0f, this.rigidBody.velocity.z);
            return horizontalVelocity.magnitude;
        }
    }

    protected virtual void Awake()
    {
        this.maxMoveSpeed = this.jogSpeed;

        this.rigidBody = this.GetComponent<Rigidbody>();
        this.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
        
        this.shouldOrientRotationToMovement = true;
    }

    public void Move(Vector3 moveVector)
    {
        Vector3 newVelocity = new Vector3(moveVector.x, this.rigidBody.velocity.y, moveVector.z) * this.maxMoveSpeed;
        this.rigidBody.velocity = newVelocity;

        this.OrientRotationToMovement(moveVector);
    }

    private void OrientRotationToMovement(Vector3 moveVector)
    {
        if (this.shouldOrientRotationToMovement && moveVector.magnitude != 0.0f)
        {
            Quaternion rotation = Quaternion.LookRotation(moveVector, Vector3.up);
            this.transform.rotation = rotation;
        }
    }
}
