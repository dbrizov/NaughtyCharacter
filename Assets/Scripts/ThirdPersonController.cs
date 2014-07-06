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

        this.GetLocomotionInput();
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

    private void GetLocomotionInput()
    {

    }
}
