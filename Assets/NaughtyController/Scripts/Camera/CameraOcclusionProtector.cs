using UnityEngine;

public class CameraOcclusionProtector : MonoBehaviour
{
    private const float MIN_DISTANCE_TO_PLAYER = 1f;
    private const float MAX_DISTANCE_TO_PLAYER = 5f;
    private const float MIN_NEAR_CLIP_PLANE_EXTENT_MULTIPLIER = 1f;
    private const float MAX_NEAR_CLIP_PLANE_EXTENT_MULTIPLIER = 2f;
    private const float MIN_OCCLUSION_MOVE_TIME = 0f;
    private const float MAX_OCCLUSION_MOVE_TIME = 1f;

    private struct ClipPlaneCornerPoints
    {
        public Vector3 UpperLeft { get; set; }

        public Vector3 UpperRight { get; set; }

        public Vector3 LowerLeft { get; set; }

        public Vector3 LowerRight { get; set; }
    }

    // Serializable fields
    [SerializeField]
    [Range(MIN_DISTANCE_TO_PLAYER, MAX_DISTANCE_TO_PLAYER)]
    [Tooltip("The original distance to target (in meters)")]
    private float distanceToTarget = 2.5f; // In meters

    [SerializeField]
    [Range(MIN_NEAR_CLIP_PLANE_EXTENT_MULTIPLIER, MAX_NEAR_CLIP_PLANE_EXTENT_MULTIPLIER)]
    [Tooltip("Higher values ensure better occlusion protection, but decrease the distance between the camera and the target")]
    private float nearClipPlaneExtentMultiplier = 1.2f;

    [SerializeField]
    [Range(MIN_OCCLUSION_MOVE_TIME, MAX_OCCLUSION_MOVE_TIME)]
    [Tooltip("The time needed for the camera to reach secure position when an occlusion occurs (in seconds)")]
    private float occlusionMoveTime = 0.025f; // The lesser, the better

    [SerializeField]
    [Tooltip("What objects should the camera ignore when checked for clips and occlusions")]
    private LayerMask ignoreLayerMask = 0; // What objects should the camera ignore when checked for clips and occlusions

#if UNITY_EDITOR
    [SerializeField]
    private bool visualizeInScene = true;
#endif

    // Private fields
    private new Camera camera;
    private Transform pivot; // The point at which the camera pivots around
    private Vector3 cameraVelocity;
    private float nearClipPlaneHalfHeight;
    private float nearClipPlaneHalfWidth;
    private float sphereCastRadius;

    protected virtual void Awake()
    {
        this.camera = this.GetComponent<Camera>();
        this.pivot = this.transform.parent;

        float halfFOV = (this.camera.fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
        this.nearClipPlaneHalfHeight = Mathf.Tan(halfFOV) * this.camera.nearClipPlane * this.nearClipPlaneExtentMultiplier;
        this.nearClipPlaneHalfWidth = nearClipPlaneHalfHeight * this.camera.aspect;
        this.sphereCastRadius = new Vector2(this.nearClipPlaneHalfWidth, this.nearClipPlaneHalfHeight).magnitude; // Pythagoras
    }

    protected virtual void LateUpdate()
    {
        this.UpdateCameraPosition();

#if UNITY_EDITOR
        this.DrawDebugVisualization();
#endif
    }

#if UNITY_EDITOR
    private void DrawDebugVisualization()
    {
        if (this.visualizeInScene)
        {
            ClipPlaneCornerPoints nearClipPlaneCornerPoints = this.GetNearClipPlaneCornerPoints(this.transform.position);

            Debug.DrawLine(this.pivot.position, this.transform.position - this.transform.forward * this.camera.nearClipPlane, Color.red);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.UpperLeft, Color.green);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.UpperRight, Color.green);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.LowerLeft, Color.green);
            Debug.DrawLine(this.pivot.position, nearClipPlaneCornerPoints.LowerRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.UpperLeft, nearClipPlaneCornerPoints.UpperRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.UpperRight, nearClipPlaneCornerPoints.LowerRight, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.LowerRight, nearClipPlaneCornerPoints.LowerLeft, Color.green);
            Debug.DrawLine(nearClipPlaneCornerPoints.LowerLeft, nearClipPlaneCornerPoints.UpperLeft, Color.green);
        }
    }
#endif

    /// <summary>
    /// Checks if the camera is Occluded.
    /// </summary>
    /// <param name="cameraPosition"> The position of the camera</param>
    /// <param name="outDistanceToTarget"> if the camera is occluded, the new distance to target is saved in this variable</param>
    /// <returns></returns>
    private bool IsCameraOccluded(Vector3 cameraPosition, ref float outDistanceToTarget)
    {
        // Cast a sphere along a ray to see if the camera is occluded
        Ray ray = new Ray(this.pivot.transform.position, -this.transform.forward);
        float rayLength = this.distanceToTarget - this.camera.nearClipPlane;
        RaycastHit hit;

        if (Physics.SphereCast(ray, this.sphereCastRadius, out hit, rayLength, ~this.ignoreLayerMask))
        {
            outDistanceToTarget = hit.distance + this.sphereCastRadius;
            return true;
        }
        else
        {
            outDistanceToTarget = -1f;
            return false;
        }
    }

    private void UpdateCameraPosition()
    {
        Vector3 newCameraLocalPosition = this.transform.localPosition;
        newCameraLocalPosition.z = -this.distanceToTarget;
        Vector3 newCameraPosition = this.pivot.TransformPoint(newCameraLocalPosition);
        float newDistanceToTarget = this.distanceToTarget;
        
        if (this.IsCameraOccluded(newCameraPosition, ref newDistanceToTarget))
        {
            newCameraLocalPosition.z = -newDistanceToTarget;
            newCameraPosition = this.pivot.TransformPoint(newCameraLocalPosition);
        }
        
        this.transform.localPosition = Vector3.SmoothDamp(
            this.transform.localPosition, newCameraLocalPosition, ref this.cameraVelocity, this.occlusionMoveTime);
    }

    private ClipPlaneCornerPoints GetNearClipPlaneCornerPoints(Vector3 cameraPosition)
    {
        ClipPlaneCornerPoints nearClipPlanePoints = new ClipPlaneCornerPoints();

        nearClipPlanePoints.UpperLeft = cameraPosition - this.transform.right * nearClipPlaneHalfWidth;
        nearClipPlanePoints.UpperLeft += this.transform.up * nearClipPlaneHalfHeight;
        nearClipPlanePoints.UpperLeft += this.transform.forward * this.camera.nearClipPlane;

        nearClipPlanePoints.UpperRight = cameraPosition + this.transform.right * nearClipPlaneHalfWidth;
        nearClipPlanePoints.UpperRight += this.transform.up * nearClipPlaneHalfHeight;
        nearClipPlanePoints.UpperRight += this.transform.forward * this.camera.nearClipPlane;

        nearClipPlanePoints.LowerLeft = cameraPosition - this.transform.right * nearClipPlaneHalfWidth;
        nearClipPlanePoints.LowerLeft -= this.transform.up * nearClipPlaneHalfHeight;
        nearClipPlanePoints.LowerLeft += this.transform.forward * this.camera.nearClipPlane;

        nearClipPlanePoints.LowerRight = cameraPosition + this.transform.right * nearClipPlaneHalfWidth;
        nearClipPlanePoints.LowerRight -= this.transform.up * nearClipPlaneHalfHeight;
        nearClipPlanePoints.LowerRight += this.transform.forward * this.camera.nearClipPlane;

        return nearClipPlanePoints;
    }

    //private void OnDrawGizmos()
    //{
    //    if (Application.isPlaying)
    //    {
    //        Gizmos.color = Color.yellow;
    //        Gizmos.DrawSphere(this.pivot.transform.position - (this.transform.forward * (this.distanceToTarget - this.camera.nearClipPlane)), this.sphereCastRadius);
    //    }
    //}
}
