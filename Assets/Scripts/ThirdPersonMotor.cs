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

    }

    private void ProcessMotion()
    {

    }
}
