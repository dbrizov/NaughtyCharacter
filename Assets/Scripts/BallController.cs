using UnityEngine;
using System.Collections;

public class BallController : MonoBehaviour
{
    private static BallController instance;

    public float moveVectorDeadZone = 0.1f;

    #region Unity Events

    private void Awake()
    {
        instance = this;
    }

    private void Update()
    {
        if (ThirdPersonCameraController.Camera != null)
        {
            BallMotor.Instance.MoveVector = this.GetMoveVectorFromInput();
            BallMotor.Instance.UpdateMotor();
        }
    }

    #endregion Unity Events

    public static BallController Instance
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
