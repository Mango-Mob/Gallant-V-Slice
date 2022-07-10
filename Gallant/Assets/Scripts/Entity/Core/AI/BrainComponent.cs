
using UnityEngine;

namespace EntitySystem.Core.AI
{
    public class BrainComponent : CoreComponent
    {
        protected Animator LogicAnimator;

        public BrainComponent(Entity _owner) : base(_owner)
        {
            LogicAnimator = _owner.GetComponent<Animator>();
        }

        public override void Update(float deltaTime)
        {
            
        }
    }
}
