using UnityEngine;
using System.Collections;

public class BallCameraController : MonoBehaviour
{
    private static BallCameraController instance;
    private static Camera cameraInstance;

    #region Unity Events

    private void Awake()
    {
        instance = this;
        cameraInstance = this.camera;
    }

    private void Update()
    {
    
    }

    #endregion Unity Events

    public static BallCameraController Instance
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
