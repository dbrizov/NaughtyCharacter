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
    private const float DefaultMouseXRotation = 0.0f;
    private const float DefaultMouseYRotation = 0.0f;
    private const float DefaultMouseXRotationSensitivity = 5.0f;
    private const float DefaultMouseYRotationSensitivity = 5.0f;
    private const float DefaultMouseWheelSensitivity = 5.0f;
    private const float UpperMouseYRotationLimit = -40.0f; // In degrees
    private const float LowerMouseYRotationLimit = 80.0f; // In degrees

    private static ThirdPersonCameraController instance;
    private static Camera cameraInstance;

    private float mouseXRotation = DefaultMouseXRotation; // In degrees
    private float mouseYRotation = DefaultMouseYRotation; // In degrees
    private float desiredDistanceToTarget = DefaultDistanceToTarget;

    public Transform targetToLookAt;
    public float distanceToTarget = DefaultDistanceToTarget; // The distance between the camera and the target
    public float minDistanceToTarget = DefaultMinDistanceToTarget; // The min distance between the camera and the target
    public float maxDistanceToTarget = DefaultMaxDistanceToTarget; // The max distance between the camera and the target
    public float mouseXSensitivity = DefaultMouseXRotationSensitivity;
    public float mouseYSensitivity = DefaultMouseYRotationSensitivity;
    public float mouseWheelSensitivity = DefaultMouseWheelSensitivity;
    public float mouseWheelDeadZone = 0.1f;

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
        this.mouseXRotation = DefaultMouseXRotation;
        this.mouseYRotation = DefaultMouseYRotation;
        this.distanceToTarget = DefaultDistanceToTarget;
        this.desiredDistanceToTarget = DefaultDistanceToTarget;
    }

    private void HandlePlayerInput()
    {
        if (Input.GetMouseButton(1))
        {
            this.mouseXRotation += Input.GetAxis("Mouse X") * this.mouseXSensitivity;
            this.mouseYRotation -= Input.GetAxis("Mouse Y") * this.mouseYSensitivity;

            this.mouseXRotation =
                Utils.MathfUtils.ClampAngle(this.mouseYRotation, LowerMouseYRotationLimit, UpperMouseYRotationLimit);
        }

        float mouseWheelAxis = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(mouseWheelAxis) > this.mouseWheelDeadZone)
        {
            this.desiredDistanceToTarget = Mathf.Clamp(
                this.distanceToTarget - mouseWheelAxis * this.mouseWheelSensitivity, this.minDistanceToTarget, this.maxDistanceToTarget);
        }
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
