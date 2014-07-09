using UnityEngine;
using System.Collections;

public class BallMotor : MonoBehaviour
{
    private static BallMotor instance;

    private GameObject cameraAligner; // A dummy game object that helps for proper convertion of the moveVector from Local-Space to World-Space
    private Vector3 moveVector = Vector3.zero;
    
    public float moveSpeed = 750.0f;

    #region Unity Events

    private void Awake()
    {
        instance = this;
    }

    private void Start()
    {
        this.cameraAligner = new GameObject("_CameraAligner");
        this.cameraAligner.transform.position = Vector3.zero;
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
            this.SnapAlignCameraAlignerWithCamera();
            this.ProcessMotion();
        }
    }

    private void SnapAlignCameraAlignerWithCamera()
    {
        this.cameraAligner.transform.rotation = Quaternion.Euler(
            this.cameraAligner.transform.eulerAngles.x, ThirdPersonCameraController.Camera.transform.eulerAngles.y, this.cameraAligner.transform.eulerAngles.z);
    }

    private void ProcessMotion()
    {
        // Transform MoveVector to World Space relative to cameraAligner's orientation
        this.MoveVector = this.cameraAligner.transform.TransformDirection(this.MoveVector);

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
