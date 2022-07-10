using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(Animator))]
    public class AIEntity : Entity
    {
        protected override void Awake()
        {
            base.Awake();
        }



        protected override bool DealDamageToEntity(float _damage, float _pen, DamageType _type, GameObject _source = null, bool _playHurtSound = false)
        {
            throw new System.NotImplementedException();
        }
    }
}
