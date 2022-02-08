using Actor.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewaveProjectile : MonoBehaviour
{
    private List<Enemy> m_hitList = new List<Enemy>();
    public float m_speed = 25.0f;
    private float m_lifeTimer = 0.0f;
    public AbilityData m_data;
    [SerializeField] private GameObject flameParticles;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * m_speed * Time.fixedDeltaTime;
        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            ParticleSystem[] particleSystems = flameParticles.GetComponentsInChildren<ParticleSystem>();
            foreach (var particles in particleSystems)
            {
                particles.Stop();
            }

            flameParticles.transform.SetParent(null);
            flameParticles.GetComponent<VFXTimerScript>().m_startedTimer = true;
            Destroy(gameObject);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Enemy actor = other.GetComponentInParent<Enemy>();
            if (actor != null && !m_hitList.Contains(actor))
            {
                Debug.Log("Hit " + other.name + " with " + m_data.damage + " for " + m_data.damage);
                actor.DealDamage(m_data.damage);
                m_hitList.Add(actor);
            }

            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                status.AddStatusEffect(new BurnStatus(m_data.effectiveness, m_data.duration));
            }
        }
    }
}
