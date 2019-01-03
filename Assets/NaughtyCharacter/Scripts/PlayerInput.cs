using UnityEngine;

namespace NaughtyCharacter
{
    public class PlayerInput : MonoBehaviour
    {
        public Vector2 MoveInput { get; private set; }
        public Vector2 LastMoveInput { get; private set; }
        public Vector2 CameraInput { get; private set; }
        public bool JumpInput { get; private set; }

        public bool HasMoveInput { get; private set; }

        public void UpdateInput()
        {
            Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            bool hasMoveInput = moveInput.sqrMagnitude > 0.0f;

            if (HasMoveInput && !hasMoveInput)
            {
                LastMoveInput = MoveInput;
            }

            MoveInput = moveInput;
            CameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            JumpInput = Input.GetButton("Jump");

            HasMoveInput = hasMoveInput;
        }
    }
}
