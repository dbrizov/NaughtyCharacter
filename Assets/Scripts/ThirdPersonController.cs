using UnityEngine;
using System.Collections;

/// <summary>
/// Third person controller.
/// Processes player input and sends the input as "Move Vector" to the ThirdPersonMotor.
/// </summary>
public class ThirdPersonController : MonoBehaviour
{
    public static CharacterController CharacterController;
    public static ThirdPersonController Instance;

    #region Unity Events

    private void Awake()
    {
        CharacterController = this.GetComponent<CharacterController>();
        Instance = this;
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

    private void GetLocomotionInput()
    {

    }
}
