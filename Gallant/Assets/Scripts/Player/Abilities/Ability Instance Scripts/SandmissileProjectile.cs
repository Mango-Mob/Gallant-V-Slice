using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandmissileProjectile : MonoBehaviour
{
    [SerializeField] private GameObject m_sandAreaPrefab;
    [SerializeField] private GameObject m_sandPoofPrefab;

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
    private void FixedUpdate()
    {
        // Homing code
        //Collider[] colliders = Physics.OverlapSphere(transform.position, m_targetRange);

        //List<Collider> actors = new List<Collider>();
        //foreach (var collider in colliders)
        //{
        //    if (collider.GetComponentInParent<Actor>())
        //        actors.Add(collider);
        //}

        //if (actors.Count > 0)
        //{
        //    float closestDistance = Mathf.Infinity;
        //    Collider closestTarget = null;

        //    foreach (var actor in actors)
        //    {
        //        float distance = Vector3.Distance(actor.transform.position, transform.position);

        //        if (distance < closestDistance)
        //        {
        //            closestDistance = distance;
        //            closestTarget = actor;
        //        }
        //    }

        //    //m_rigidbody.AddForce(Time.fixedDeltaTime * (closestTarget.transform.position - transform.position + transform.up).normalized * m_forceMultiplier /* * (1.0f - (closestDistance / m_targetRange))*/);
        //}
    }
    private void DetonateProjectile()
    {
        foreach (var system in m_particleSystems)
        {
            system.Stop();
            system.transform.SetParent(null);
            system.GetComponent<VFXTimerScript>().m_startedTimer = true;
        }
        if (m_sandPoofPrefab != null)
        {
            GameObject poof = Instantiate(m_sandPoofPrefab,
                transform.position,
                transform.rotation);
        }
        if (m_sandAreaPrefab != null)
        {
            GameObject area = Instantiate(m_sandAreaPrefab,
                transform.position,
                transform.rotation);

            area.GetComponent<SandArea>().m_data = m_data;
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
            Debug.Log("Hit " + other.name + " with sand for " + m_data.damage);

            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                if (actor.CheckIsDead())
                    return;

                actor.DealDamage(m_data.damage);
                DetonateProjectile();
            }
        }
    }
}
