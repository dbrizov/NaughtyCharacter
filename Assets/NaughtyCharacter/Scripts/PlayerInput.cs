using UnityEngine;

namespace NaughtyCharacter
{
    public class PlayerInput : MonoBehaviour
    {
        private static PlayerInput _instance;

        private Vector2 _moveInput;
        private Vector2 _cameraInput;
        private bool _jumpInput;

        public static Vector2 MoveInput => _instance._moveInput;
        public static Vector2 CameraInput => _instance._cameraInput;
        public static bool JumpInput => _instance._jumpInput;

        public static bool HasMoveInput => _instance._moveInput.sqrMagnitude > 0.0f;

        private void Awake()
        {
            if (_instance != null)
            {
                Destroy(gameObject);
                return;
            }

            _instance = this;
        }

        private void Update()
        {
            _moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));
            _cameraInput = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
            _jumpInput = Input.GetButton("Jump");
        }
    }
}
