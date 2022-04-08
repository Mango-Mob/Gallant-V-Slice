using ActorSystem.AI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.Data
{
    [CreateAssetMenu(fileName = "UtilityAttack_Data", menuName = "Game Data/Attack Data/Utility", order = 1)]
    public class UtilityAttackData : AttackData
    {
        public enum UtilityType { Teleport}
        public UtilityType m_type;

        public override bool InvokeAttack(Transform parent, GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
            return true;
        }

        public override void BeginActor(Actor user)
        {
            switch (m_type)
            {
                case UtilityType.Teleport:
                    break;
                default:
                    break;
            }
        }

        public override void UpdateActor(Actor user)
        {
            switch (m_type)
            {
                case UtilityType.Teleport:
                    break;
                default:
                    break;
            }
        }

        public override void EndActor(Actor user)
        {
            switch (m_type)
            {
                case UtilityType.Teleport:
                    base.EndActor(user);
                    break;
                default:
                    break;
            } 
        }
    }
}
