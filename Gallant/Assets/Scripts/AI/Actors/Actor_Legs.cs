using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Actor_Legs : MonoBehaviour
{
    public Vector3 velocity { get{ return m_agent.velocity; } }
    public Vector3 localVelocity { get {return Quaternion.AngleAxis(transform.rotation.eulerAngles.y, -Vector3.up) * m_agent.velocity;}}

    public float m_angleSpeed = 45f;

    protected NavMeshAgent m_agent;
    protected Vector3 m_targetPosition;
    protected Quaternion m_targetRotation;

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_angleSpeed * Time.deltaTime);
    }

    public bool IsResting()
    {
        return (m_agent.velocity.magnitude < 0.15f && Vector3.Distance(m_targetPosition, transform.position) < 0.5f) || m_agent.isStopped;
    }

    public void Halt()
    {
        m_agent.isStopped = true;
    }

    public void SetTargetLocation(Vector3 target, bool lookAtTarget = false)
    {
        m_agent.isStopped = false;
        m_agent.destination = target;
        m_targetPosition = target;

        Vector3 direction = m_agent.destination - transform.position;
        direction.y = 0;

        if (lookAtTarget)
            SetTargetRotation(Quaternion.LookRotation(direction.normalized, Vector3.up));
    }

    public void SetTargetRotation(Quaternion rotation)
    {
        m_targetRotation = rotation;
    }
}
