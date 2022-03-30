using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneboltProjectile : MonoBehaviour
{
    [SerializeField] private GameObject m_impactPrefab;

    public Player_Controller playerController;
    public ParticleSystem[] m_particleSystems { get; private set; }

    public AbilityData m_data;
    private bool m_spawning = true;
    private float m_targetScale;
    private float m_scaleLerp = 0.0f;

    [SerializeField] private float m_startSpeed = 3.0f;
    [SerializeField] private float m_forceMultiplier = 15.0f;
    [SerializeField] private float m_targetRange = 15.0f;
    [SerializeField] private float m_lifetimeProjectile = 5.0f;

    private float m_lifetimeTimer = 0.0f;
    private Rigidbody m_rigidbody;

    // Start is called before the first frame update
    void Start()
    {
        m_particleSystems = GetComponentsInChildren<ParticleSystem>();
        m_rigidbody = GetComponent<Rigidbody>();
        m_targetScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
        StartCoroutine(Spawning());

        m_rigidbody.velocity = transform.forward * m_startSpeed;
    }

    // Update is called once per frame
    void Update()
    {
        if (m_spawning)
        {
            m_scaleLerp += Time.deltaTime * 4.0f;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.0f, 1.0f, 1.0f) * m_targetScale, m_scaleLerp);
        }

        m_lifetimeTimer += Time.deltaTime;
        if (m_lifetimeTimer >= m_lifetimeProjectile)
        {
            DetonateProjectile();
        }
    }
    private void DetonateProjectile()
    {
        if (playerController)
            playerController.playerAudioAgent.SandmissileImpact();

        foreach (var system in m_particleSystems)
        {
            system.Stop();
            system.transform.SetParent(null);
            system.GetComponent<VFXTimerScript>().m_startedTimer = true;
        }
        if (m_impactPrefab != null)
        {
            GameObject impact = Instantiate(m_impactPrefab,
                transform.position,
                transform.rotation);
        }
        Destroy(gameObject);
    }
    IEnumerator Spawning()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        m_spawning = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable") && other.GetComponentInParent<Actor>() != null)
        {
            Debug.Log("Hit " + other.name + " with arcane bolt for " + m_data.damage);

            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                if (actor.m_myBrain.IsDead)
                    return;

                actor.DealDamage(m_data.damage, CombatSystem.DamageType.Ability);
                DetonateProjectile();
            }
        }
    }
}
