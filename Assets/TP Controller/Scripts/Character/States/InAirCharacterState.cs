using UnityEngine;
using System.Collections;

/// <summary>
/// The character is in the air, but he didn't jump to achieve that
/// </summary>
public class InAirCharacterState : CharacterStateBase
{
    public InAirCharacterState(Character character)
        : base(character)
    {
    }

    public override void OnEnter()
    {
        base.OnEnter();

        this.character.ResetVerticalSpeed();
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
