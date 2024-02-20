using ActorSystem.Data;
using BTSystem.Core;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    public class AIEntity : Entity
    {
        [Header("AI Entity")]
        public BTGraph Behaviour;
        public Entity Target;
        public LayerMask TargetMask;
        public List<AttackData> MyAttackData;
        public GameObject AttackSource;

        public BrainComponent Brain { get; private set; } = null;
        public MoveComponent Movement { get; private set; } = null;
        public AttackComponent Attack { get; private set; } = null;
        
        protected override void Awake()
        {
            base.Awake();

            if (BrainComponent.CanOwnerHaveComponent(this) && Behaviour)
            {
                Behaviour = Behaviour.Copy() as BTGraph;
                Brain = new BrainComponent(this, Behaviour);
            }

            if( MoveComponent.CanOwnerHaveComponent(this) )
                Movement = new MoveComponent(this);

            if(AttackComponent.CanOwnerHaveComponent(this))
                Attack = new AttackComponent(this, MyAttackData);
        }

        protected virtual void Update()
        {
            Brain?.Update(Time.deltaTime);
            Movement?.Update(Time.deltaTime);
            Attack?.Update(Time.deltaTime);
        }
		
        protected virtual void FixedUpdate()
        {
            Movement?.FixedUpdate(Time.fixedDeltaTime);
        }

        public override bool DealDamageToEntity(DamageInstance _damage, bool _playHurtSound = false)
        {
            throw new System.NotImplementedException();
        }

        public void OnDrawGizmosSelected()
        {
            if(Application.isPlaying)
            {
                Gizmos.color = Color.yellow;
                Gizmos.DrawSphere(Movement.TargetPosition, 0.5f);

                Gizmos.color = Color.red;
                Gizmos.DrawLine(transform.position, transform.position + Movement.TargetRotation * Vector3.forward);
                Gizmos.color = Color.green;
                Gizmos.DrawLine(transform.position, transform.position + transform.rotation * Vector3.forward);
                
            }
        }

        public void InvokeAttack() 
        {
            if(Attack.CurrPerformance != null)
            {
                Attack.CurrPerformance.InvokeAttack(transform, ref AttackSource, TargetMask, 0);
            }
        }

        public void EndAttack()
        {
            Attack.CurrPerformance = null; 
        }
    }
}
