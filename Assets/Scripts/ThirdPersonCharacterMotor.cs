using UnityEngine;
using System.Collections;

/// <summary>
/// Third person motor.
/// Processes the MoveVector and moves the character
/// </summary>
public class ThirdPersonCharacterMotor : MonoBehaviour
{
    private static ThirdPersonCharacterMotor instance;

    private GameObject cameraAligner; // A dummy game object that helps for proper convertion of the moveVector from Local-Space to World-Space
    private Vector3 moveVector;

    public float moveSpeed = 10.0f;

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

    /// <summary>
    /// Gets a reference to this instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ThirdPersonCharacterMotor Instance
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

    /// <summary>
    /// Updates the motor so that any motion data is converted to character movement.
    /// </summary>
    public void UpdateMotor()
    {
        if (this.IsCharacterMoving())
        {
            this.OrientCameraAlignerRotationToCameraDirection();

            // Transform MoveVector to World Space relative to camera aligner's orientation
            this.MoveVector = this.cameraAligner.transform.TransformDirection(this.MoveVector);

            this.OrientCharacterRotationToMoveVector();

            this.ProcessMotion();
        }
    }

    private void OrientCameraAlignerRotationToCameraDirection()
    {
        this.cameraAligner.transform.rotation = Quaternion.Euler(0.0f, ThirdPersonCameraController.Camera.transform.eulerAngles.y, 0.0f);
    }

    private void OrientCharacterRotationToMoveVector()
    {
        this.transform.rotation = Quaternion.LookRotation(this.MoveVector, Vector3.up);
    }

    private void ProcessMotion()
    {
        // Normalize MoveVector if Magnitude > 1
        if (this.MoveVector.magnitude > 1)
        {
            this.MoveVector = Vector3.Normalize(this.MoveVector);
        }

        // Multiply MoveVector by moveSpeed
        this.MoveVector *= this.moveSpeed;

        // Convert from units/frame to units/second
        this.MoveVector *= Time.deltaTime;

        // Move the character
        if (ThirdPersonCharacterController.CharacterController != null)
        {
            ThirdPersonCharacterController.CharacterController.Move(this.MoveVector);
        }
    }

    private bool IsCharacterMoving()
    {
        return (this.MoveVector.x != 0) || (this.MoveVector.z != 0);
    }
}
