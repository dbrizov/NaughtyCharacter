using UnityEngine;
using System.Collections;

/// <summary>
/// The character is on the ground
/// </summary>
public class GroundedCharacterState : CharacterStateBase
{
    public GroundedCharacterState(Character character)
        : base(character)
    {
    }

    public override void Update()
    {
        base.Update();
        this.character.ApplyGravity(true); // Apply extra gravity

        if (PlayerInput.GetToggleWalkInput())
        {
            this.character.ToggleWalk();
        }

        this.character.IsSprinting = PlayerInput.GetSprintInput();

        if (PlayerInput.GetJumpInput())
        {
            this.character.Jump();
            this.ToJumpingState();
        }
        else if (!this.character.IsGrounded)
        {
            this.ToInAirState();
        }
    }
}
