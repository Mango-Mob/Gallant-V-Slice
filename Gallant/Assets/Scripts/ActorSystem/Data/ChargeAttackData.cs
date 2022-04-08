using ActorSystem.AI;
using UnityEngine;

namespace ActorSystem.Data
{
    [CreateAssetMenu(fileName = "ChargeAttack_Data", menuName = "Game Data/Attack Data/Charge", order = 1)]
    public class ChargeAttackData : AttackData
    {
        public int hitsCount = 1;
        public bool ignoreIFrames;
        public float speedOverride = 5.0f;

        public override bool InvokeAttack(Transform parent, GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
            int i = hitsCount;
            Collider[] targets = GetDamageOverlap(parent, filter, id);
            foreach (var item in targets)
            {
                if (item.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    i--;
                    Player_Controller player = item.GetComponentInParent<Player_Controller>();
                    player.DamagePlayer(baseDamage * damageMod, damageType, source, ignoreIFrames);
                    ApplyEffect(player, parent, onHitEffect, effectPower);
                    if (i <= 0) return true;
                }
                else if (item.gameObject.layer == LayerMask.NameToLayer("Destructible"))
                {
                    i--;
                    if (i <= 0) return true;
                }
            }
            return canContinueAfterMiss;
        }

        public override void BeginActor(Actor user)
        {
            base.BeginActor(user);
            if (user.m_myBrain.m_legs != null)
            {
                user.m_myBrain.m_legs.OverrideStopDist(0);
                user.m_myBrain.m_legs.m_canBeKnocked = false;
                user.m_myBrain.m_legs.m_agent.speed = speedOverride;
            } 
        }

        public override void UpdateActor(Actor user)
        {
            user.SetTargetLocation(user.transform.position + user.transform.forward, true);
        }

        public override void EndActor(Actor user)
        {
            base.EndActor(user);
            if(user.m_myBrain.m_legs != null)
            {
                user.m_myBrain.m_legs.RefreshStopDist();
                user.m_myBrain.m_legs.m_canBeKnocked = true;
                user.m_myBrain.m_legs.m_agent.speed = user.m_myBrain.m_legs.m_baseSpeed;
            }
        }
    }
}
