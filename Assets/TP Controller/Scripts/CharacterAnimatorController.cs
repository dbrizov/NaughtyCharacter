using UnityEngine;

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
        this.animator.SetFloat(AnimatorPropertyHash.HorizontalSpeed, this.character.HorizontalSpeed);
        this.animator.SetFloat(AnimatorPropertyHash.VerticalSpeed, this.character.VerticalSpeed);
        this.animator.SetBool(AnimatorPropertyHash.IsGrounded, this.character.IsGrounded);
    }
}
