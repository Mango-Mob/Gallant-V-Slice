using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

namespace ActorSystem.AI.Traps
{
    public class WallTrap : MonoBehaviour
    {
        public float m_damage = 10f;
        public float m_knockback = 5f;

        public LayerMask m_detectLayers;
        public LayerMask m_damageLayers;
        public bool hasDetected = false;
        public bool hasHitPlayer = false;

        private List<Actor> m_hitActors = new List<Actor>();

        private void OnTriggerStay(Collider other)
        {
            if(!hasDetected && m_detectLayers == (m_detectLayers | (1 << other.gameObject.layer)))
            {
                hasDetected = true;
                GetComponent<Animator>().SetTrigger("Shoot");
            }
        }
        public bool LogPlayer(Player_Controller player)
        {
            if(!hasHitPlayer)
            {
                hasHitPlayer = true;
                player.DamagePlayer(m_damage, CombatSystem.DamageType.Physical, null, true);
                player.StunPlayer(0.3f, (player.transform.position - transform.position).normalized * m_knockback, null);
                return true;
            }
            return false;
        }

        public bool LogActor(Actor actor)
        {
            if (!m_hitActors.Contains(actor))
            {
                m_hitActors.Add(actor);
                actor.DealDamage(m_damage, CombatSystem.DamageType.Physical, 0, transform.position);
                actor.DealImpactDamage(m_knockback * 10f, 0, (actor.transform.position - transform.position).normalized, CombatSystem.DamageType.Physical);
                return true;
            }
            return false;
        }

        public void ResetTrap()
        {
            hasHitPlayer = false;
            hasDetected = false;
            m_hitActors.Clear();
            GetComponent<Animator>().SetTrigger("Reset");
        }
    }
}
