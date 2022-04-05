using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Whirlpool : MonoBehaviour
{
    public AbilityData m_data;
    private float m_lifeTimer = 0.0f;
    public ParticleSystem particles { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        particles = GetComponentInChildren<ParticleSystem>();
    }

    // Update is called once per frame
    void Update()
    {
        m_lifeTimer += Time.deltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            particles.Stop();
            particles.transform.SetParent(null);
            particles.GetComponent<VFXTimerScript>().m_startedTimer = true;
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                actor.KnockbackActor((actor.transform.position - transform.position).normalized * m_data.effectiveness * Time.fixedDeltaTime);
            }
            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                
            }
        }
    }
}
