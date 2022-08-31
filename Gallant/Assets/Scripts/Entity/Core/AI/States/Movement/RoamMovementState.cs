using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI.States.Movement
{
    public class RoamMovementState : BasicMovementState
    {
        public float WalkRange;
        public float WaitTimeMin = 1.0f;
        public float WaitTimeMax = 1.5f;

        private float m_timer = 0f;
        private Vector3 _currentTargetLocation;

        public override void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            base.OnStateEnter(animator, stateInfo, layerIndex);
            _currentTargetLocation = GetNewRoamPoint();
        }

        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            Vector3 _targetLoc = CalculateTargetLocation(animator);

            float dist = Vector3.Distance(_currentTargetLocation, Owner.transform.position);

            if (dist < 0.05f)
            {
                m_timer -= Time.deltaTime;
            }

            Owner.Movement.SetTargetLocation(_targetLoc, FaceTarget);

        }
        protected override Vector3 CalculateTargetLocation(Animator _animator)
        {
            float dist = Vector3.Distance(_currentTargetLocation, Owner.transform.position);
            if (dist < 0.05f && m_timer <= 0f)
            {
                _currentTargetLocation = GetNewRoamPoint();
                m_timer = Random.Range(WaitTimeMin, WaitTimeMax);
            }

            return _currentTargetLocation;
        }

        protected Vector3 GetNewRoamPoint()
        {
            Vector3 randomDirection = Random.insideUnitSphere * Mathf.Abs(WalkRange);
            randomDirection += Owner.transform.position;

            NavMeshHit hit;
            NavMesh.SamplePosition(randomDirection, out hit, WalkRange, 1);
            return hit.position;
        }
    }
}
