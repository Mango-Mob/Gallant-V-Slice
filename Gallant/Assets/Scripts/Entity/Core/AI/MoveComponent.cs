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

        public int RotateDirection { get; private set; }

        public Vector3 Velocity { get { return (m_isKnocked) ? Vector3.zero : Navigator.velocity; } }
        public Vector3 LocalVelocity { get { return (m_isKnocked) ? Vector3.zero : Quaternion.AngleAxis(Owner.transform.rotation.eulerAngles.y, -Vector3.up) * Navigator.velocity; } }
        
        public Vector3 ScaledVelocity {
            get {
                if (Navigator.speed == 0 || Navigator.isStopped)
                    return Vector3.zero;
                else
                    return LocalVelocity / Navigator.speed;
            } 
        }

        public float RemainingDist { 
            get {
                if (Navigator.isStopped)
                    return 0;
                else
                    return Navigator.remainingDistance;
            } 
        }

        private bool m_isKnocked;
        private bool m_canRotate;

        private Animator EntityAnimation;

        public MoveComponent(AIEntity _owner) : base(_owner)
        {
            EntityAnimation = _owner.GetComponentInChildren<Animator>();
            Navigator = _owner.GetComponent<NavMeshAgent>();
            Body = _owner.GetComponent<Rigidbody>();

            Debug.Assert(Navigator != null && Body != null);

            m_isKnocked = false;
            m_canRotate = true;
            RotateDirection = 0;
            TargetPosition = Owner.transform.position;
            TargetRotation = Owner.transform.rotation;
        }

        static public bool CanOwnerHaveComponent(AIEntity _owner) { return _owner.GetComponent<NavMeshAgent>() && _owner.GetComponentInChildren<Animator>() && _owner.GetComponent<Rigidbody>(); }

        public override void Update(float deltaTime)
        {
            EntityAnimation?.SetFloat("VelocityHorizontal", ScaledVelocity.x);
            EntityAnimation?.SetFloat("VelocityVertical", ScaledVelocity.z);
            EntityAnimation?.SetFloat("RotationVelocity", RotateDirection);
            EntityAnimation?.SetFloat("VelocityHaste", 1f);
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

                if (m_canRotate && Quaternion.Angle(Owner.transform.rotation, TargetRotation) > 5f)
                {
                    Owner.transform.rotation = Quaternion.RotateTowards(Owner.transform.rotation, TargetRotation, Owner.Speed * 40 * Time.fixedDeltaTime);

                    if (Quaternion.Angle(Owner.transform.rotation, TargetRotation) > 5f)
                    {
                        if (Vector3.Dot(Owner.transform.right, TargetRotation * Vector3.forward) > 0)
                        {
                            RotateDirection = 1;
                        }
                        else
                        {
                            RotateDirection = -1;
                        }
                    }
                    else
                    {
                        RotateDirection = 0;
                    }
                }
                else
                {
                    RotateDirection = 0;
                }
            }

            if (!Navigator.updatePosition)
                Navigator.Warp(Owner.transform.position);

        }

        public virtual void SetTargetLocation(Vector3 target, bool lookAtTarget = false)
        {
            if (!Navigator.enabled || !Navigator.isOnNavMesh)
                return;

            if(target == Owner.transform.position)
            {
                Halt();
                return;
            }    

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

        public void SetTargetRotationTowards(Vector3 _targetLook)
        {
            Vector3 direction = _targetLook - Owner.transform.position;
            direction.y = 0;
            TargetRotation = Quaternion.LookRotation(direction.normalized, Vector3.up);
        }

        public void Halt()
        {
            if (Navigator.enabled && Navigator.isOnNavMesh)
                Navigator.isStopped = true;
        }
    }
}
