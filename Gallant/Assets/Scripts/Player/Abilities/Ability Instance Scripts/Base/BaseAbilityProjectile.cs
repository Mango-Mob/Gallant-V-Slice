using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseAbilityProjectile : MonoBehaviour
{
    [HideInInspector] public Player_Controller playerController;
    private List<Actor> m_hitList = new List<Actor>();
    [HideInInspector] public AbilityData m_data;
    [SerializeField] private GameObject m_particles;
    [SerializeField] private AudioClip m_hitSound;

    public float m_overwriteLifetime = 0.0f;

    public float m_speed = 25.0f;
    private float m_lifeTimer = 0.0f;

    // Start is called before the first frame update
    protected void Start()
    {
        playerController = FindObjectOfType<Player_Controller>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * m_speed * Time.fixedDeltaTime;
        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > (m_overwriteLifetime <= 0.0f ? m_data.lifetime : m_overwriteLifetime))
        {
            DetonateProjectile();
        }
    }

    protected void BaseDetonateProjectile(bool hitTarget = false)
    {
        ParticleSystem[] particleSystems = m_particles.GetComponentsInChildren<ParticleSystem>();
        foreach (var particles in particleSystems)
        {
            particles.Stop();
        }

        m_particles.transform.SetParent(null);
        m_particles.GetComponent<VFXTimerScript>().m_startedTimer = true;

        if (m_hitSound)
            AudioManager.Instance.PlayAudioTemporary(transform.position, m_hitSound);
    }
    protected abstract void DetonateProjectile(bool hitTarget = false);
    protected bool ProjectileCollide(Collider other)
    {
        Actor actor = other.GetComponentInParent<Actor>();
        if (actor != null && m_hitList.Contains(actor))
            return false;

        playerController.playerAttack.DamageTarget(other.gameObject, m_data.damage, 0, 0, CombatSystem.DamageType.Ability, m_data.m_tags);

        if (actor != null && !m_hitList.Contains(actor))
        {
            if (actor.m_myBrain.IsDead)
                return false;

            //Debug.Log("Hit " + other.name + " with " + m_data.damage + " for " + m_data.damage);
            //actor.DealDamage(m_data.damage, CombatSystem.DamageType.Ability);
            //playerController.playerAttack.DamageTarget(other.gameObject, m_data.damage, 0, 0, CombatSystem.DamageType.Ability);
            m_hitList.Add(actor);
            return true;
        }
        return false;
    }

}
