using ActorSystem.Data;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class AIEntity : Entity
    {
        [Header("AI Entity")]
        public Entity Target;
        public LayerMask TargetMask;
        public List<AttackData> MyAttackData;

        public BrainComponent Brain { get; private set; } = null;
        public MoveComponent Movement { get; private set; } = null;

        public AttackComponent Attack { get; private set; } = null;

        public Animator EntityAnimation;
        
        protected override void Awake()
        {
            base.Awake();

            Brain = new BrainComponent(this);
            Movement = new MoveComponent(this);
            Attack = new AttackComponent(this, MyAttackData);

        }

        protected virtual void Update()
        {
            Brain.Update(Time.deltaTime);
            Movement.Update(Time.deltaTime);
            Attack.Update(Time.deltaTime);

            EntityAnimation?.SetFloat("VelocityHorizontal", Movement.ScaledVelocity.x);
            EntityAnimation?.SetFloat("VelocityVertical", Movement.ScaledVelocity.z);
            EntityAnimation?.SetFloat("RotationVelocity", Movement.RotateDirection);
            EntityAnimation?.SetFloat("VelocityHaste", 1f);
        }
		
        protected virtual void FixedUpdate()
        {
            Movement.FixedUpdate(Time.fixedDeltaTime);
        }

        public override bool DealDamageToEntity(DamageInstance _damage, bool _playHurtSound = false)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrawGizmos()
        {
            if(Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(Movement.TargetPosition, 0.5f);
            }
        }
    }
}
