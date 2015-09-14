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

    public virtual void OnEnter()
    {
    }

    public virtual void OnExit()
    {
    }

    public virtual void ToGroundedState()
    {
        this.character.CurrentState.OnExit();
        this.character.CurrentState = this.character.GroundedState;
        this.character.CurrentState.OnEnter();
    }

    public void ToInAirState()
    {
        this.character.CurrentState.OnExit();
        this.character.CurrentState = this.character.InAirState;
        this.character.CurrentState.OnEnter();
    }

    public virtual void ToJumpingState()
    {
        this.character.CurrentState.OnExit();
        this.character.CurrentState = this.character.JumpingState;
        this.character.CurrentState.OnEnter();
    }

    public virtual void Update()
    {
        this.character.ApplyGravity();
        this.character.MoveVector = PlayerInput.Instance.MovementInput();
        this.character.ControlRotation = PlayerInput.Instance.MouseRotationInput();
    }
}
