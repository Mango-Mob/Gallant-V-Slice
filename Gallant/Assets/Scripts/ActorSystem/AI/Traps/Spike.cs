using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class Spike : MonoBehaviour
    {
        private List<Collider> m_hitColliders = new List<Collider>();

        public void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
            {
                m_hitColliders.Add(other);
            }
        }

        public void OnTriggerExit(Collider other)
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Player") || other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
            {
                m_hitColliders.Remove(other);
            }
        }

        public void DamageColliders(float damage)
        {
            foreach (var other in m_hitColliders)
            {
                if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    other.GetComponent<Player_Controller>().DamagePlayer(damage, CombatSystem.DamageType.Physical, null, false);
                }
                else if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
                {
                    other.GetComponent<Actor>().DealDamage(damage, CombatSystem.DamageType.Physical, 0, null);
                }
            }
        }
    }
}
