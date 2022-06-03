using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AnimSync : StateMachineBehaviour
{
    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        animator.Play(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("IdleArmL")).shortNameHash, animator.GetLayerIndex("IdleArmL"));
        animator.Play(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("IdleArmR")).shortNameHash, animator.GetLayerIndex("IdleArmR"));
        animator.Play(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("RunArmL")).shortNameHash, animator.GetLayerIndex("RunArmL"));
        animator.Play(animator.GetCurrentAnimatorStateInfo(animator.GetLayerIndex("RunArmR")).shortNameHash, animator.GetLayerIndex("RunArmR"));
    }
}
