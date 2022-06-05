using UnityEngine;

namespace ActorSystem.AI.Other
{
    public class ButtSlamIndicator : VFXTimerScript
    {
        public float m_slamDamage = 10f;
        public AudioClip m_damageClip;
        public GameObject m_vfxPrefab;

        public void Damage()
        {
            GameObject target = GameManager.Instance.m_player;
            float dist = Vector3.Distance(target.transform.position, transform.position);
            if (dist <= 15.0f)
            {
                target.GetComponent<Player_Controller>().ScreenShake(10 * (1.0f - dist / 15f), 0.3f);
                if (dist <= 5.0f)
                {
                    target.GetComponent<Player_Controller>().DamagePlayer(m_slamDamage, CombatSystem.DamageType.Physical, this.gameObject, true);
                    target.GetComponent<Player_Controller>().StunPlayer(0.3f, (target.transform.position - transform.position).normalized * 5f);
                }
            }
            AudioManager.Instance.PlayAudioTemporary(transform.position, m_damageClip);
            Instantiate(m_vfxPrefab, transform.position, transform.rotation);
        }
    }
}
