﻿using UnityEngine;
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
        public Vector3 ScaledVelocity { get { return LocalVelocity / Navigator.speed; } }

        private bool m_isKnocked;
        private bool m_canRotate;
        
        
        public MoveComponent(AIEntity _owner) : base(_owner)
        {
            Navigator = _owner.GetComponent<NavMeshAgent>();
            Body = _owner.GetComponent<Rigidbody>();

            m_isKnocked = false;
            m_canRotate = true;
            RotateDirection = 0;
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

        public void Halt()
        {
            if (Navigator.enabled && Navigator.isOnNavMesh)
                Navigator.isStopped = true;
        }
    }
}
