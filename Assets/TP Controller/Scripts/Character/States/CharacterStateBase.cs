using UnityEngine;
using System.Collections;
using System;

public abstract class CharacterStateBase : ICharacterState
{
    protected Character character;

    public CharacterStateBase(Character character)
    {
        this.character = character;
    }

    public virtual void ToGroundedState()
    {
        this.character.CurrentState = this.character.GroundedState;
    }

    public virtual void ToJumpState()
    {
        this.character.CurrentState = this.character.JumpState;
    }

    public virtual void UpdateState()
    {
        this.character.MoveVector = PlayerInput.Instance.MovementInput();
        this.character.ControlRotation = PlayerInput.Instance.MouseRotationInput();
    }
}
