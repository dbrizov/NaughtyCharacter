using UnityEngine;
using System.Collections;

/// <summary>
/// Third person camera controller.
/// Converts mouse input to orbital motion around a pivot object (target to follow)
/// </summary>
public class ThirdPersonCameraController : MonoBehaviour
{
    private const float DefaultDistanceToTarget = 5.0f;
    private const float DefaultMinDistanceToTarget = 2.0f;
    private const float DefaultMaxDistanceToTarget = 15.0f;
    private const float DefaultXAxisRotation = 0.0f;
    private const float DefaultYAxisRotation = 0.0f;

    private static ThirdPersonCameraController instance;
    private static Camera cameraInstance;

    private float xAxisRotation = DefaultXAxisRotation; // Rotation around the X axis in degrees
    private float yAxisRotation = DefaultYAxisRotation; // Rotation around the Y axis in degrees

    public Transform targetToLookAt;
    public float distanceToTarget = DefaultDistanceToTarget; // The distance between the camera and the target
    public float minDistanceToTarget = DefaultMinDistanceToTarget; // The min distance between the camera and the target
    public float maxDistanceToTarget = DefaultMaxDistanceToTarget; // The max distance between the camera and the target

    #region Unity Events

    private void Awake()
    {
        instance = this;
        cameraInstance = this.camera;
    }

    private void Start()
    {
        this.distanceToTarget = Mathf.Clamp(this.distanceToTarget, this.minDistanceToTarget, this.maxDistanceToTarget);
        this.Reset();
    }

    private void LateUpdate()
    {
        if (this.targetToLookAt != null)
        {
            this.HandlePlayerInput();
            this.CalculateDesiredPosition();
            this.UpdatePosition();
        }
    }

    #endregion Unity Events

    /// <summary>
    /// Gets a reference to this instance.
    /// </summary>
    /// <value>The instance.</value>
    public static ThirdPersonCameraController Instance
    {
        get
        {
            return instance;
        }
    }

    /// <summary>
    /// Gets the camera.
    /// </summary>
    /// <value>The camera.</value>
    public static Camera Camera
    {
        get
        {
            return cameraInstance;
        }
    }

    /// <summary>
    /// Resets the camera to it's default position and orientation.
    /// </summary>
    public void Reset()
    {
        this.xAxisRotation = DefaultXAxisRotation;
        this.yAxisRotation = DefaultYAxisRotation;
        this.distanceToTarget = DefaultDistanceToTarget;
    }

    private void HandlePlayerInput()
    {
        // TODO Implementation
    }

    private void CalculateDesiredPosition()
    {
        // TODO Implementation
    }

    private void UpdatePosition()
    {
        // TODO Implementation
    }
}
