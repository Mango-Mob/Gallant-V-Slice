using ActorSystem.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntitySystem.Core.AI
{
    public class AttackComponent : CoreComponent
    {
        public AttackData CurrPerformance = null;
        protected List<float> AtkCDs;
        protected List<AttackData> AtkData;
        public AttackComponent(AIEntity _owner, List<AttackData> _atks) : base(_owner)
        {
            AtkCDs = new List<float>();
            AtkData = _atks;
        }

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
            Owner.EntityAnimation?.Play(_attackData.animID);
        }
    }
}
