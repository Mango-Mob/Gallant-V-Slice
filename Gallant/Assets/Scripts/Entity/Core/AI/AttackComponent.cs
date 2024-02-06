using ActorSystem.Data;
using AmplifyShaderEditor;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using static UnityEngine.UIElements.UxmlAttributeDescription;

namespace EntitySystem.Core.AI
{
    public class AttackComponent : CoreComponent
    {
        public AttackData CurrPerformance = null;
        protected List<float> AtkCDs;
        protected List<AttackData> AtkData;

        private Animator EntityAnimation;
        public AttackComponent(AIEntity _owner, List<AttackData> _atks) : base(_owner)
        {
            EntityAnimation = _owner.GetComponentInChildren<Animator>();
            AtkCDs = new List<float>();
            AtkData = _atks;
        }
        static public bool CanOwnerHaveComponent(AIEntity _owner) { return _owner.GetComponentInChildren<Animator>(); }

        public override void Update(float deltaTime)
        {
            for (int i = 0; i < AtkCDs.Count; i++)
            {
                AtkCDs[i] = Mathf.Max(AtkCDs[i] - deltaTime, 0);
            }
        }

        public void Perform(AttackData _attackData)
        {
            CurrPerformance = _attackData;
            EntityAnimation?.Play(_attackData.animID);
        }

        public AttackData GetFirstAvailable()
        {
            for (int i = 0; i < AtkCDs.Count; i++)
            {
                if (AtkCDs[i] > 0)
                    continue;

                if (AtkData[i].HasDetectedCollider(Owner.transform, Owner.TargetMask))
                    return AtkData[i];
            }
            return null;
        }
    }
}
