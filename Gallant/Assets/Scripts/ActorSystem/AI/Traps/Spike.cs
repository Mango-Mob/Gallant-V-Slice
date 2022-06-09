using UnityEngine;

namespace ActorSystem.AI.Traps
{
    public class Spike : MonoBehaviour
    {
        public float m_damage = 0f;
        private float m_decay = 0.5f;
        public void OnTriggerStay(Collider other)
        {
            if (m_damage <= 0)
                return;

            if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                other.GetComponent<Player_Controller>().DamagePlayer(m_damage, CombatSystem.DamageType.Physical, null, true);
            }
            else if(other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
            {
                other.GetComponent<Actor>().DealDamage(m_damage, CombatSystem.DamageType.Physical, 0, null);
            }
        }

        public void LateUpdate()
        {
            if (m_damage <= 0.005f)
                m_damage = 0;

            m_damage = Mathf.Clamp(m_damage * m_decay, 0, float.MaxValue);
        }
    }
}
