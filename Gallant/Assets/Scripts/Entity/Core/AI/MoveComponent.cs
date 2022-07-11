using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI
{
    public class MoveComponent : CoreComponent
    {
        protected NavMeshAgent Navigator;
        protected Rigidbody Body;
        public Vector3 TargetPosition { get; private set; }
        public Quaternion TargetRotation { get; private set; }

        private bool m_canRotate;
        private int m_rotateDirection;
        
        public MoveComponent(AIEntity _owner) : base(_owner)
        {
            Navigator = _owner.GetComponent<NavMeshAgent>();
            Body = _owner.GetComponent<Rigidbody>();

            m_canRotate = true;
            m_rotateDirection = 0;
            TargetPosition = Owner.transform.position;
            TargetRotation = Owner.transform.rotation;
        }

        public override void Update(float deltaTime)
        {

        }

        public void FixedUpdate(float _fixedDeltaTime)
        {
            if (Navigator.enabled && Navigator.isOnNavMesh && Navigator.updatePosition)
            {
                if (!Navigator.isStopped)
                {
                    Navigator.destination = TargetPosition;
                    Navigator.speed = Owner.Speed;
                }

                if (m_canRotate && Quaternion.Angle(Owner.transform.rotation, TargetRotation) > 1f)
                {
                    Owner.transform.rotation = Quaternion.RotateTowards(Owner.transform.rotation, TargetRotation, Owner.Speed * Time.fixedDeltaTime);

                    if (Quaternion.Angle(Owner.transform.rotation, TargetRotation) > 5f)
                    {
                        if (Vector3.Dot(Owner.transform.right, TargetRotation * Vector3.forward) > 0)
                        {
                            m_rotateDirection = 1;
                        }
                        else
                        {
                            m_rotateDirection = -1;
                        }
                    }
                    else
                    {
                        m_rotateDirection = 0;
                    }
                }
                else
                {
                    m_rotateDirection = 0;
                }
            }

            if (!Navigator.updatePosition)
                Navigator.Warp(Owner.transform.position);

        }

        public virtual void SetTargetLocation(Vector3 target, bool lookAtTarget = false)
        {
            if (!Navigator.enabled || !Navigator.isOnNavMesh)
                return;

            Navigator.isStopped = false;
            TargetPosition = target;

            Vector3 direction = TargetPosition - Owner.transform.position;
            direction.y = 0;

            if (lookAtTarget)
                SetTargetRotation(Quaternion.LookRotation(direction.normalized, Vector3.up));
        }

        public void SetTargetRotation(Quaternion rotation)
        {
            TargetRotation = rotation;
        }

        public void Halt()
        {
            if (Navigator.enabled && Navigator.isOnNavMesh)
                Navigator.isStopped = true;
        }
    }
}
