using System;

public interface ICharacterState
{
    void OnEnter();

    void OnExit();

    void Update();

    void ToGroundedState();

    void ToJumpingState();

    void ToInAirState();
}
