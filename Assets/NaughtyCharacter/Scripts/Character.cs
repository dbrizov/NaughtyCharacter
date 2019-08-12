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
		public float Gravity = 20.0f; // Gravity applied when the player is airborne
		public float GroundedGravity = 5.0f; // A constant gravity that is applied when the player is grounded
		public float MaxFallSpeed = 40.0f; // The max speed at which the player can fall
	}

	[System.Serializable]
	public class RotationSettings
	{
		[Header("Control Rotation")]
		public float MinPitchAngle = -45.0f;
		public float MaxPitchAngle = 75.0f;

		[Header("Character Orientation")]
		[SerializeField] private bool _useControlRotation = false;
		[SerializeField] private bool _orientRotationToMovement = true;
		public float MinRotationSpeed = 600.0f; // The turn speed when the player is at max speed (in degrees/second)
		public float MaxRotationSpeed = 1200.0f; // The turn speed when the player is stationary (in degrees/second)

		public bool UseControlRotation { get { return _useControlRotation; } set { SetUseControlRotation(value); } }
		public bool OrientRotationToMovement { get { return _orientRotationToMovement; } set { SetOrientRotationToMovement(value); } }

		private void SetUseControlRotation(bool useControlRotation)
		{
			_useControlRotation = useControlRotation;
			_orientRotationToMovement = !_useControlRotation;
		}

		private void SetOrientRotationToMovement(bool orientRotationToMovement)
		{
			_orientRotationToMovement = orientRotationToMovement;
			_useControlRotation = !_orientRotationToMovement;
		}
	}

	public class Character : MonoBehaviour
	{
		public Controller Controller; // The controller that controls the character
		public MovementSettings MovementSettings;
		public GravitySettings GravitySettings;
		public RotationSettings RotationSettings;

		private CharacterController _characterController; // The Unity's CharacterController
		private CharacterAnimator _characterAnimator;

		private float _targetHorizontalSpeed; // In meters/second
		private float _horizontalSpeed; // In meters/second
		private float _verticalSpeed; // In meters/second

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
			UpdateState();
			Controller.OnCharacterFixedUpdate();
		}

		private void UpdateState()
		{
			UpdateHorizontalSpeed();
			UpdateVerticalSpeed();

			Vector3 movement = _horizontalSpeed * GetMovementDirection() + _verticalSpeed * Vector3.up;
			_characterController.Move(movement * Time.deltaTime);

			OrientToTargetRotation(movement.SetY(0.0f));

			IsGrounded = _characterController.isGrounded;

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

		private void UpdateHorizontalSpeed()
		{
			Vector3 movementInput = _movementInput;
			if (movementInput.sqrMagnitude > 1.0f)
			{
				movementInput.Normalize();
			}

			_targetHorizontalSpeed = movementInput.magnitude * MovementSettings.MaxHorizontalSpeed;
			float acceleration = _hasMovementInput ? MovementSettings.Acceleration : MovementSettings.Decceleration;

			_horizontalSpeed = Mathf.MoveTowards(_horizontalSpeed, _targetHorizontalSpeed, acceleration * Time.deltaTime);
		}

		private void UpdateVerticalSpeed()
		{
			if (IsGrounded)
			{
				_verticalSpeed = -GravitySettings.GroundedGravity;

				if (_jumpInput)
				{
					_verticalSpeed = MovementSettings.JumpSpeed;
					IsGrounded = false;
				}
			}
			else
			{
				if (!_jumpInput && _verticalSpeed > 0.0f)
				{
					// This is what causes holding jump to jump higher than tapping jump.
					_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, MovementSettings.JumpAbortSpeed * Time.deltaTime);
				}

				_verticalSpeed = Mathf.MoveTowards(_verticalSpeed, -GravitySettings.MaxFallSpeed, GravitySettings.Gravity * Time.deltaTime);
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

		private void OrientToTargetRotation(Vector3 horizontalMovement)
		{
			if (RotationSettings.OrientRotationToMovement && horizontalMovement.sqrMagnitude > 0.0f)
			{
				float rotationSpeed = Mathf.Lerp(
					RotationSettings.MaxRotationSpeed, RotationSettings.MinRotationSpeed, _horizontalSpeed / _targetHorizontalSpeed);

				Quaternion targetRotation = Quaternion.LookRotation(horizontalMovement, Vector3.up);

				transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
			}
			else if (RotationSettings.UseControlRotation)
			{
				Quaternion targetRotation = Quaternion.Euler(0.0f, _controlRotation.y, 0.0f);
				transform.rotation = targetRotation;
			}
		}
	}
}
