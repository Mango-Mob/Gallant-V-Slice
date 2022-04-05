using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcaneboltProjectile : BaseAbilityProjectile
{
    [SerializeField] private GameObject m_impactPrefab;

    private bool m_spawning = true;
    private float m_targetScale;
    private float m_scaleLerp = 0.0f;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

        m_targetScale = transform.localScale.x;
        transform.localScale = Vector3.zero;
        StartCoroutine(Spawning());

    }

    // Update is called once per frame
    void Update()
    {
        if (m_spawning)
        {
            m_scaleLerp += Time.deltaTime * 4.0f;
            transform.localScale = Vector3.Lerp(Vector3.zero, new Vector3(1.0f, 1.0f, 1.0f) * m_targetScale, m_scaleLerp);
        }
    }
    protected override void DetonateProjectile(bool hitTarget = false)
    {
        BaseDetonateProjectile(hitTarget);
        if (m_impactPrefab != null)
        {
            GameObject impact = Instantiate(m_impactPrefab,
                transform.position,
                transform.rotation);

            ParticleSystem[] particleSystems = impact.GetComponentsInChildren<ParticleSystem>();
            foreach (var particle in particleSystems)
            {
                ParticleSystem.MainModule mainModule = particle.main;
                Color newColor = m_data.droppedEnergyColor;
                newColor.a = mainModule.startColor.color.a;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(newColor);
            }
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
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            if (ProjectileCollide(other))
            {
                DetonateProjectile();
            }
        }
    }
}
