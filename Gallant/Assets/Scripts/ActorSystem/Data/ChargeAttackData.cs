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

        private bool _startCharge = false;
        public override bool InvokeAttack(Transform parent, ref GameObject source, int filter, uint id = 0, float damageMod = 1)
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
                    item.GetComponentInParent<Destructible>().ExplodeObject(damageColliders[0].GetHitlocation(parent), baseDamage * damageMod, 2f, false);
                    if (i <= 0) return true;
                }
            }
            return canContinueAfterMiss;
        }

        public override void BeginActor(Actor user)
        {
            base.BeginActor(user);
            _startCharge = false;
            user.m_myBrain.m_legs.Halt();
        }

        public override void StartActor(Actor user)
        {
            base.StartActor(user);
            _startCharge = true;
            if (user.m_myBrain.m_legs != null)
            {
                user.m_myBrain.m_legs.m_agent.velocity = user.m_myBrain.transform.forward * user.m_myBrain.m_legs.m_agent.velocity.magnitude;
                user.m_myBrain.m_legs.OverrideStopDist(0);
                user.m_myBrain.m_legs.m_canBeKnocked = false;
                user.m_myBrain.m_legs.m_baseSpeed = speedOverride;
            }
        }

        public override void UpdateActor(Actor user)
        {
            if (_startCharge)
                user.SetTargetLocation(user.transform.position + user.transform.forward, true);
            else
                user.SetTargetOrientaion(user.m_target.transform.position);
        }

        public override void EndActor(Actor user)
        {
            base.EndActor(user);
            if(user.m_myBrain.m_legs != null)
            {
                user.m_myBrain.m_legs.RefreshStopDist();
                user.m_myBrain.m_legs.m_canBeKnocked = true;
                user.m_myBrain.m_legs.m_baseSpeed = user.m_myData.baseSpeed;
            }
        }
    }
}
