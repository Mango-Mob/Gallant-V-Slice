using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class BarrierProjectile : MonoBehaviour
{
    public float m_barrierValue;
    public float m_speed = 10.0f;
    private float m_lifeTimer = 0.0f;
    public AbilityData m_data;

    [SerializeField] private GameObject particles;
    [SerializeField] private GameObject model;
    [SerializeField] private Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        transform.localScale = new Vector3(1, 1, 1) * (0.5f + (m_barrierValue / 40.0f));
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * m_speed * Time.fixedDeltaTime;
        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            Detonate();
        }
    }

    private void Detonate()
    {
        if (particles != null)
        {
            ParticleSystem[] particleSystems = particles.GetComponentsInChildren<ParticleSystem>();
            foreach (var particles in particleSystems)
            {
                particles.Stop();
            }

            particles.transform.SetParent(null);
            particles.GetComponent<VFXTimerScript>().m_startedTimer = true;
        }

        animator.SetTrigger("Explode");
        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                if (actor.CheckIsDead())
                    return;

                actor.DealDamage(m_data.damage * m_barrierValue / 50.0f);
                Detonate();
            }
        }
    }
}
