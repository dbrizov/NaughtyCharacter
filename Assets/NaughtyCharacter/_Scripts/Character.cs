using UnityEngine;

namespace NaughtyCharacter
{
    public enum ERotationBehavior
    {
        OrientRotationToMovement,
        UseControlRotation
    }

    [System.Serializable]
    public class RotationSettings
    {
        [Header("Control Rotation")]
        public float MinPitchAngle = -45.0f;
        public float MaxPitchAngle = 75.0f;

        [Header("Character Orientation")]
        public ERotationBehavior RotationBehavior = ERotationBehavior.OrientRotationToMovement;
        public float MinRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
        public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)
    }

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
        public float Gravity = 20.0f; // Gravity applied when the player is airborne
        public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
        public float MaxFallSpeed = 40.0f; // The max speed at which the player can fall
    }

    [System.Serializable]
    public class GroundSettings
    {
        public LayerMask GroundLayers; // Which layers are considered as ground
        public float SphereCastRadius = 0.35f; // The radius of the sphere cast for the grounded check
        public float SphereCastDistance = 0.15f; // The distance below the character's capsule used for the sphere cast grounded check
    }

    public class Character : MonoBehaviour
    {
        public Controller Controller; // The controller that controls the character
        public MovementSettings MovementSettings;
        public GravitySettings GravitySettings;
        public RotationSettings RotationSettings;
        public GroundSettings GroundSettings;

        private CharacterController _characterController; // The Unity's CharacterController
        private CharacterAnimator _characterAnimator;

        private float _targetHorizontalSpeed; // In meters/second
        private float _horizontalSpeed; // In meters/second
        private float _verticalSpeed; // In meters/second
        private bool _justWalkedOffALedge;

        private Vector2 _controlRotation; // X (Pitch), Y (Yaw)
        private Vector3 _movementInput;
        private Vector3 _lastMovementInput;
        private bool _hasMovementInput;
        private bool _jumpInput;

        public Vector3 Velocity => _characterController.velocity;
        public Vector3 HorizontalVelocity => _characterController.velocity.SetY(0.0f);
        public Vector3 VerticalVelocity => _characterController.velocity.Multiply(0.0f, 1.0f, 0.0f);
        public bool IsGrounded { get; private set; }

        private void Awake()
        {
            Controller.Init();
            Controller.Character = this;

            _characterController = GetComponent<CharacterController>();
            _characterAnimator = GetComponent<CharacterAnimator>();
        }

        private void Update()
        {
            Controller.OnCharacterUpdate();
        }

        private void FixedUpdate()
        {
            Tick(Time.deltaTime);
            Controller.OnCharacterFixedUpdate();
        }

        private void Tick(float deltaTime)
        {
            UpdateHorizontalSpeed(deltaTime);
            UpdateVerticalSpeed(deltaTime);

            Vector3 movement = _horizontalSpeed * GetMovementDirection() + _verticalSpeed * Vector3.up;
            _characterController.Move(movement * deltaTime);

            OrientToTargetRotation(movement.SetY(0.0f), deltaTime);

            UpdateGrounded();

            _characterAnimator.UpdateState();
        }

        public void SetMovementInput(Vector3 movementInput)
        {
            bool hasMovementInput = movementInput.sqrMagnitude > 0.0f;

            if (_hasMovementInput && !hasMovementInput)
            {
                _lastMovementInput = _movementInput;
            }

            _movementInput = movementInput;
            _hasMovementInput = hasMovementInput;
        }

        public void SetJumpInput(bool jumpInput)
        {
            _jumpInput = jumpInput;
        }

        public Vector2 GetControlRotation()
        {
            return _controlRotation;
        }

        public void SetControlRotation(Vector2 controlRotation)
        {
            // Adjust the pitch angle (X Rotation)
            float pitchAngle = controlRotation.x;
            pitchAngle %= 360.0f;
            pitchAngle = Mathf.Clamp(pitchAngle, RotationSettings.MinPitchAngle, RotationSettings.MaxPitchAngle);

            // Adjust the yaw angle (Y Rotation)
            float yawAngle = controlRotation.y;
            yawAngle %= 360.0f;

            _controlRotation = new Vector2(pitchAngle, yawAngle);
        }

        private bool CheckGrounded()
        {
            Vector3 spherePosition = transform.position;
            spherePosition.y = transform.position.y + GroundSettings.SphereCastRadius - GroundSettings.SphereCastDistance;
            bool isGrounded = Physics.CheckSphere(spherePosition, GroundSettings.SphereCastRadius, GroundSettings.GroundLayers, QueryTriggerInteraction.Ignore);

            return isGrounded;
        }

        private void UpdateGrounded()
        {
            _justWalkedOffALedge = false;

            bool isGrounded = CheckGrounded();
            if (IsGrounded && !isGrounded && !_jumpInput)
            {
                _justWalkedOffALedge = true;
            }

            IsGrounded = isGrounded;
        }

        private void UpdateHorizontalSpeed(float deltaTime)
        {
            Vector3 movementInput = _movementInput;
            if (movementInput.sqrMagnitude > 1.0f)
            {
                movementInput.Normalize();
            }

            _targetHorizontalSpeed = movementInput.magnitude * MovementSettings.MaxHorizontalSpeed;
            float acceleration = _hasMovementInput ? MovementSettings.Acceleration : MovementSettings.Decceleration;

            _horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * deltaTime);
        }

        private void UpdateVerticalSpeed(float deltaTime)
        {
            if (IsGrounded)
            {
                _verticalSpeed = -GravitySettings.GroundedGravity;

                if (_jumpInput)
                {
                    _verticalSpeed = MovementSettings.JumpSpeed;
                }
            }
            else
            {
                if (!_jumpInput && _verticalSpeed > 0.0f)
                {
                    // This is what causes holding jump to jump higher than tapping jump.
                    _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, MovementSettings.JumpAbortSpeed * deltaTime);
                }
                else if (_justWalkedOffALedge)
                {
                    _verticalSpeed = 0.0f;
                }

                _verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, GravitySettings.Gravity * deltaTime);
            }
        }

        private Vector3 GetMovementDirection()
        {
            Vector3 moveDir = _hasMovementInput ? _movementInput : _lastMovementInput;
            if (moveDir.sqrMagnitude > 1f)
            {
                moveDir.Normalize();
            }

            return moveDir;
        }

        private void OrientToTargetRotation(Vector3 horizontalMovement, float deltaTime)
        {
            if (RotationSettings.RotationBehavior == ERotationBehavior.OrientRotationToMovement && horizontalMovement.sqrMagnitude > 0.0f)
            {
                float rotationSpeed = Mathf.Lerp(
                    RotationSettings.MaxRotationSpeed, RotationSettings.MinRotationSpeed, _horizontalSpeed / _targetHorizontalSpeed);

                Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);
                transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * deltaTime);
            }
            else if (RotationSettings.RotationBehavior == ERotationBehavior.UseControlRotation)
            {
                Quaternion targetRotation = Quaternion.Euler(0.0f, _controlRotation.y, 0.0f);
                transform.rotation = targetRotation;
            }
        }
    }
}
