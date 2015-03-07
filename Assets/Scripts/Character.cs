using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class Character : MonoBehaviour
{
    [SerializeField]
    private float movementSpeed = 10.0f; // In meters per second

    private Rigidbody rigidBody;
    private CapsuleCollider capsuleCollider;
    private bool shouldOrientRotationToMovement;

    protected virtual void Awake()
    {
        this.rigidBody = this.GetComponent<Rigidbody>();
        this.rigidBody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;

        this.capsuleCollider = this.GetComponent<CapsuleCollider>();

        this.shouldOrientRotationToMovement = true;
    }

    public void Move(Vector3 moveVector)
    {
        Vector3 newVelocity = new Vector3(moveVector.x, this.rigidBody.velocity.y, moveVector.z) * this.movementSpeed;
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
