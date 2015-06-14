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
        this.animator.SetFloat(AnimatorPropertyHash.HorizontalSpeed, this.character.HorizontalVelocity.magnitude);
        this.animator.SetFloat(AnimatorPropertyHash.VerticalSpeed, this.character.VerticalVelocity.magnitude);
        //this.animator.SetBool(AnimatorPropertyHash.IsGrounded, this.character.IsGrounded);
        //Debug.Log(this.character.IsGrounded);
    }
}
