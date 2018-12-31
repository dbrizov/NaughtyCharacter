using UnityEngine;
using System.Collections.Generic;

public class IdleStateBehaviour : StateMachineBehaviour
{
    private int nextIdleState;
    private List<int> idleStates;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (this.idleStates == null)
        {
            this.idleStates = new List<int>();
            idleStates.Add(CharacterAnimator.IDLE_THINKING);
            idleStates.Add(CharacterAnimator.IDLE_REJECTED);
        }

        this.nextIdleState = this.idleStates.GetRandomElement();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (stateInfo.normalizedTime > 1f)
        {
            animator.SetTrigger(this.nextIdleState);
        }
    }

    // OnStateExit is called when a transition ends and the state machine finishes evaluating this state
    override public void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.ResetTrigger(this.nextIdleState);
    }

    // OnStateMove is called right after Animator.OnAnimatorMove(). Code that processes and affects root motion should be implemented here
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK(). Code that sets up animation IK (inverse kinematics) should be implemented here.
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) {
    //
    //}
}
