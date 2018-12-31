using UnityEngine;
using System.Collections;

/// <summary>
/// The character is on the ground
/// </summary>
public class GroundedCharacterState : CharacterStateBase
{
    public override void Update(Character character)
    {
        base.Update(character);

        character.ApplyGravity(true); // Apply extra gravity

        if (PlayerInput.GetToggleWalkInput())
        {
            character.ToggleWalk();
        }

        character.IsSprinting = PlayerInput.GetSprintInput();

        if (PlayerInput.GetJumpInput())
        {
            character.Jump();
            this.ToState(character, CharacterStateBase.JUMPING_STATE);
        }
        else if (!character.IsGrounded)
        {
            this.ToState(character, CharacterStateBase.IN_AIR_STATE);
        }
    }
}
