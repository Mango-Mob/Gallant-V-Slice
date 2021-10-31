using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    public AbilityData m_data;
    [SerializeField] private LineRenderer lineRenderer;
    private List<Actor> m_hitTargets = new List<Actor>();
    public int m_maxTargets = 3;
    public Transform m_handTransform;
    public float m_range = 10.0f;

    private float m_lifeTimer = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        lineRenderer.positionCount = 1;
        lineRenderer.SetPosition(0, m_handTransform.position);

        Vector3 lastTargetPos = m_handTransform.position;
        Actor[] actors = FindObjectsOfType<Actor>();

        for (int i = 0; i < m_maxTargets; i++)
        {
            Actor bestTarget = null;
            float closestDistance = Mathf.Infinity;
            foreach (var actor in actors)
            {
                if (actor == bestTarget || m_hitTargets.Contains(actor))
                    continue;

                Vector3 directionToTarget = actor.transform.position - lastTargetPos;
                float distance = directionToTarget.sqrMagnitude;

                if (distance > m_range)
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

    // Update is called once per frame
    void FixedUpdate()
    {
        lineRenderer.SetPosition(0, m_handTransform.position);
        foreach (var target in m_hitTargets)
        {
            lineRenderer.SetPosition(m_hitTargets.IndexOf(target) + 1, target.transform.position);
        }

        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            Destroy(gameObject);
        }
    }
}
