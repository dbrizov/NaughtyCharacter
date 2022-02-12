using UnityEngine;
using UnityEngine.InputSystem;

namespace NaughtyCharacter
{
    public class TestHelper : MonoBehaviour
    {
        public InputAction SlowMotionAction;

        [SerializeField]
        private int _targetFrameRate = 60;

        [SerializeField]
        private float _slowMotion = 0.1f;

        private void Awake()
        {
            Application.targetFrameRate = _targetFrameRate;
        }

        private void OnEnable()
        {
            SlowMotionAction.Enable();
            SlowMotionAction.started += ToggleSlowMotion;
        }

        private void OnDisable()
        {
            SlowMotionAction.Disable();
            SlowMotionAction.started -= ToggleSlowMotion;
        }

        private void ToggleSlowMotion(InputAction.CallbackContext context)
        {
            Time.timeScale = Time.timeScale == 1f ? _slowMotion : 1f;
        }
    }
}
