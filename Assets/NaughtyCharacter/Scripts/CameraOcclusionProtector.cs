using UnityEngine;

namespace NaughtyCharacter
{
    public class CameraOcclusionProtector : MonoBehaviour
    {
        private const float MinDistanceToPlayer = 1f;
        private const float MaxDistanceToPlayer = 5f;
        private const float MinNearClipPlaneExtentMultiplier = 1f;
        private const float MaxNearClipPlaneExtentMultiplier = 2f;
        private const float MinOcclusionMoveTime = 0f;
        private const float MaxOcclusionMoveTime = 1f;

        private struct ClipPlaneCornerPoints
        {
            public Vector3 UpperLeft { get; set; }
            public Vector3 UpperRight { get; set; }
            public Vector3 LowerLeft { get; set; }
            public Vector3 LowerRight { get; set; }
        }

        // Serializable fields
        [SerializeField]
        [Range(MinDistanceToPlayer, MaxDistanceToPlayer)]
        [Tooltip("The original distance to target (in meters)")]
        private float _distanceToTarget = 3.0f;

        [SerializeField]
        [Range(MinNearClipPlaneExtentMultiplier, MaxNearClipPlaneExtentMultiplier)]
        [Tooltip("Higher values ensure better occlusion protection, but decrease the distance between the camera and the target")]
        private float _nearClipPlaneExtentMultiplier = 1.2f;

        [SerializeField]
        [Range(MinOcclusionMoveTime, MaxOcclusionMoveTime)]
        [Tooltip("The time needed for the camera to reach secure position when an occlusion occurs (in seconds)")]
        private float _occlusionMoveTime = 0.025f;

        [SerializeField]
        [Tooltip("What objects should the camera ignore when checked for clips and occlusions")]
        private LayerMask _ignoreLayerMask = 0;

#if UNITY_EDITOR
        [SerializeField]
        private bool _visualizeInScene = true;
#endif

        // Private fields
        private Camera _camera;
        private Transform _pivot; // The point at which the camera pivots around
        private Vector3 _cameraVelocity;
        private float _nearClipPlaneHalfHeight;
        private float _nearClipPlaneHalfWidth;
        private float _sphereCastRadius;

        protected virtual void Awake()
        {
            _camera = GetComponent<Camera>();
            _pivot = transform.parent;

            float halfFOV = (_camera.fieldOfView / 2.0f) * Mathf.Deg2Rad; // vertical FOV in radians
            _nearClipPlaneHalfHeight = Mathf.Tan(halfFOV) * _camera.nearClipPlane * _nearClipPlaneExtentMultiplier;
            _nearClipPlaneHalfWidth = _nearClipPlaneHalfHeight * _camera.aspect;
            _sphereCastRadius = new Vector2(_nearClipPlaneHalfWidth, _nearClipPlaneHalfHeight).magnitude; // Pythagoras
        }

        protected virtual void LateUpdate()
        {
            UpdateCameraPosition();

#if UNITY_EDITOR
            DrawDebugVisualization();
#endif
        }

#if UNITY_EDITOR
        private void DrawDebugVisualization()
        {
            if (_visualizeInScene)
            {
                ClipPlaneCornerPoints nearClipPlaneCornerPoints = GetNearClipPlaneCornerPoints(transform.position);

                Debug.DrawLine(_pivot.position, transform.position - transform.forward * _camera.nearClipPlane, Color.red);
                Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.UpperLeft, Color.green);
                Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.UpperRight, Color.green);
                Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.LowerLeft, Color.green);
                Debug.DrawLine(_pivot.position, nearClipPlaneCornerPoints.LowerRight, Color.green);
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
            Ray ray = new Ray(_pivot.transform.position, -transform.forward);
            float rayLength = _distanceToTarget - _camera.nearClipPlane;
            RaycastHit hit;

            if (Physics.SphereCast(ray, _sphereCastRadius, out hit, rayLength, ~_ignoreLayerMask))
            {
                outDistanceToTarget = hit.distance + _sphereCastRadius;
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
            Vector3 newCameraLocalPosition = transform.localPosition;
            newCameraLocalPosition.z = -_distanceToTarget;
            Vector3 newCameraPosition = _pivot.TransformPoint(newCameraLocalPosition);
            float newDistanceToTarget = _distanceToTarget;

            if (IsCameraOccluded(newCameraPosition, ref newDistanceToTarget))
            {
                newCameraLocalPosition.z = -newDistanceToTarget;
                newCameraPosition = _pivot.TransformPoint(newCameraLocalPosition);
            }

            transform.localPosition = Vector3.SmoothDamp(
                transform.localPosition, newCameraLocalPosition, ref _cameraVelocity, _occlusionMoveTime);
        }

        private ClipPlaneCornerPoints GetNearClipPlaneCornerPoints(Vector3 cameraPosition)
        {
            ClipPlaneCornerPoints nearClipPlanePoints = new ClipPlaneCornerPoints();

            nearClipPlanePoints.UpperLeft = cameraPosition - transform.right * _nearClipPlaneHalfWidth;
            nearClipPlanePoints.UpperLeft += transform.up * _nearClipPlaneHalfHeight;
            nearClipPlanePoints.UpperLeft += transform.forward * _camera.nearClipPlane;

            nearClipPlanePoints.UpperRight = cameraPosition + transform.right * _nearClipPlaneHalfWidth;
            nearClipPlanePoints.UpperRight += transform.up * _nearClipPlaneHalfHeight;
            nearClipPlanePoints.UpperRight += transform.forward * _camera.nearClipPlane;

            nearClipPlanePoints.LowerLeft = cameraPosition - transform.right * _nearClipPlaneHalfWidth;
            nearClipPlanePoints.LowerLeft -= transform.up * _nearClipPlaneHalfHeight;
            nearClipPlanePoints.LowerLeft += transform.forward * _camera.nearClipPlane;

            nearClipPlanePoints.LowerRight = cameraPosition + transform.right * _nearClipPlaneHalfWidth;
            nearClipPlanePoints.LowerRight -= transform.up * _nearClipPlaneHalfHeight;
            nearClipPlanePoints.LowerRight += transform.forward * _camera.nearClipPlane;

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
}
