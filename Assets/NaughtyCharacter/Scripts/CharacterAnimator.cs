using UnityEngine;

namespace NaughtyCharacter
{
	public static class CharacterAnimatorParamId
	{
		public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
		public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
		public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
	}

	public class CharacterAnimator : MonoBehaviour
	{
		private Animator _animator;
		private Character _character;

		private void Awake()
		{
			_animator = GetComponent<Animator>();
			_character = GetComponent<Character>();
		}

		public void UpdateState()
		{
			float normHorizontalSpeed = _character.HorizontalVelocity.magnitude / _character.MovementSettings.MaxHorizontalSpeed;
			_animator.SetFloat(CharacterAnimatorParamId.HorizontalSpeed, normHorizontalSpeed);

			float jumpSpeed = _character.MovementSettings.JumpSpeed;
			float normVerticalSpeed = _character.VerticalVelocity.y.Remap(-jumpSpeed, jumpSpeed, -1.0f, 1.0f);
			_animator.SetFloat(CharacterAnimatorParamId.VerticalSpeed, normVerticalSpeed);

			_animator.SetBool(CharacterAnimatorParamId.IsGrounded, _character.IsGrounded);
		}
	}
}
