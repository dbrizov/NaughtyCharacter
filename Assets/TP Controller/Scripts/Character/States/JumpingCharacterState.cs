using UnityEngine;
using System.Collections;

/// <summary>
/// The character is in the air, and he jumped to achieve that
/// </summary>
public class JumpingCharacterState : CharacterStateBase
{
    public JumpingCharacterState(Character character)
        : base(character)
    {
    }

    public override void Update()
    {
        base.Update();

        if (this.character.IsGrounded)
        {
            this.ToGroundedState();
        }
    }
}
