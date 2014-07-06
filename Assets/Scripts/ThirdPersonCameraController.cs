using UnityEngine;
using System.Collections;

/// <summary>
/// Third person camera controller.
/// Converts mouse input to orbital motion around a pivot object (target to follow)
/// </summary>
public class ThirdPersonCameraController : MonoBehaviour
{
    private static ThirdPersonCameraController instance;
    private static Camera cameraInstance;

    public Transform targetToFollow;

    #region Unity Events

    private void Awake()
    {
        instance = this;
        cameraInstance = this.camera;
    }

    #endregion Unity Events

    public static ThirdPersonCameraController Instance
    {
        get
        {
            return instance;
        }
    }

    public static Camera Camera
    {
        get
        {
            return cameraInstance;
        }
    }
}
