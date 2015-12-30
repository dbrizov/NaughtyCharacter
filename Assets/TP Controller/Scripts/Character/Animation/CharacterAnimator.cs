using UnityEngine;

public class CharacterAnimator : MonoBehaviour
{
    public static readonly int HORIZONTAL_SPEED = Animator.StringToHash("HorizontalSpeed");
    public static readonly int VERTICAL_SPEED = Animator.StringToHash("VerticalSpeed");
    public static readonly int IS_GROUNDED = Animator.StringToHash("IsGrounded");
    public static readonly int IDLE = Animator.StringToHash("Idle");
    public static readonly int IDLE_THINKING = Animator.StringToHash("IdleThinking");
    public static readonly int IDLE_REJECTED = Animator.StringToHash("IdleRejected");

    private Animator animator;
    private Character character;

    protected virtual void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.character = this.GetComponent<Character>();
    }

    protected virtual void Update()
    {
        this.animator.SetFloat(HORIZONTAL_SPEED, this.character.HorizontalSpeed);
        this.animator.SetFloat(VERTICAL_SPEED, this.character.VerticalSpeed);
        this.animator.SetBool(IS_GROUNDED, this.character.IsGrounded);
    }
}
