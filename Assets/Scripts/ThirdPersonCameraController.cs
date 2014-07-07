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

    private static ThirdPersonCameraController instance;
    private static Camera cameraInstance;

    private float mouseXRotation = DefaultMouseXRotation; // In degrees
    private float mouseYRotation = DefaultMouseYRotation; // In degrees
    private float desiredDistanceToTarget = DefaultDistanceToTarget;
    private float velocityDistance = 0.0f; // A variable that will help calculating of the SmoothDamp of the camera

    public Transform targetToLookAt;
    public float distanceToTarget = DefaultDistanceToTarget; // The distance between the camera and the target
    public float minDistanceToTarget = DefaultMinDistanceToTarget; // The min distance between the camera and the target
    public float maxDistanceToTarget = DefaultMaxDistanceToTarget; // The max distance between the camera and the target
    public float distanceSmooth = 0.1f; // In seconds
    public float mouseXSensitivity = 5.0f;
    public float mouseYSensitivity = 5.0f;
    public float mouseWheelSensitivity = 5.0f;
    public float mouseWheelDeadZone = 0.1f;
    public float upperMouseYRotationLimit = 80.0f; // In degrees
    public float lowerMouseYRotationLimit = -40.0f; // In degrees

    #region Unity Events

    private void Awake()
    {
        instance = this;
        cameraInstance = this.camera;
    }

    private void Start()
    {
        this.distanceToTarget = Mathf.Clamp(this.distanceToTarget, this.minDistanceToTarget, this.maxDistanceToTarget);
        this.desiredDistanceToTarget = this.distanceToTarget;
    }

    private void LateUpdate()
    {
        if (this.targetToLookAt != null)
        {
            this.HandlePlayerInput(ref this.desiredDistanceToTarget, ref this.mouseXRotation, ref this.mouseYRotation);

            Vector3 desiredPosition =
                this.CalculateDesiredPosition(this.desiredDistanceToTarget, this.mouseXRotation, this.mouseYRotation);

            this.UpdatePosition(desiredPosition);
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
        this.minDistanceToTarget = DefaultMinDistanceToTarget;
        this.maxDistanceToTarget = DefaultMaxDistanceToTarget;
        this.distanceToTarget = DefaultDistanceToTarget;
        this.desiredDistanceToTarget = DefaultDistanceToTarget;
    }

    private void HandlePlayerInput(
        ref float outDesiredDistanceToTarget, ref float outMouseXRotation, ref float outMouseYRotation)
    {
        if (Input.GetMouseButton(1))
        {
            outMouseXRotation += Input.GetAxis("Mouse X") * this.mouseXSensitivity;
            outMouseYRotation -= Input.GetAxis("Mouse Y") * this.mouseYSensitivity;

            outMouseYRotation =
                Utils.MathfUtils.ClampAngle(outMouseYRotation, this.lowerMouseYRotationLimit, this.upperMouseYRotationLimit);
        }

        outDesiredDistanceToTarget = this.distanceToTarget;
        float mouseWheelAxis = Input.GetAxis("Mouse ScrollWheel");
        if (Mathf.Abs(mouseWheelAxis) > this.mouseWheelDeadZone)
        {
            outDesiredDistanceToTarget = Mathf.Clamp(
                this.distanceToTarget - mouseWheelAxis * this.mouseWheelSensitivity, this.minDistanceToTarget, this.maxDistanceToTarget);
        }
    }

    private Vector3 CalculateDesiredPosition(float desiredDistanceToTarget, float mouseXRotation, float mouseYRotation)
    {
        // Evaluate distance
        this.distanceToTarget = Mathf.SmoothDamp(
            this.distanceToTarget, desiredDistanceToTarget, ref this.velocityDistance, this.distanceSmooth);

        // Calculate desired position
        Vector3 desiredPosition =
            this.CalculatePosition(mouseYRotation, mouseXRotation, this.distanceToTarget);

        return desiredPosition;
    }

    private Vector3 CalculatePosition(float rotationX, float rotationY, float distance)
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, -distance);
        Quaternion rotation = Quaternion.Euler(rotationX, rotationY, 0.0f);

        Vector3 position = this.targetToLookAt.position + rotation * direction;
        return position;
    }

    private void UpdatePosition(Vector3 newPosition)
    {
        // TODO Implementation
    }
}
