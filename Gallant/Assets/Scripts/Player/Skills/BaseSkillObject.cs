using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public abstract class BaseSkillObject : MonoBehaviour
{
    public float m_strength = 1.0f;
    public float m_lifetime = 1.0f;
    public List<GameObject> particles { get; private set; } = new List<GameObject>();
    private float m_lifeTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        VisualEffect vfx = gameObject.GetComponentInChildren<VisualEffect>();
        vfx.SetFloat("life time", m_lifetime);
        vfx.enabled = true;
    }

    void FixedUpdate()
    {
        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_lifetime)
        {
            foreach (var vfx in particles)
            {
                vfx.transform.SetParent(null);
                vfx.GetComponent<VFXTimerScript>().m_startedTimer = true;
            }
            Destroy(gameObject);
            return;
        }
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
                AddStatusEffect(status);
            }
        }
    }
    protected abstract void ApplyToActor(Actor _actor);
    protected abstract void AddStatusEffect(StatusEffectContainer _container);
}
