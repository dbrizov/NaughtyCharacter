using UnityEngine;

namespace NaughtyCharacter
{
    public static class AnimatorParamId
    {
        public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
        public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
        public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    }

    public class PlayerAnimator : MonoBehaviour
    {
        private Animator _animator;
        private PlayerController _playerController;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerController = GetComponent<PlayerController>();
        }

        public void UpdateState()
        {
            float normHorizontalSpeed = _playerController.HorizontalVelocity.magnitude / _playerController.MovementSettings.MaxHorizontalSpeed;
            _animator.SetFloat(AnimatorParamId.HorizontalSpeed, normHorizontalSpeed);

            float jumpSpeed = _playerController.MovementSettings.JumpSpeed;
            float normVerticalSpeed = _playerController.VerticalVelocity.y.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);
            _animator.SetFloat(AnimatorParamId.VerticalSpeed, normVerticalSpeed);

            _animator.SetBool(AnimatorParamId.IsGrounded, _playerController.IsGrounded);
        }
    }
}
