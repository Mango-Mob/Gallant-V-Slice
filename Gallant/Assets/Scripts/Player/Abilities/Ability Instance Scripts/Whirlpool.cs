using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;

public class Whirlpool : MonoBehaviour
{
    public AbilityData m_data;
    private float m_lifeTimer = 0.0f;
    public GameObject VFX;

    [SerializeField] private float m_spawnSpeed = 4.0f;
    private int m_spawnState = 0;
    private float m_scaleLerp = 0.0f;
    private Vector3 m_targetScale;

    // Start is called before the first frame update
    void Start()
    {
        m_targetScale = VFX.transform.localScale;
        Animator animator = GetComponentInChildren<Animator>();
        animator.speed =animator.GetCurrentAnimatorClipInfo(0)[0].clip.length / m_data.duration;
    }
    // Update is called once per frame
    void Update()
    {
        //switch (m_spawnState)
        //{
        //    case 0:
        //        m_scaleLerp += Time.deltaTime * m_spawnSpeed;
        //        break;
        //    case 2:
        //        m_scaleLerp -= Time.deltaTime * m_spawnSpeed;
        //        break;
        //    default:
        //        break;
        //}

        //VFX.transform.localScale = Vector3.Lerp(Vector3.zero, m_targetScale, m_scaleLerp);
        //if (Mathf.Abs(m_lifeTimer - m_data.duration) < (1.0f / m_spawnSpeed))
        //{
        //    m_spawnState = 2;
        //}
        //else if (m_scaleLerp >= 1.0f)
        //{
        //    m_spawnState = 1;
        //}

        m_lifeTimer += Time.deltaTime;
        if (m_lifeTimer > m_data.duration)
        {
            VFX.transform.SetParent(null);
            VFX.GetComponent<VFXTimerScript>().m_startedTimer = true;
            Destroy(gameObject);
        }
    }
    private void FixedUpdate()
    {

    }
    private void OnTriggerStay(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                Vector3 direction = transform.position - actor.transform.position;
                direction.y = 0.0f;
                if (direction.magnitude > 0.5f)
                {
                    Vector3 forward = direction.normalized * m_data.effectiveness;
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
        Debug.Log("END NADO");
    }
}
