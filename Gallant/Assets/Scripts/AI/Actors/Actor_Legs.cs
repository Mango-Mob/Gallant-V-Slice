using System.Collections;
using UnityEngine;
using UnityEngine.AI;

/****************
 * Actor_Legs : An navmesh accessor to be used by an Actor.
 * @author : Michael Jordan
 * @file : Actor_Legs.cs
 * @year : 2021
 */
public class Actor_Legs : MonoBehaviour
{
    [HideInInspector] 
    public float m_baseSpeed;

    [Range(0.0f, 2.0f)]
    public float m_speedModifier = 1.0f;

    //External Accessors
    public Vector3 velocity { get{ return m_agent.velocity; } }
    public Vector3 localVelocity { get {return Quaternion.AngleAxis(transform.rotation.eulerAngles.y, -Vector3.up) * m_agent.velocity;}}

    //Statistics:
    public float m_angleSpeed = 45f;

    //Accessables:
    protected NavMeshAgent m_agent;
    
    //Target Orientation
    protected Vector3 m_targetPosition;
    protected Quaternion m_targetRotation;

    private bool m_isKnocked = false;

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isKnocked)
            return;

        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_angleSpeed * m_speedModifier * Time.deltaTime);
        m_agent.destination = m_targetPosition;
        m_agent.speed = m_baseSpeed * m_speedModifier;
    }

    /*********************
     * IsResting : Is the actor's legs currently resting?
     * @author : Michael Jordan
     * @return : (bool) true if the actor's legs are either not moving or if the velocity value is small enough.
     */
    public bool IsResting()
    {
        return (m_agent.velocity.magnitude < 0.15f && Vector3.Distance(m_targetPosition, transform.position) < 0.5f) || m_agent.isStopped;
    }

    /*********************
     * Halt : Stops the actor's legs.
     * @author : Michael Jordan
     */
    public void Halt()
    {
        m_agent.isStopped = true;
    }

    /*******************
    * SetTargetLocation : Sets the target destination of the navmesh
    * @author : Michael Jordan
    * @param : (Vector3) position in world space of the target destination.
    * @param : (bool) if the actor should set it's target rotation to look towards the destination (default = false).
    */
    public void SetTargetLocation(Vector3 target, bool lookAtTarget = false)
    {
        m_agent.isStopped = false;
        m_targetPosition = target;

        Vector3 direction = m_targetPosition - transform.position;
        direction.y = 0;

        if (lookAtTarget)
            SetTargetRotation(Quaternion.LookRotation(direction.normalized, Vector3.up));
    }

    /*******************
    * SetTargetRotation : Sets the target rotation of the actor.
    * @author : Michael Jordan
    * @param : (Quaternion) target rotation of the actor.
    */
    public void SetTargetRotation(Quaternion rotation)
    {
        m_targetRotation = rotation;
    }

    /*******************
    * SetTargetRotation : Gets the total angle of rotation to looktowards a GameObject.
    * @author : Michael Jordan
    * @param : (GameObject) target to calculate towards.
    */
    public float GetAngleTowards(GameObject _target)
    {
        if (_target == null)
            return 0;

        Quaternion lookAt = Quaternion.LookRotation((_target.transform.position - transform.position).normalized, Vector3.up);
        return Quaternion.Angle(transform.rotation, lookAt);
    }

    public void KnockBack(Vector3 force, float mass)
    {
        Vector3 direct = force.normalized;
        float power = force.magnitude;

        float percentage = (100 / (100 + mass));

        Vector3 targetLoc = transform.position + direct * (power * percentage);

        StartCoroutine(KnockbackRoutine(targetLoc));
    }

    private IEnumerator KnockbackRoutine(Vector3 targetlocation)
    {
        m_isKnocked = true;
        m_agent.destination = targetlocation;
        NavMeshPath path = new NavMeshPath();
        //Calculate if the knockback is in a straight line
        if(m_agent.CalculatePath(targetlocation, path))
        {
            //path.corners;
            if(path.corners.Length > 2)
            {
                //We have a turn, replace with straight line
                m_agent.destination = path.corners[1];
            }
        }

        float startAngSpeed = m_agent.angularSpeed;
        float startSpeed = m_agent.speed;
        float startAccel = m_agent.acceleration;

        m_agent.angularSpeed = 0;
        m_agent.speed = 150;
        m_agent.acceleration = 300;

        while (Vector3.Distance(transform.position, m_agent.destination) > 1.0f)
        {
            yield return new WaitForEndOfFrame();
        }

        m_agent.angularSpeed = startAngSpeed;
        m_agent.speed = startSpeed;
        m_agent.acceleration = startAccel;
        m_isKnocked = false;

        yield return false;
    }
}
