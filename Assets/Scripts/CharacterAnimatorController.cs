using UnityEngine;
using System.Collections;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Character))]
public class CharacterAnimatorController : MonoBehaviour
{
    private readonly int SpeedHash = Animator.StringToHash("Speed");

    private Animator animator;
    private Character character;

    protected virtual void Awake()
    {
        this.animator = this.GetComponent<Animator>();
        this.character = this.GetComponent<Character>();
    }

    protected virtual void Update()
    {
        this.animator.SetFloat(SpeedHash, this.character.Speed);
    }
}
