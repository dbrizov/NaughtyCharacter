using UnityEngine;
using System.Collections;

/// <summary>
/// Third person character animator.
/// Handles the animations for the character.
/// </summary>
public class ThirdPersonCharacterAnimator : MonoBehaviour
{
    private static ThirdPersonCharacterAnimator instance;

    private Animator animator;
    private Vector3 moveVector;
    private GameObject cameraAligner; // A dummy game object that helps for proper convertion of the moveVector from Local-Space to World-Space
    private int speedFloatHash;

    public float speed = 6.0f; // The speed of the character
    public float speedDampTime = 0.1f;
    public float turnSmoothing = 15.0f;

    /// <summary>
    /// Gets a reference to this instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ThirdPersonCharacterAnimator Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Gets or sets the move vector.
    /// </summary>
    /// <value>The move vector.</value>
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

    #region Unity Events

    private void Awake()
    {
        instance = this;

        this.animator = this.GetComponent<Animator>();

        this.speedFloatHash = Animator.StringToHash("Speed");
    }

    private void Start()
    {
        this.cameraAligner = new GameObject("_CameraAligner");
        this.cameraAligner.transform.position = Vector3.zero;
    }

    #endregion Unity Events

    public void UpdateAnimator()
    {
        if (this.IsCharacterMoving())
        {
            this.OrientCameraAlignerRotationToCameraDirection();

            // Transform MoveVector to World Space relative to camera aligner's orientation
            this.MoveVector = this.cameraAligner.transform.TransformDirection(this.MoveVector);
            
            this.OrientCharacterRotationToMoveVector();

            this.MoveCharacter();
        }
        else
        {
            this.StopCharacter();
        }
    }

    private void OrientCameraAlignerRotationToCameraDirection()
    {
        this.cameraAligner.transform.rotation = Quaternion.Euler(0.0f, ThirdPersonCameraController.Camera.transform.eulerAngles.y, 0.0f);
    }
    
    private void OrientCharacterRotationToMoveVector()
    {
        Quaternion newRotation = Quaternion.LookRotation(this.MoveVector, Vector3.up);
//        Quaternion newRotation = Quaternion.Lerp(this.rigidbody.rotation, targetRotation, this.turnSmoothing * Time.deltaTime);

        this.rigidbody.MoveRotation(newRotation);
    }

    private void MoveCharacter()
    {
        this.animator.SetFloat(this.speedFloatHash, this.speed, this.speedDampTime, Time.deltaTime);
    }

    private void StopCharacter()
    {
        this.animator.SetFloat(this.speedFloatHash, 0.0f);
    }

    private bool IsCharacterMoving()
    {
        return (this.MoveVector.x != 0) || (this.MoveVector.z != 0);
    }
}
