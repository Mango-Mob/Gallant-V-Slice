using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Combo : StateMachineBehaviour
{
    public bool m_comboStarter = true;
    public AnimatorTransitionInfo[] leftTransitions;
    public AnimatorTransitionInfo[] rightTransitions;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.SetBool("LeftShield", false);
        animator.SetBool("RightShield", false);
        animator.SetBool("LeftSword", false);
        animator.SetBool("RightSword", false);
        animator.SetBool("LeftBoomerang", false);
        animator.SetBool("RightBoomerang", false);
        animator.SetBool("LeftCast", false);
        animator.SetBool("RightCast", false);
    }

}