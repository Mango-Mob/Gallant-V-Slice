using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace EntitySystem.Core.AI.States
{
    public abstract class BasicAnimatorState : StateMachineBehaviour
    {
        public AIEntity Owner { get; private set; }

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        {
            Owner = animator.GetComponent<AIEntity>();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) { }
    }
}
