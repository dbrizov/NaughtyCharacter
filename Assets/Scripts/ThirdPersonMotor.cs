using UnityEngine;
using System.Collections;

/// <summary>
/// Third person motor.
/// Processes the MoveVector and moves the character
/// </summary>
public class ThirdPersonMotor : MonoBehaviour
{
    public static ThirdPersonMotor Instance;

    public float moveSpeed = 10.0f;

    public Vector3 MoveVector { get; set; }

    #region Unity Events

    private void Awake()
    {
        Instance = this;
    }

    #endregion Unity Events

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
