using UnityEngine;
using System.Collections;

public static class AnimatorPropertyHash
{
    public static readonly int HorizontalSpeed = Animator.StringToHash("HorizontalSpeed");
    public static readonly int VerticalSpeed = Animator.StringToHash("VerticalSpeed");
    public static readonly int IsGrounded = Animator.StringToHash("IsGrounded");
    public static readonly int Idle = Animator.StringToHash("Idle");
    public static readonly int IdleThinking = Animator.StringToHash("IdleThinking");
    public static readonly int IdleRejected = Animator.StringToHash("IdleRejected");
}
