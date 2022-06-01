using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Whirlpool : MonoBehaviour
{
    public GameObject VFX;

    private bool m_endPull = false;
    [SerializeField] private float m_spawnSpeed = 4.0f;
    [SerializeField] private WhirlpoolMove m_whirlpoolMove;
    public Animator animator;

    // Start is called before the first frame update
    void Start()
    {
        Animator animator = GetComponentInChildren<Animator>();
        animator.speed = animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / m_whirlpoolMove.m_data.duration;

        
    }
    // Update is called once per frame
    void Update()
    {
    }
    private void FixedUpdate()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (m_endPull)
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                Vector3 direction = transform.position - actor.transform.position;
                direction.y = 0.0f;
                if (direction.magnitude > 0.5f)
                {
                    Vector3 forward = direction.normalized * m_whirlpoolMove.m_data.effectiveness;
                    actor.KnockbackActor(Quaternion.Euler(0.0f, 65.0f, 0.0f) * forward);
                }
            }
            StatusEffectContainer status = other.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {
                
            }
        }
    }

    public void EndEvent()
    {
        m_endPull = true;
        Collider[] colliders = Physics.OverlapSphere(transform.position, 3.0f);
        foreach (var collider in colliders)
        {
            Actor actor = collider.GetComponentInParent<Actor>();
            if (actor != null)
            {
                Vector3 direction = actor.transform.position - transform.position;
                direction.y = 0.0f;
                actor.KnockbackActor(direction.normalized * 10.0f);
            }
            StatusEffectContainer status = collider.GetComponentInParent<StatusEffectContainer>();
            if (status != null)
            {

            }
        }
    }
}
