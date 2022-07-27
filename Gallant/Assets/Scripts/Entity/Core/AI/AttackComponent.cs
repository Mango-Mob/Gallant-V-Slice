using ActorSystem.Data;
using System;
using System.Collections.Generic;
using UnityEngine;

namespace EntitySystem.Core.AI
{
    public class AttackComponent : CoreComponent
    {
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

        public int GetNextAttack()
        {
            int next = -1;

            if (Owner.Target == null)
                return -1;

            for (int i = 0; i < AtkCDs.Count; i++)
            {
                if(AtkCDs[i] <= 0)
                {
                    //Attack is ready
                    if(AtkData[i].HasDetectedCollider(Owner.transform, Owner.TargetMask))
                    {
                        //Attack in range;
                        if (next >= 0)
                        {
                            next = (AtkData[i].priority < AtkData[next].priority) ? i : next;
                        }
                        else
                        {
                            next = i;
                        }
                    }
                }
            }

            return next;
        }
    }
}
