using UnityEngine;
using System.Collections;

public class BallMotor : MonoBehaviour
{
    private static BallMotor instance;

    private GameObject dummy;
    private Vector3 moveVector = Vector3.zero;
    
    public float moveSpeed = 750.0f;

    #region Unity Events

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        this.dummy = new GameObject("Dummy");
        this.dummy.transform.position = Vector3.zero;
    }

    #endregion Unity Events

    public static BallMotor Instance
    {
        get
        {
            return instance;
        }
    }

    public Vector3 MoveVector
    {
        get
        {
            return this.moveVector;
        }
        set
        {
            this.moveVector = value;
        }
    }

    public void UpdateMotor()
    {
        if (this.IsBallMoving())
        {
            this.SnapAlignDummyWithCamera();
            this.ProcessMotion();
        }
    }

    private void SnapAlignDummyWithCamera()
    {
        this.dummy.transform.rotation = Quaternion.Euler(
            this.dummy.transform.eulerAngles.x, BallCameraController.Camera.transform.eulerAngles.y, this.dummy.transform.eulerAngles.z);
    }

    private void ProcessMotion()
    {
        // Transform MoveVector to World Space relative to Dummy's orientation
        this.MoveVector = this.dummy.transform.TransformDirection(this.MoveVector);

        if (this.MoveVector.magnitude> 1.0f)
        {
            this.MoveVector = Vector3.Normalize(this.MoveVector);
        }

        this.MoveVector *= this.moveSpeed;
        this.MoveVector *= Time.deltaTime;

        this.rigidbody.AddForce(this.MoveVector, ForceMode.Force);
    }

    private bool IsBallMoving()
    {
        return (this.MoveVector.x != 0.0f) || (this.MoveVector.z != 0.0f);
    }
}
