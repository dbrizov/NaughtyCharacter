using UnityEngine;

namespace NaughtyCharacter
{
    public class PlayerController : Controller
    {
        public PlayerCamera PlayerCamera;
        public float ControlRotationSensitivity = 3.0f;

        private PlayerInput _playerInput;

        public override void Init()
        {
            base.Init();

            _playerInput = GetComponent<PlayerInput>();
        }

        public override void OnInputUpdate()
        {
            _playerInput.UpdateInput();
        }

        public override void OnBeforeCharacterMoved()
        {
            UpdateControlRotation();
            PlayerCamera.SetControlRotation(_character.GetControlRotation());

            Vector3 movementInput = GetMovementInput();
            _character.SetMovementInput(movementInput);

            _character.SetJumpInput(_playerInput.JumpInput);
        }

        public override void OnAfterCharacterMoved()
        {
            PlayerCamera.SetPosition(_character.transform.position);
        }

        private void UpdateControlRotation()
        {
            Vector2 camInput = _playerInput.CameraInput;
            Vector2 controlRotation = _character.GetControlRotation();

            // Adjust the pitch angle (X Rotation)
            float pitchAngle = controlRotation.x;
            pitchAngle -= camInput.y * ControlRotationSensitivity;

            // Adjust the yaw angle (Y Rotation)
            float yawAngle = controlRotation.y;
            yawAngle += camInput.x * ControlRotationSensitivity;

            controlRotation = new Vector2(pitchAngle, yawAngle);
            _character.SetControlRotation(controlRotation);
        }

        private Vector3 GetMovementInput()
        {
            // Calculate the move direction relative to camera's yaw rotation
            Vector3 cameraForward = PlayerCamera.Camera.transform.forward.SetY(0.0f).normalized;
            Vector3 cameraRight = PlayerCamera.Camera.transform.right.SetY(0.0f).normalized;
            Vector3 movementInput = (cameraForward * _playerInput.MoveInput.y + cameraRight * _playerInput.MoveInput.x);

            if (movementInput.sqrMagnitude > 1f)
            {
                movementInput.Normalize();
            }

            return movementInput;
        }
    }
}