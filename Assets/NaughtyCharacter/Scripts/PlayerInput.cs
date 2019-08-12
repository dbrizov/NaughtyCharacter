using UnityEngine;

namespace NaughtyCharacter
{
	public class PlayerInput : MonoBehaviour
	{
		public float MoveAxisDeadZone = 0.2f;

		public Vector2 MoveInput { get; private set; }
		public Vector2 LastMoveInput { get; private set; }
		public Vector2 CameraInput { get; private set; }
		public bool JumpInput { get; private set; }

		public bool HasMoveInput { get; private set; }

		public void UpdateInput()
		{
			// Update MoveInput
			Vector2 moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
			if (Mathf.Abs(moveInput.x) < MoveAxisDeadZone)
			{
				moveInput.x = 0.0f;
			}

			if (Mathf.Abs(moveInput.y) < MoveAxisDeadZone)
			{
				moveInput.y = 0.0f;
			}

			bool hasMoveInput = moveInput.sqrMagnitude > 0.0f;

			if (HasMoveInput && !hasMoveInput)
			{
				LastMoveInput = MoveInput;
			}

			MoveInput = moveInput;
			HasMoveInput = hasMoveInput;

			// Update other inputs
			CameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
			JumpInput = Input.GetButton("Jump");
		}
	}
}
