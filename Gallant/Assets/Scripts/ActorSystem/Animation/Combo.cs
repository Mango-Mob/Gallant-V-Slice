using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Combo : StateMachineBehaviour
{
    public bool m_comboStarter = true;
    public AnimatorTransitionInfo[] leftTransitions;
    public AnimatorTransitionInfo[] rightTransitions;
    private Player_Controller playerController;


    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        //foreach (string name in System.Enum.GetNames(typeof(Weapon)))
        //{
        //    if (name == "BRICK")
        //        continue;

            //    animator.SetBool("Right" + name[0] + name.Substring(1).ToLower(), false);
            //    animator.SetBool("Left" + name[0] + name.Substring(1).ToLower(), false);
            //}

        animator.SetBool("UsingLeft", false);
        animator.SetBool("UsingRight", false);

        animator.SetBool("LeftCast", false);
        animator.SetBool("LeftSwingCast", false);
        animator.SetBool("RightCast", false);
        animator.SetBool("RightSwingCast", false);

        animator.SetBool("IsHealing", false);
        animator.SetInteger("ComboCount", 0);

        animator.SetBool("CanRotate", true);
        //animator.SetBool("LeftShield", false);
        //animator.SetBool("RightShield", false);

        //animator.SetBool("LeftSword", false);
        //animator.SetBool("RightSword", false);

        //animator.SetBool("LeftBoomerang", false);
        //animator.SetBool("RightBoomerang", false);

        //animator.SetBool("LeftCrossbow", false);
        //animator.SetBool("RightCrossbow", false);

        //animator.SetBool("LeftSpear", false);
        //animator.SetBool("RightSpear", false);

    }
}