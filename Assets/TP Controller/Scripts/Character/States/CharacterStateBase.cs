using UnityEngine;

public abstract class CharacterStateBase : ICharacterState
{
    public static readonly ICharacterState GROUNDED_STATE = new GroundedCharacterState();
    public static readonly ICharacterState JUMPING_STATE = new JumpingCharacterState();
    public static readonly ICharacterState IN_AIR_STATE = new InAirCharacterState();

    public virtual void OnEnter(Character character) { }

    public virtual void OnExit(Character character) { }

    public virtual void Update(Character character)
    {
        character.ApplyGravity();
        character.MoveVector = PlayerInput.GetMovementInput(character.Camera);
        character.ControlRotation = PlayerInput.GetMouseRotationInput();
    }

    public virtual void ToState(Character character, ICharacterState state)
    {
        character.CurrentState.OnExit(character);
        character.CurrentState = state;
        character.CurrentState.OnEnter(character);
    }
}
