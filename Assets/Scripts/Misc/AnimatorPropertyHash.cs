using UnityEngine;
using System.Collections;

public static class AnimatorPropertyHash
{
    public static readonly int Speed = Animator.StringToHash("Speed");
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int IdleThinking = Animator.StringToHash("IdleThinking");
    public static readonly int IdleRejected = Animator.StringToHash("IdleRejected");
}
