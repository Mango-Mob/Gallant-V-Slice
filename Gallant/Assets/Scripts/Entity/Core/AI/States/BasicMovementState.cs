using UnityEngine;

namespace EntitySystem.Core.AI.States
{
    public class BasicMovementState : StateMachineBehaviour
    {
        public bool FaceTarget = true;
        public float IdealDistance = 2;
        public AIEntity Owner { get; private set; }

        public enum MovementType { Stop, MoveTo, Strafe, Roam}
        public MovementType MoveType;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Owner = animator.GetComponent<AIEntity>();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            if(Owner.Target)
                Owner.Movement.SetTargetLocation(CalculateTargetLocation(animator), FaceTarget);
        }

        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        private Vector3 CalculateTargetLocation(Animator animator)
        {
            float dist = animator.GetFloat("DistToTarget");
            Vector3 forward = (Owner.Target.transform.position - Owner.transform.position).normalized;
            switch (MoveType)
            {
                default:
                case MovementType.Stop:
                    return Owner.transform.position;
                case MovementType.MoveTo:
                    return Owner.transform.position + forward * (dist - IdealDistance);
                case MovementType.Strafe:
                    return Owner.transform.position;
                case MovementType.Roam:
                    return Owner.transform.position;
            }
        }
    }
}
