using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WhirlpoolProjectile : BaseAbilityProjectile
{
    [SerializeField] private GameObject m_whirlpoolAreaPrefab;
    [SerializeField] private GameObject m_detonatePrefab;

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

        if (m_detonatePrefab != null)
        {
            GameObject poof = Instantiate(m_detonatePrefab,
                transform.position,
                transform.rotation);
        }
        if (m_whirlpoolAreaPrefab != null)
        {
            GameObject area = Instantiate(m_whirlpoolAreaPrefab,
                transform.position,
                transform.rotation);

            area.GetComponent<Whirlpool>().m_data = m_data;
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
