using UnityEngine;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(Camera))]
public class CameraOcclusionProtector : MonoBehaviour
{
    private struct ClipPlaneCornerPoints
    {
        public Vector3 UpperLeft { get; set; }

        public Vector3 UpperRight { get; set; }

        public Vector3 LowerLeft { get; set; }

        public Vector3 LowerRight { get; set; }
    }

    [SerializeField]
    private float occlusionMoveTime = 0.05f; // The lesser, the better

    [SerializeField]
    [Tooltip("What objects should the camera ignore when checked for clips and occlusions")]
    private LayerMask ignoreLayerMask; // What objects should the camera ignore when checked for clips and occlusions

    [SerializeField]
    private int maxOcclusionChecks = 5;

    [SerializeField]
    private bool visualizeInScene = false;

    private Camera followCamera;
    private Transform followCameraTransform;
    private Transform pivot; // The point at which the camera pivots around
    private float originalDistanceToTarget;
    private float distanceToTarget;
    private Vector3 initialCameraLocalPosition;
    private Vector3 cameraVelocity;
    private int currentOcclusionCheck;

    protected virtual void Awake()
    {
        this.followCamera = this.GetComponent<Camera>();
        this.followCameraTransform = this.followCamera.transform;
        this.pivot = this.followCamera.transform.parent;

        this.initialCameraLocalPosition = this.followCameraTransform.localPosition;
    }

    protected virtual void Start()
    {
        this.originalDistanceToTarget = this.followCameraTransform.localPosition.magnitude;
        this.distanceToTarget = this.originalDistanceToTarget;
    }

    protected virtual void LateUpdate()
    {
        this.distanceToTarget = this.originalDistanceToTarget;

        this.currentOcclusionCheck = 0;
        while (this.IsCameraOccluded(this.followCameraTransform.position, ref this.distanceToTarget) &&
               this.currentOcclusionCheck < this.maxOcclusionChecks)
        {
            this.currentOcclusionCheck++;
        }

        //this.UpdateCameraPosition();
    }

    private ClipPlaneCornerPoints GetNearClipPlaneCornerPoints(Vector3 cameraPosition)
    {
        ClipPlaneCornerPoints nearClipPlanePoints = new ClipPlaneCornerPoints();

        float halfFOV = (this.followCamera.fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
        float aspectRatio = this.followCamera.aspect; // viewportWidth / viewportHeight
        float distanceToNearClipPlane = this.followCamera.nearClipPlane;
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

        if (this.visualizeInScene)
        {
            // Draw the lines to the collision points for debugging
            Debug.DrawLine(this.pivot.position, cameraPosition - this.transform.forward * this.followCamera.nearClipPlane, Color.red);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.UpperLeft, Color.green);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.UpperRight, Color.green);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.LowerLeft, Color.green);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.LowerRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.UpperLeft, nearClipPlaneCornerPoints.UpperRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.UpperRight, nearClipPlaneCornerPoints.LowerRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.LowerRight, nearClipPlaneCornerPoints.LowerLeft, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.LowerLeft, nearClipPlaneCornerPoints.UpperLeft, Color.green);
        }

        // Cast lines to the collision points to see if the camera is occluded
        RaycastHit hit;
        List<Vector3> collisionPoints = new List<Vector3>();
        collisionPoints.Add(nearClipPlaneCornerPoints.UpperLeft);
        collisionPoints.Add(nearClipPlaneCornerPoints.UpperRight);
        collisionPoints.Add(nearClipPlaneCornerPoints.LowerLeft);
        collisionPoints.Add(nearClipPlaneCornerPoints.LowerRight);
        collisionPoints.Add(cameraPosition - this.transform.forward * this.followCamera.nearClipPlane);

        foreach (var collisionPoint in collisionPoints)
        {
            if (Physics.Linecast(this.pivot.position, collisionPoint, out hit, ~this.ignoreLayerMask))
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

            outDistanceToTarget = nearestCollisionDistance - this.followCamera.nearClipPlane;
        }

        return isOccluded;
    }

    private void UpdateCameraPosition()
    {
        Vector3 cameraTargetLocalPosition = this.initialCameraLocalPosition;

        if (this.distanceToTarget > -1.0f)
        {
            cameraTargetLocalPosition.z = -this.distanceToTarget;
        }
        else
        {
            cameraTargetLocalPosition.z = -this.originalDistanceToTarget;
        }

        this.followCameraTransform.localPosition = Vector3.SmoothDamp(
            this.followCameraTransform.localPosition, cameraTargetLocalPosition, ref this.cameraVelocity, this.occlusionMoveTime);
    }
}
