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

        if (PlayerInput.Instance.ToggleWalkInput())
        {
            this.character.ToggleWalk();
        }

        this.character.IsSprinting = PlayerInput.Instance.SprintInput();

        if (PlayerInput.Instance.JumpInput())
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
