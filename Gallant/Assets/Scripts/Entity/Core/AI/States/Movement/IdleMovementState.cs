using UnityEngine;

namespace EntitySystem.Core.AI.States.Movement
{
    //Sets the target rotation towards the target Entity and halts all movement of this entity.
    public class IdleMovementState : BasicMovementState
    {
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Owner.Movement.Halt();

            if(Owner.Target && FaceTarget)
            {
                Owner.Movement.SetTargetRotationTowards(Owner.Target.transform.position);
            }
        }

        protected override Vector3 CalculateTargetLocation(Animator _animator)
        {
            return Owner.transform.position;
        }
    }
}
