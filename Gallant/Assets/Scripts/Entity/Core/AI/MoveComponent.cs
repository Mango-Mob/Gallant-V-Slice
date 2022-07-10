using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI
{
    public class MoveComponent : CoreComponent
    {
        protected NavMeshAgent Navigator;
        protected Rigidbody Body;

        public MoveComponent(Entity _owner) : base(_owner)
        {
            Navigator = _owner.GetComponent<NavMeshAgent>();
            Body = _owner.GetComponent<Rigidbody>();
        }

        public override void Update(float deltaTime)
        {

        }
    }
}
