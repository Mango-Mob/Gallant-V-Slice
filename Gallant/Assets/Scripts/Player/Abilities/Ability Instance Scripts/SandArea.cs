using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandArea : MonoBehaviour
{
    public AbilityData m_data;
    private float m_lifeTimer = 0.0f;
    public ParticleSystem sandParticles { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        sandParticles = GetComponentInChildren<ParticleSystem>();

        //if (sandParticles != null)
        //{
        //    ParticleSystem.MainModule mainModule = sandParticles.main;
        //    mainModule.duration = m_data.lifetime;
        //    sandParticles.Play();
        //}
    }

    // Update is called once per frame
    void Update()
    {
        m_lifeTimer += Time.deltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            sandParticles.Stop();
            sandParticles.transform.SetParent(null);
            sandParticles.GetComponent<VFXTimerScript>().m_startedTimer = true;
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

            }
            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                status.AddStatusEffect(new WeakenStatus(m_data.effectiveness, m_data.duration));
            }
        }
    }
}
