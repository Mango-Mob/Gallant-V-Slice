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

    // Start is called before the first frame update
    void Awake()
    {
        m_agent = GetComponent<NavMeshAgent>();
    }

    // Update is called once per frame
    void Update()
    {
        transform.rotation = Quaternion.RotateTowards(transform.rotation, m_targetRotation, m_angleSpeed * m_speedModifier * Time.deltaTime);
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
        m_agent.destination = target;
        m_targetPosition = target;

        Vector3 direction = m_agent.destination - transform.position;
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
}
