using UnityEngine;

namespace NaughtyCharacter
{
    [System.Serializable]
    public class MovementSettings
    {
        public float Acceleration = 25.0f; // In meters/second
        public float Decceleration = 25.0f; // In meters/second
        public float MaxHorizontalSpeed = 8.0f; // In meters/second
        public float JumpSpeed = 10.0f; // In meters/second
        public float JumpAbortSpeed = 10.0f; // In meters/second
    }

    [System.Serializable]
    public class GravitySettings
    {
        public float Gravity = 20f; // Gravity applied when the player is airborne
        public float GroundedGravity = 7f; // A constant gravity that is applied when the player is grounded
        public float MaxFallSpeed = 40f; // The max speed at which the player can fall
    }

    [System.Serializable]
    public class RotationSettings
    {
        [Header("Character Rotation")]
        public bool OrientRotationToMovement = true;
        public float MinRotationSpeed = 400.0f; // The turn speed when the player is at max speed (in degrees/second)
        public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)

        [Header("Control Rotation")]
        public float ControlRotationSensitivity = 3.0f;
        public float MinPitchAngle = -45.0f;
        public float MaxPitchAngle = 75.0f;
    }

    public enum ControllerState
    {
        Idle,
        Running,
        Airborne
    }

    public class PlayerController : MonoBehaviour
    {
        public PlayerCamera PlayerCamera;
        public MovementSettings MovementSettings;
        public GravitySettings GravitySettings;
        public RotationSettings RotationSettings;

        private PlayerInput _playerInput;
        private PlayerAnimator _playerAnimator;
        private CharacterController _characterController;

        private float _targetHorizontalSpeed; // In meters/second
        private float _horizontalSpeed; // In meters/second
        private float _verticalSpeed; // In meters/second
        private bool _isGrounded;

        public ControllerState State { get; private set; }
        public ControllerState PrevState { get; private set; }
        public Vector3 Velocity => _characterController.velocity;
        public Vector3 HorizontalVelocity => _characterController.velocity.SetY(0.0f);
        public Vector3 VerticalVelocity => _characterController.velocity.Multiply(0.0f, 1.0f, 0.0f);
        public Vector2 ControlRotation { get; private set; } // X (Pitch), Y (Yaw)

        private void Awake()
        {
            _playerInput = GetComponent<PlayerInput>();
            _playerAnimator = GetComponent<PlayerAnimator>();
            _characterController = GetComponent<CharacterController>();
        }

        private void Update()
        {
            _playerInput.UpdateInput();

            UpdateControlRotation();
            PlayerCamera.SetControlRotation(ControlRotation);

            UpdateState();
            UpdateHorizontalSpeed();
            UpdateVerticalSpeed();

            Vector3 movement = _horizontalSpeed * GetMovementVector() + _verticalSpeed * Vector3.up;
            _characterController.Move(movement * Time.deltaTime);

            OrientToTargetRotation(movement.SetY(0.0f));
            PlayerCamera.SetPosition(transform.position);

            _isGrounded = _characterController.isGrounded;

            _playerAnimator.UpdateState();
        }

        private void UpdateState()
        {
            PrevState = State;

            if (_isGrounded)
            {
                if (Velocity.sqrMagnitude > 0.0f)
                {
                    State = ControllerState.Running;
                }
                else
                {
                    State = ControllerState.Idle;
                }
            }
            else
            {
                State = ControllerState.Airborne;
            }
        }

        private void UpdateHorizontalSpeed()
        {
            Vector2 moveInput = _playerInput.MoveInput;
            if (moveInput.sqrMagnitude > 1.0f)
            {
                moveInput.Normalize();
            }

            _targetHorizontalSpeed = moveInput.magnitude * MovementSettings.MaxHorizontalSpeed;
            float acceleration = _playerInput.HasMoveInput ? MovementSettings.Acceleration : MovementSettings.Decceleration;

            _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * Time.deltaTime);
        }

        private void UpdateVerticalSpeed()
        {
            if (_isGrounded)
            {
                _verticalSpeed = -GravitySettings.GroundedGravity;

                if (_playerInput.JumpInput)
                {
                    _verticalSpeed = MovementSettings.JumpSpeed;
                    _isGrounded = false;
                }
            }
            else
            {
                if (!_playerInput.JumpInput && _verticalSpeed > 0.0f)
                {
                    // This is what causes holding jump to jump higher than tapping jump.
                    _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, MovementSettings.JumpAbortSpeed * Time.deltaTime);
                }

                _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, GravitySettings.Gravity * Time.deltaTime);
            }
        }

        private void UpdateControlRotation()
        {
            Vector2 camInput = _playerInput.CameraInput;

            // Adjust the yaw angle (Y Rotation)
            float yawAngle = ControlRotation.y;
            yawAngle += camInput.x * RotationSettings.ControlRotationSensitivity;
            yawAngle %= 360.0f;

            // Adjust the pitch angle (X Rotation)
            float pitchAngle = ControlRotation.x;
            pitchAngle -= camInput.y * RotationSettings.ControlRotationSensitivity;
            pitchAngle %= 360.0f;
            pitchAngle = Mathf.Clamp(pitchAngle, RotationSettings.MinPitchAngle, RotationSettings.MaxPitchAngle);

            ControlRotation = new Vector2(pitchAngle, yawAngle);
        }

        private void OrientToTargetRotation(Vector3 horizontalMovement)
        {
            if (RotationSettings.OrientRotationToMovement && horizontalMovement.sqrMagnitude > 0.0f)
            {
                float rotationSpeed = Mathf.Lerp(
                    RotationSettings.MaxRotationSpeed, RotationSettings.MinRotationSpeed, _horizontalSpeed / _targetHorizontalSpeed);

                Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);

                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }
        }

        private Vector3 GetMovementVector()
        {
            // Calculate the move direction relative to camera's yaw rotation
            Vector3 cameraForward = PlayerCamera.Camera.transform.forward.SetY(0.0f).normalized;
            Vector3 cameraRight = PlayerCamera.Camera.transform.right.SetY(0.0f).normalized;

            Vector2 moveInput = _playerInput.HasMoveInput ? _playerInput.MoveInput : _playerInput.LastMoveInput;
            Vector3 moveDir = (cameraForward * moveInput.y + cameraRight * moveInput.x);

            if (moveDir.sqrMagnitude > 1f)
            {
                moveDir.Normalize();
            }

            return moveDir;
        }
    }
}