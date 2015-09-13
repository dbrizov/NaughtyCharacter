using UnityEngine;
using System.Collections;

public class JumpCharacterState : CharacterStateBase
{
    public JumpCharacterState(Character character)
        : base(character)
    {
    }

    public override void UpdateState()
    {
        base.UpdateState();

        if (this.character.IsGrounded)
        {
            this.ToGroundedState();
        }
    }
}
