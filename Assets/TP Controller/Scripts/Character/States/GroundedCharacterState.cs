using UnityEngine;
using System.Collections;

public class GroundedCharacterState : CharacterStateBase
{
    public GroundedCharacterState(Character character)
        : base(character)
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (PlayerInput.Instance.ToggleWalkInput())
        {
            this.character.ToggleWalk();
        }

        this.character.IsSprinting = PlayerInput.Instance.SprintInput();

        if (PlayerInput.Instance.JumpInput())
        {
            this.character.Jump();
            this.ToJumpState();
        }
    }
}
