using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class AIEntity : Entity
    {
        public Entity Target { get; set; }

        protected BrainComponent Brain { get; private set; } = null;
        protected MoveComponent Movement { get; private set; } = null;

        protected override void Awake()
        {
            base.Awake();
            Brain = new BrainComponent(this);
            Movement = new MoveComponent(this);
        }

        protected virtual void Update()
        {
            Brain.Update(Time.deltaTime);
        }

        public override bool DealDamageToEntity(Damage _damage, bool _playHurtSound = false)
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
