using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSwordRain : BaseAbilityProjectile
{
    [SerializeField] private ParticleSystem[] m_particleRings;
    private int m_nextRing = 0;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
        m_knockbackScalar = m_data.effectiveness;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_nextRing < m_data.starPowerLevel && m_particleRings[m_nextRing].main.startDelay.constant + 0.3f < m_lifeTimer)
        {
            ParticleSystem ring = m_particleRings[m_nextRing];

            Collider[] colliders = Physics.OverlapCapsule(transform.position - Vector3.up, transform.position + Vector3.up, (ring.shape.radius + ring.shape.donutRadius * 1.3f) * 1.5f, playerController.playerAttack.m_attackTargets);
            foreach (var collider in colliders)
            {
                Vector3 targetDifference = collider.transform.position - transform.position;
                targetDifference.y = 0.0f;

                if (targetDifference.magnitude > (ring.shape.radius - ring.shape.donutRadius * 1.3f) * 1.5f)
                {
                    if (ProjectileCollide(collider))
                    {
                        //StatusEffectContainer status = collider.GetComponentInParent<StatusEffectContainer>();
                        //if (status != null)
                        //{
                        //    status.AddStatusEffect(new BurnStatus(m_data.effectiveness * playerController.playerStats.m_abilityDamage, m_data.duration));
                        //}
                    }
                }
            }

            Debug.Log("Ring " + (m_nextRing + 1) + ". Delay: " + m_particleRings[m_nextRing].main.startDelay.constant);

            m_nextRing++;
            m_hitList.Clear();
        }
    }

    protected override void DetonateProjectile(bool hitTarget = false)
    {
        if (m_hitSound)
            AudioManager.Instance.PlayAudioTemporary(transform.position, m_hitSound, AudioManager.VolumeChannel.SOUND_EFFECT);

        Destroy(gameObject);
    }
    private void OnDrawGizmos()
    {
        foreach (var ring in m_particleRings)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(transform.position, (ring.shape.radius + ring.shape.donutRadius * 1.3f) * 1.5f);
            Gizmos.color = Color.red;
            Gizmos.DrawWireSphere(transform.position, (ring.shape.radius - ring.shape.donutRadius * 1.3f) * 1.5f);
        }
    }
}
