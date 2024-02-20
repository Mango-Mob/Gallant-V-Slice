
using BTSystem.Core;
using UnityEditor;
using UnityEngine;
using UnityEngine.AI;

namespace EntitySystem.Core.AI
{
    public class BrainComponent : CoreComponent
    {
        protected BTGraph Behaviour;

        public BrainComponent(AIEntity _owner, BTGraph _behaviour) : base(_owner)
        {
            Debug.Assert(_behaviour != null);
            Behaviour = _behaviour;
        }
        static public bool CanOwnerHaveComponent(AIEntity _owner) { return true; }
        public override void Update(float deltaTime)
        {
            UpdateParam();
            Behaviour?.Execute();
        }

        private void UpdateParam()
        {
            Behaviour.Owner = Owner;
            Behaviour.IsDead = Owner.IsDead;
            Behaviour.Target = Owner.Target;
            Behaviour.HealthPercent = Owner.HP / Owner.DataOnLoad.HP;
            Behaviour.StaminaPercent = Owner.Stamina / Owner.DataOnLoad.Stamina;
            Behaviour.IsAttacking = Owner.Attack.CurrPerformance != null;
        }
    }
}
