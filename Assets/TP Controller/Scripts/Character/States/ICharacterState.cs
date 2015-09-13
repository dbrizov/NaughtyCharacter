using System;

public interface ICharacterState
{
    void UpdateState();

    void ToGroundedState();

    void ToJumpState();
}
