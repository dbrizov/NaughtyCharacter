using UnityEngine;
using System.Collections;

/// <summary>
/// Third person character controller.
/// Processes player input and sends the input as "Move Vector" to the ThirdPersonMotor.
/// </summary>
public class ThirdPersonCharacterController : MonoBehaviour
{
    private static ThirdPersonCharacterController instance;

    /// <summary>
    /// Gets a reference to this instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ThirdPersonCharacterController Instance
    {
        get
        {
            return instance;
        }
    }
    
    #region Unity Events
    
    private void Awake()
    {
        instance = this;
    }
    
    private void FixedUpdate()
    {
        if (ThirdPersonCameraController.Camera != null)
        {
            this.ProcessMovement();
        }
    }
    
    #endregion Unity Events
    
    private void ProcessMovement()
    {
        ThirdPersonCharacterAnimator.Instance.IsCharacterSprinting = Input.GetButton("Sprint");
        
        Vector3 moveVector = this.GetMoveVectorFromInput();
        ThirdPersonCharacterAnimator.Instance.MoveCharacter(moveVector);
    }

    private Vector3 GetMoveVectorFromInput()
    {
        Vector3 moveVector = Vector3.zero;

        float xAxis = Input.GetAxis("Horizontal");
        moveVector.x += xAxis;

        float zAxis = Input.GetAxis("Vertical");
        moveVector.z += zAxis;

        return moveVector;
    }
}
