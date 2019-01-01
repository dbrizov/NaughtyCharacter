using UnityEngine;

namespace NaughtyCharacter
{
    public class CameraController : MonoBehaviour
    {
        [SerializeField]
        [Tooltip("The target to follow")]
        private Transform _target = null;

        [SerializeField]
        [Range(0.0f, 1.0f)]
        private float _catchSpeedDamp = 0.0f;

        [SerializeField]
        [Tooltip("How fast the camera rotates around the pivot. Value <= 0 are interpreted as instant rotation")]
        private float _rotationSpeed = 0.0f;

        [SerializeField]
        private float _mouseSensitivity = 3.0f;
        [SerializeField]
        private float _minTiltAngle = -75.0f;
        [SerializeField]
        private float _maxTiltAngle = 45.0f;

        private float _lookAngle;
        private float _tiltAngle;
        private Transform _rig; // The root transform of the camera rig
        private Transform _pivot; // The point at which the camera pivots around
        private Quaternion _pivotTargetLocalRotation; // Controls the X Rotation (Tilt Rotation)
        private Quaternion _rigTargetLocalRotation; // Controls the Y Rotation (Look Rotation)
        private Vector3 _cameraVelocity; // The velocity at which the camera moves

        private void Awake()
        {
            _pivot = transform.parent;
            _rig = _pivot.parent;

            transform.localRotation = Quaternion.identity;
        }

        private void Update()
        {
            var controlRotation = GetControlRotation();
            UpdateRotation(controlRotation);
        }

        private void FixedUpdate()
        {
            FollowTarget();
        }

        private void FollowTarget()
        {
            _rig.position = Vector3.SmoothDamp(_rig.position, _target.transform.position, ref _cameraVelocity, _catchSpeedDamp);
        }

        private void UpdateRotation(Quaternion controlRotation)
        {
            if (_target != null)
            {
                // Y Rotation (Look Rotation)
                _rigTargetLocalRotation = Quaternion.Euler(0f, controlRotation.eulerAngles.y, 0f);

                // X Rotation (Tilt Rotation)
                _pivotTargetLocalRotation = Quaternion.Euler(controlRotation.eulerAngles.x, 0f, 0f);

                if (_rotationSpeed > 0.0f)
                {
                    _pivot.localRotation =
                        Quaternion.Slerp(_pivot.localRotation, _pivotTargetLocalRotation, _rotationSpeed * Time.deltaTime);

                    _rig.localRotation =
                        Quaternion.Slerp(_rig.localRotation, _rigTargetLocalRotation, _rotationSpeed * Time.deltaTime);
                }
                else
                {
                    _pivot.localRotation = _pivotTargetLocalRotation;
                    _rig.localRotation = _rigTargetLocalRotation;
                }
            }
        }

        private Quaternion GetControlRotation()
        {
            Vector2 camInput = PlayerInput.CameraInput;

            // Adjust the look angle (Y Rotation)
            _lookAngle += camInput.x * _mouseSensitivity;
            _lookAngle %= 360.0f;

            // Adjust the tilt angle (X Rotation)
            _tiltAngle += camInput.y * _mouseSensitivity;
            _tiltAngle %= 360.0f;
            _tiltAngle = Util.ClampAngle(_tiltAngle, _minTiltAngle, _maxTiltAngle);

            var controlRotation = Quaternion.Euler(_tiltAngle * -1.0f, _lookAngle, 0.0f);
            return controlRotation;
        }
    }
}
