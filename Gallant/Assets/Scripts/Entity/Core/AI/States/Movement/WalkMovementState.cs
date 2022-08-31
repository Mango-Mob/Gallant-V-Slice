using UnityEngine;

namespace EntitySystem.Core.AI.States.Movement
{
    public class WalkMovementState : BasicMovementState
    {
        public float IdealDistance;
        protected override Vector3 CalculateTargetLocation(Animator _animator)
        {
            float dist = _animator.GetFloat("DistToTarget");
            Vector3 forward = (Owner.Target.transform.position - Owner.transform.position).normalized;

            return Owner.transform.position + forward * (dist - Mathf.Abs(IdealDistance));
        }
    }
}
