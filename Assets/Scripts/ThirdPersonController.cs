using UnityEngine;
using System.Collections;

/// <summary>
/// Third person controller.
/// Processes player input and sends the input as "Move Vector" to the ThirdPersonMotor.
/// </summary>
public class ThirdPersonController : MonoBehaviour
{
    private static CharacterController characterController;
    private static ThirdPersonController instance;

    public float moveVectorDeadZone = 0.1f; // The character will move only if any of the "x" and "z" properties of the MoveVector is greated then the dead zone;

    #region Unity Events

    private void Awake()
    {
        characterController = this.GetComponent<CharacterController>();
        instance = this;
    }

    private void Update()
    {
        if (Camera.main == null)
        {
            return;
        }

        ThirdPersonMotor.Instance.MoveVector = this.GetMoveVectorFromInput();
        ThirdPersonMotor.Instance.UpdateMotor();
    }

    #endregion Unity Events

    public static CharacterController CharacterController
    {
        get
        {
            return characterController;
        }
    }

    public static ThirdPersonController Instance
    {
        get
        {
            return instance;
        }
    }

    private Vector3 GetMoveVectorFromInput()
    {
        Vector3 moveVector = Vector3.zero;

        float xAxis = Input.GetAxis("Horizontal");
        if (Mathf.Abs(xAxis) > this.moveVectorDeadZone)
        {
            moveVector.x += xAxis;
        }

        float zAxis = Input.GetAxis("Vertical");
        if (Mathf.Abs(zAxis) > this.moveVectorDeadZone)
        {
            moveVector.z += zAxis;
        }

        return moveVector;
    }
}
