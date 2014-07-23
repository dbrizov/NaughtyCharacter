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
    private float walkSpeed = 1.5f;
    private float runSpeed = 5.5f;
    private float sprintSpeed = 7.0f;
    private float speedDampTime = 0.1f;
    private int speedHash;
    private int isSprintingHash;
    
    #region Unity Events
    
    private void Awake()
    {
        instance = this;
        
        this.animator = this.GetComponent<Animator>();
        
        this.speedHash = Animator.StringToHash("Speed");
        this.isSprintingHash = Animator.StringToHash("IsSprinting");
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
    public static ThirdPersonCharacterAnimator Instance
    {
        get
        {
            return instance;
        }
    }
    
    public bool IsCharacterRunning
    {
        get
        {
            return this.animator.GetFloat(this.speedHash) > this.walkSpeed;
        }
    }

    public bool IsCharacterSprinting
    {
        get
        {
            return this.animator.GetBool(this.isSprintingHash);
        }
        set
        {
            this.animator.SetBool(this.isSprintingHash, value);
        }
    }
    
    public void MoveCharacter(Vector3 moveVector)
    {
        this.moveVector = moveVector;
        
        if (this.IsCharacterMoving())
        {
            this.MoveCharacter();
        }
        else
        {
            this.StopCharacter();
        }
    }
    
    private void MoveCharacter()
    {
        // Orient the camera aligner rotation to the camera's direction
        this.cameraAligner.transform.rotation = Quaternion.Euler(0.0f, ThirdPersonCameraController.Camera.transform.eulerAngles.y, 0.0f);
        
        // Transform moveVector to World Space relative to camera aligner's orientation
        this.moveVector = this.cameraAligner.transform.TransformDirection(this.moveVector);
        
        // Orient character rotation to moveVector
        Quaternion newRotation = Quaternion.LookRotation(this.moveVector, Vector3.up);
        this.rigidbody.MoveRotation(newRotation);
        
        // Move the character
        float moveVectorMagnitude = this.moveVector.magnitude;
        if (moveVectorMagnitude > 1.0f)
        {
            moveVectorMagnitude = 1.0f;
        }

        if (this.IsCharacterSprinting)
        {
            this.animator.SetFloat(this.speedHash, this.sprintSpeed * moveVectorMagnitude, this.speedDampTime, Time.deltaTime);
        }
        else
        {
            this.animator.SetFloat(this.speedHash, this.runSpeed * moveVectorMagnitude, this.speedDampTime, Time.deltaTime);
        }
    }
    
    private void StopCharacter()
    {
        this.animator.SetFloat(this.speedHash, 0.0f);
    }
    
    private bool IsCharacterMoving()
    {
        return (this.moveVector.x != 0) || (this.moveVector.z != 0);
    }
}
