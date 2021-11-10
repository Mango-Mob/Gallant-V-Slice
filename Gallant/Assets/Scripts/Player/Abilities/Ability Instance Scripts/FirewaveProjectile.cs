using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewaveProjectile : MonoBehaviour
{
    public float m_speed = 10.0f;
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
            Debug.Log("Hit " + other.name + " with " + m_data.damage + " for " + m_data.damage);

            Actor actor = other.GetComponent<Actor>();
            if (actor != null)
            {
                actor.DealDamage(m_data.damage);
            }

            StatusEffectContainer status = other.GetComponent<StatusEffectContainer>();
            if (status != null)
            {
                status.AddStatusEffect(new BurnStatus(m_data.effectiveness, m_data.duration));
            }
        }
    }
}
