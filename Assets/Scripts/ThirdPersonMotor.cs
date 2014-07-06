using UnityEngine;
using System.Collections;

/// <summary>
/// Third person motor.
/// Processes the MoveVector and moves the character
/// </summary>
public class ThirdPersonMotor : MonoBehaviour
{
    private static ThirdPersonMotor instance;

    public float moveSpeed = 10.0f;

    public Vector3 MoveVector { get; set; }

    #region Unity Events

    private void Awake()
    {
        instance = this;
    }

    #endregion Unity Events

    public static ThirdPersonMotor Instance
    {
        get
        {
            return instance;
        }
    }

    public void UpdateMotor()
    {
        this.SnapAlignCharacterWithCamera();
        this.ProcessMotion();
    }

    private void SnapAlignCharacterWithCamera()
    {
        if (this.IsCharacterMoving())
        {
            // Rotate the character to match the direction the camera is facing
            this.transform.rotation = Quaternion.Euler(
                this.transform.eulerAngles.x, ThirdPersonCameraController.Camera.transform.eulerAngles.y, this.transform.eulerAngles.z);
        }
    }

    private void ProcessMotion()
    {
        // Transform MoveVector to World Space relative to character's orientation
        this.MoveVector = this.transform.TransformDirection(this.MoveVector);

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
        ThirdPersonController.CharacterController.Move(this.MoveVector);
    }

    private bool IsCharacterMoving()
    {
        return (this.MoveVector.x != 0) || (this.MoveVector.z != 0);
    }
}
