using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI.States.Movement
{
    public class FleeMovementState : BasicMovementState
    {
        public float IdealDistanceFromTarget;
        [Tooltip("Enables sampling around the target location.")]
        public bool SmartFlee = false;
        
        protected override Vector3 CalculateTargetLocation(Animator _animator)
        {
            float dist = _animator.GetFloat("DistToTarget");
            Vector3 forward = (Owner.Target.transform.position - Owner.transform.position).normalized;

            if (dist > Mathf.Abs(IdealDistanceFromTarget))
            {
                //Too far from the target!
                return Owner.transform.position;
            }
            else //dist <= idealDistance
            {
                //Too close
                NavMeshHit hit;
                Vector3 _targetLoc = Owner.transform.position + forward * (dist - Mathf.Abs(IdealDistanceFromTarget));
                if (SmartFlee && NavMesh.SamplePosition(_targetLoc, out hit, 3f, ~0))
                {
                    return hit.position;
                }
                return _targetLoc;
            }
        }
    }
}
