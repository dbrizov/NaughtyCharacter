using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Character))]
public class CharacterAnimatorController : MonoBehaviour
{
    private Animator animator;
    private Character character;

    protected virtual void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.character = this.GetComponent<Character>();
    }

    protected virtual void Update()
    {
        this.animator.SetFloat(AnimatorPropertyHash.Speed, this.character.HorizontalVelocity.magnitude);
        this.animator.SetBool(AnimatorPropertyHash.IsJumping, this.character.IsJumping);
    }
}
