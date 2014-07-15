using UnityEngine;
using System.Collections;
using Utils;

public class ThirdPersonCameraController : MonoBehaviour
{
    private struct ClipPlanePoints
    {
        public Vector3 UpperLeft { get; set; }

        public Vector3 UpperRight { get; set; }

        public Vector3 LowerLeft { get; set; }

        public Vector3 LowerRight { get; set; }
    }

    private static ThirdPersonCameraController instance;
    private static Camera cameraInstance;

    private float mouseXRotation = 0.0f; // In degrees
    private float mouseYRotation = 20.0f; // In degrees

    public Transform targetToLookAt;
    public float distanceToTarget = 5.0f; // In meters
    public float minDistanceToTarget = 2.0f;
    public float maxDistanceToTarget = 20.0f;
    public float catchUpSpeed = 10.0f; // In meters/second
    public float mouseXSensitivity = 5.0f;
    public float mouseYSensitivity = 5.0f;
    public float mouseWheelSensitivity = 5.0f;
    public float mouseYRotationUpperLimit = 80.0f; // In degrees
    public float mouseYRotationLowerLimit = -40.0f; // In degrees

    #region Unity Events

    private void Awake()
    {
        instance = this;
        cameraInstance = this.camera;
    }

    private void Start()
    {
        this.distanceToTarget = Mathf.Clamp(this.distanceToTarget, this.minDistanceToTarget, this.maxDistanceToTarget);
    }

    private void LateUpdate()
    {
        if (this.targetToLookAt != null)
        {
            this.HandlePlayerInput(ref this.distanceToTarget, ref this.mouseXRotation, ref this.mouseYRotation);

            Vector3 newPosition = this.CalculateNewCameraPosition(this.distanceToTarget, this.mouseXRotation, this.mouseYRotation);

            this.UpdatePosition(newPosition);
        }
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

    private void HandlePlayerInput(ref float outDistanceToTarget, ref float outMouseXRotation, ref float outMouseYRotation)
    {
        // If the right mouse button is held down
        if (Input.GetMouseButton(1))
        {
            // Calculate the new rotation
            outMouseXRotation += Input.GetAxis("Mouse X") * this.mouseXSensitivity;
            outMouseYRotation -= Input.GetAxis("Mouse Y") * this.mouseYSensitivity;

            outMouseYRotation = Mathf.Clamp(outMouseYRotation, this.mouseYRotationLowerLimit, this.mouseYRotationUpperLimit);
        }

        // Calculate the new distance to target
        float mouseWheelAxis = Input.GetAxis("Mouse ScrollWheel") * this.mouseWheelSensitivity;
        outDistanceToTarget = Mathf.Clamp(
            outDistanceToTarget - mouseWheelAxis, this.minDistanceToTarget, this.maxDistanceToTarget);
    }

    private Vector3 CalculateNewCameraPosition(float distanceToTarget, float mouseXRotation, float mouseYRotation)
    {
        Vector3 direction = new Vector3(0.0f, 0.0f, -1.0f * distanceToTarget);
        Quaternion rotation = Quaternion.Euler(mouseYRotation, mouseXRotation, 0.0f);

        Vector3 newPosition = this.targetToLookAt.position + rotation * direction;
        return newPosition;
    }

    private void UpdatePosition(Vector3 position)
    {
        this.transform.position = Vector3.Lerp(this.transform.position, position, this.catchUpSpeed * Time.deltaTime);
        this.transform.LookAt(this.targetToLookAt);
    }

    private ClipPlanePoints GetNearClipPlanePoints()
    {
        ClipPlanePoints nearClipPlanePoints = new ClipPlanePoints();

        if (this.camera != null)
        {
            float halfFOV = (this.camera.fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
            float aspectRatio = this.camera.aspect; // viewportWidth / viewportHeight
            float distanceToNearClipPlane = this.camera.nearClipPlane;
            float halfHeight = Mathf.Tan(halfFOV) * distanceToNearClipPlane; // The half height of the Near Clip Plane of the Camera's frustum
            float halfWidth = halfHeight * aspectRatio; // The half width of the Near Clip Plane of the Camera's frustum

            nearClipPlanePoints.UpperLeft = this.transform.position - this.transform.right * halfWidth;
            nearClipPlanePoints.UpperLeft += this.transform.up * halfHeight;
            nearClipPlanePoints.UpperLeft += this.transform.forward * distanceToNearClipPlane;

            nearClipPlanePoints.UpperRight = this.transform.position + this.transform.right * halfWidth;
            nearClipPlanePoints.UpperRight += this.transform.up * halfHeight;
            nearClipPlanePoints.UpperRight += this.transform.forward * distanceToNearClipPlane;

            nearClipPlanePoints.LowerLeft = this.transform.position - this.transform.right * halfWidth;
            nearClipPlanePoints.LowerLeft -= this.transform.up * halfHeight;
            nearClipPlanePoints.LowerLeft += this.transform.forward * distanceToNearClipPlane;

            nearClipPlanePoints.LowerRight = this.transform.position + this.transform.right * halfWidth;
            nearClipPlanePoints.LowerRight -= this.transform.up * halfHeight;
            nearClipPlanePoints.LowerRight += this.transform.forward * distanceToNearClipPlane;
        }

        return nearClipPlanePoints;
    }
}
