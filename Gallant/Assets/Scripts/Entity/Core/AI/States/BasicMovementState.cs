using UnityEngine;

namespace EntitySystem.Core.AI.States
{
    public abstract class BasicMovementState : BasicAnimatorState
    {
        public bool FaceTarget = true;

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex) 
        { 
            if(Owner.Target)
            {
                Vector3 _targetLoc = CalculateTargetLocation(animator);

                if(_targetLoc == Owner.transform.position)
                {
                    Owner.Movement.Halt();
                }
                else
                {
                    Owner.Movement.SetTargetLocation(_targetLoc, FaceTarget);
                }
            }
        }

        /// <summary>
        /// Calculate the ideal target location of this enitiy.
        /// </summary>
        /// <param name="_animator">This animator</param>
        /// <returns>Target Location</returns>
        protected abstract Vector3 CalculateTargetLocation(Animator _animator);

        //private Vector3 CalculateTargetLocation(Animator animator)
        //{
        //    float dist = animator.GetFloat("DistToTarget");
        //    Vector3 forward = (Owner.Target.transform.position - Owner.transform.position).normalized;
        //    switch (MoveType)
        //    {
        //        default:
        //        case MovementType.Stop:
        //            return Owner.transform.position;
        //        case MovementType.MoveTo:
        //            return Owner.transform.position + forward * (dist - IdealDistance);
        //        case MovementType.Strafe:
        //            return Owner.transform.position;
        //        case MovementType.Roam:
        //            return Owner.transform.position;
        //    }
        //}
    }
}
