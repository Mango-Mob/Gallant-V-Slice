using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Whirlpool : MonoBehaviour
{
    public AbilityData m_data;
    private float m_lifeTimer = 0.0f;
    public VisualEffect VFX;

    // Start is called before the first frame update
    void Start()
    {
        VFX.SetFloat("Duration", m_data.duration);
    }

    // Update is called once per frame
    void Update()
    {
        m_lifeTimer += Time.deltaTime;
        if (m_lifeTimer > m_data.duration)
        {
            VFX.transform.SetParent(null);
            VFX.GetComponent<VFXTimerScript>().m_startedTimer = true;
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
                Vector3 direction = transform.position - actor.transform.position;
                direction.y = 0.0f;
                Vector3 forward = direction.normalized * m_data.effectiveness;
                actor.KnockbackActor(Quaternion.Euler(0.0f, 15.0f, 0.0f) * forward);
            }
            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                
            }
        }
    }
}
