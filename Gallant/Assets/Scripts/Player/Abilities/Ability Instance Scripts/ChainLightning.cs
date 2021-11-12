using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChainLightning : MonoBehaviour
{
    public Player_Controller m_user;
    public LayerMask m_enemyDetectionMask;
    public AbilityData m_data;
    [SerializeField] private GameObject lightningPrefabVFX;
    [SerializeField] private LineRenderer lineRenderer;
    private List<Actor> m_hitTargets = new List<Actor>();
    public int m_maxTargets = 3;
    public Transform m_handTransform;

    public float m_hitAngle = 45.0f;
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
        List<Actor> closeActors = m_user.GetActorsInfrontOfPlayer(m_hitAngle, m_hitRange);

        float closestDistance = Mathf.Infinity;
        Actor closestTarget = null;

        foreach (var actor in closeActors)
        {
            float distance = Vector3.Distance(actor.transform.position, transform.position);

            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestTarget = actor;
            }
        }

        if (closestTarget != null)
        {
            m_hitTargets.Add(closestTarget);
            closestTarget.DealDamage(m_data.damage);

            CreateVFX(transform.position, closestTarget.transform.position);

            lastTargetPos = closestTarget.transform.position;
            //lineRenderer.SetPosition(1, closestTarget.transform.position);
        }

        StartCoroutine(TargetSearch(lastTargetPos, actors));

        if (m_hitTargets.Count == 0)
        {
            lineRenderer.SetPosition(1, transform.position + transform.forward * m_hitRange);
            return;
        }

        
    }

    IEnumerator TargetSearch(Vector3 _lastPosition, Actor[] _enemies)
    {
        float waitTime = 0.15f;
        yield return new WaitForSeconds(waitTime);

        for (int i = 1; i < m_maxTargets; i++)
        {
            Actor bestTarget = null;
            float closestDistance = Mathf.Infinity;
            foreach (var actor in _enemies)
            {
                if (actor == bestTarget || m_hitTargets.Contains(actor))
                    continue;

                float distance = Vector3.Distance(actor.transform.position, _lastPosition);

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

                CreateVFX(_lastPosition, bestTarget.transform.position);

                _lastPosition = bestTarget.transform.position;
                yield return new WaitForSeconds(waitTime);
            }
            else
            {
                break;
            }
        }
    }

    private void CreateVFX(Vector3 _origin, Vector3 _target)
    {
        // Create VFX
        GameObject vfx = Instantiate(lightningPrefabVFX, _origin, transform.rotation);

        Vector3 direction = _target - _origin;
        vfx.transform.forward = (_target - _origin).normalized;
        vfx.transform.localScale = new Vector3(vfx.transform.localScale.x , 
            vfx.transform.localScale.y, 
            vfx.transform.localScale.z * 0.1666f * direction.magnitude);
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawWireSphere(transform.position, m_chainRange);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        //lineRenderer.SetPosition(0, m_handTransform.position);
        //foreach (var target in m_hitTargets)
        //{
        //    if (target != null)
        //        lineRenderer.SetPosition(m_hitTargets.IndexOf(target) + 1, target.transform.position);
        //}

        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_data.lifetime)
        {
            Destroy(gameObject);
        }
    }
}
