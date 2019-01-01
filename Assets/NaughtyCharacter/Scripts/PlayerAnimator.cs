using UnityEngine;

namespace NaughtyCharacter
{
    public enum AnimatorState
    {
        Idle = 0,
        Running = 1,
        Airborne = 2
    }

    public class PlayerAnimator : MonoBehaviour
    {
        private static readonly int StateHash = Animator.StringToHash("State");

        private Animator _animator;
        private PlayerController _playerController;

        private AnimatorState _state;

        private void Awake()
        {
            _animator = GetComponent<Animator>();
            _playerController = GetComponent<PlayerController>();
        }

        // OnAnimatorMove will be called after FixedUpdate of the character if the animator's update mode is UpdateMode.AnimatePhysics
        private void OnAnimatorMove()
        {
            if (_playerController.State == ControllerState.Idle)
            {
                SetState(AnimatorState.Idle);
            }
            else if (_playerController.State == ControllerState.Running)
            {
                SetState(AnimatorState.Running);
            }
            else if (_playerController.State == ControllerState.Airborne)
            {
                SetState(AnimatorState.Airborne);
            }
        }

        private void SetState(AnimatorState state)
        {
            _state = state;
            _animator.SetInteger(StateHash, (int)state);
        }
    }
}
