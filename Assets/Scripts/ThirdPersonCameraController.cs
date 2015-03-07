using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ThirdPersonCameraController : MonoBehaviour
{
    private struct ClipPlaneCornerPoints
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
    private int ignorePlayerBitMask; // When checking for camera collisions/occlusions we need to ignore collisions/occlusions with players
    private float distanceToTargetWhenCameraIsOccluded;
    private int currentOcclusionCheck = 0; // The currentOcclusion check. If it exceeds the maxOcclusionChecks, then the loop breaks

    public Transform targetToLookAt;
    public float distanceToTarget = 3.0f; // In meters
    public float minDistanceToTarget = 1.0f;
    public float maxDistanceToTarget = 20.0f;
    public int maxOcclusionChecks = 5; // Each frame we check if the camera is occluded 5 times total
    public float catchUpSpeed = 10.0f; // In meters/second
    public float mouseXSensitivity = 5.0f;
    public float mouseYSensitivity = 5.0f;
    public float mouseWheelSensitivity = 5.0f;
    public float mouseYRotationUpperLimit = 80.0f; // In degrees
    public float mouseYRotationLowerLimit = -50.0f; // In degrees
    public int playerLayer; // The index of the Player Layer

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
    /// Gets the camera component.
    /// </summary>
    /// <value>The camera.</value>
    public static Camera Camera
    {
        get
        {
            return cameraInstance;
        }
    }

    #region Unity Events
    
    private void Awake()
    {
        instance = this;
        cameraInstance = this.GetComponent<Camera>();
        
        this.ignorePlayerBitMask = ~(1 << this.playerLayer);
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
            
            this.currentOcclusionCheck = 0;
            while (this.IsCameraOccluded(newPosition, ref this.distanceToTargetWhenCameraIsOccluded) &&
                   this.currentOcclusionCheck < this.maxOcclusionChecks)
            {
                newPosition = this.CalculateNewCameraPosition(this.distanceToTargetWhenCameraIsOccluded, this.mouseXRotation, this.mouseYRotation);
                this.currentOcclusionCheck++;
            }
            
            this.UpdatePosition(newPosition);
        }
    }
    
    #endregion Unity Events

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

    private void UpdatePosition(Vector3 newPosition)
    {
        this.transform.position = Vector3.Lerp(this.transform.position, newPosition, this.catchUpSpeed * Time.deltaTime);
        this.transform.LookAt(this.targetToLookAt);
    }

    private ClipPlaneCornerPoints GetNearClipPlaneCornerPoints(Vector3 cameraPosition)
    {
        ClipPlaneCornerPoints nearClipPlanePoints = new ClipPlaneCornerPoints();

        if (this.GetComponent<Camera>() != null)
        {
            float halfFOV = (this.GetComponent<Camera>().fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
            float aspectRatio = this.GetComponent<Camera>().aspect; // viewportWidth / viewportHeight
            float distanceToNearClipPlane = this.GetComponent<Camera>().nearClipPlane;
            float halfHeight = Mathf.Tan(halfFOV) * distanceToNearClipPlane; // The half height of the Near Clip Plane of the Camera's view frustum
            float halfWidth = halfHeight * aspectRatio; // The half width of the Near Clip Plane of the Camera's view frustum

            nearClipPlanePoints.UpperLeft = cameraPosition - this.transform.right * halfWidth;
            nearClipPlanePoints.UpperLeft += this.transform.up * halfHeight;
            nearClipPlanePoints.UpperLeft += this.transform.forward * distanceToNearClipPlane;

            nearClipPlanePoints.UpperRight = cameraPosition + this.transform.right * halfWidth;
            nearClipPlanePoints.UpperRight += this.transform.up * halfHeight;
            nearClipPlanePoints.UpperRight += this.transform.forward * distanceToNearClipPlane;

            nearClipPlanePoints.LowerLeft = cameraPosition - this.transform.right * halfWidth;
            nearClipPlanePoints.LowerLeft -= this.transform.up * halfHeight;
            nearClipPlanePoints.LowerLeft += this.transform.forward * distanceToNearClipPlane;

            nearClipPlanePoints.LowerRight = cameraPosition + this.transform.right * halfWidth;
            nearClipPlanePoints.LowerRight -= this.transform.up * halfHeight;
            nearClipPlanePoints.LowerRight += this.transform.forward * distanceToNearClipPlane;
        }

        return nearClipPlanePoints;
    }

    /// <summary>
    /// Checks the camera collision points and returns the nearest collision distance.
    /// The nearest collision distance is -1, if there are no collisions, else it is greater than -1
    /// </summary>
    /// <returns>If there are no collisions returns -1. Else it returns a float greater than -1</returns>
    /// <param name="cameraPosition">The position of the camera.</param>
    private float CheckCameraCollisionPoints(Vector3 cameraPosition)
    {
        float nearestCollisionDistance = -1.0f;

        ClipPlaneCornerPoints nearClipPlaneCornerPoints = this.GetNearClipPlaneCornerPoints(cameraPosition);

        // Draw the lines to the collision points for debugging
        Debug.DrawLine(this.targetToLookAt.position, cameraPosition - this.transform.forward * this.GetComponent<Camera>().nearClipPlane, Color.red);
        Debug.DrawLine(this.targetToLookAt.position, nearClipPlaneCornerPoints.UpperLeft, Color.green);
        Debug.DrawLine(this.targetToLookAt.position, nearClipPlaneCornerPoints.UpperRight, Color.green);
        Debug.DrawLine(this.targetToLookAt.position, nearClipPlaneCornerPoints.LowerLeft, Color.green);
        Debug.DrawLine(this.targetToLookAt.position, nearClipPlaneCornerPoints.LowerRight, Color.green);
        Debug.DrawLine(nearClipPlaneCornerPoints.UpperLeft, nearClipPlaneCornerPoints.UpperRight, Color.green);
        Debug.DrawLine(nearClipPlaneCornerPoints.UpperRight, nearClipPlaneCornerPoints.LowerRight, Color.green);
        Debug.DrawLine(nearClipPlaneCornerPoints.LowerRight, nearClipPlaneCornerPoints.LowerLeft, Color.green);
        Debug.DrawLine(nearClipPlaneCornerPoints.LowerLeft, nearClipPlaneCornerPoints.UpperLeft, Color.green);

        // Cast lines to the collision points to see if the camera is occluded
        RaycastHit hit;
        IList<Vector3> collisionPoints = new List<Vector3>();
        collisionPoints.Add(nearClipPlaneCornerPoints.UpperLeft);
        collisionPoints.Add(nearClipPlaneCornerPoints.UpperRight);
        collisionPoints.Add(nearClipPlaneCornerPoints.LowerLeft);
        collisionPoints.Add(nearClipPlaneCornerPoints.LowerRight);
        collisionPoints.Add(cameraPosition - this.transform.forward * this.GetComponent<Camera>().nearClipPlane);

        foreach (var collisionPoint in collisionPoints)
        {
            if (Physics.Linecast(this.targetToLookAt.position, collisionPoint, out hit, this.ignorePlayerBitMask))
            {
                if (hit.distance < nearestCollisionDistance || nearestCollisionDistance == -1.0f)
                {
                    nearestCollisionDistance = hit.distance;
                }
            }
        }

        return nearestCollisionDistance;
    }

    private bool IsCameraOccluded(Vector3 cameraPosition, ref float outDistanceToTarget)
    {
        bool isOccluded = false;

        float nearestCollisionDistance = this.CheckCameraCollisionPoints(cameraPosition);

        if (nearestCollisionDistance > -1.0f)
        {
            isOccluded = true;

            outDistanceToTarget = nearestCollisionDistance - this.GetComponent<Camera>().nearClipPlane;
        }

        return isOccluded;
    }
}
