using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    public LayerMask m_enemyDetectionMask;
    public AbilityData m_data;
    [SerializeField] private LineRenderer lineRenderer;
    private List<Actor> m_hitTargets = new List<Actor>();
    public int m_maxTargets = 3;
    public Transform m_handTransform;

    public float m_hitRange = 10.0f;
    public float m_chainRange = 3.0f;

    private float m_lifeTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = 2;
        lineRenderer.SetPosition(0, m_handTransform.position);

        Vector3 lastTargetPos = m_handTransform.position;
        Actor[] actors = FindObjectsOfType<Actor>();

        RaycastHit[] hits = Physics.SphereCastAll(transform.position + transform.forward * m_chainRange, m_chainRange, transform.forward, m_hitRange, m_enemyDetectionMask);

        foreach (var hit in hits)
        {
            Actor actor = hit.collider.GetComponent<Actor>();
            if (actor != null)
            {
                if (actor.CheckIsDead())
                    continue;

                lineRenderer.SetPosition(1, actor.transform.position);
                m_hitTargets.Add(actor);
                actor.DealDamage(m_data.damage);
                lastTargetPos = actor.transform.position;
                break;
            }
        }

        if (m_hitTargets.Count == 0)
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * (m_hitRange + m_chainRange));
            return;
        }

        for (int i = 1; i < m_maxTargets; i++)
        {
            Actor bestTarget = null;
            float closestDistance = Mathf.Infinity;
            foreach (var actor in actors)
            {
                if (actor == bestTarget || m_hitTargets.Contains(actor))
                    continue;

                float distance = Vector3.Distance(actor.transform.position, lastTargetPos);

                if (distance > m_chainRange)
                    continue;

                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    bestTarget = actor;
                }
            }

            if (bestTarget != null)
            {
                lineRenderer.positionCount = i + 2;
                lineRenderer.SetPosition(i + 1, bestTarget.transform.position);
                m_hitTargets.Add(bestTarget);
                bestTarget.DealDamage(m_data.damage);
                lastTargetPos = bestTarget.transform.position;
            }
            else
            {
                break;
            }
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_chainRange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, m_handTransform.position);
        foreach (var target in m_hitTargets)
        {
            if (target != null)
                lineRenderer.SetPosition(m_hitTargets.IndexOf(target) + 1, target.transform.position);
        }

        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            Destroy(gameObject);
        }
    }
}
