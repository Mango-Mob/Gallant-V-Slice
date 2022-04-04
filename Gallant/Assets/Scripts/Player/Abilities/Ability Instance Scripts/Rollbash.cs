using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using ActorSystem.AI;

public class Rollbash : MonoBehaviour
{
    List<GameObject> m_hitList = new List<GameObject>();

    [HideInInspector] public Player_Controller playerController;
    [HideInInspector] public AbilityData m_data;
    [SerializeField] public GameObject m_explodeVFX;
    public GameObject m_model;

    public float m_scaleSpeed = 5.0f;
    private float m_startScale = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        m_startScale = m_model.transform.localScale.x;
        m_model.transform.localScale = Vector3.zero;
    }

    // Update is called once per frame
    void Update()
    {
        transform.Rotate(-playerController.playerMovement.playerModel.transform.forward + playerController.playerMovement.playerModel.transform.right, playerController.playerMovement.m_rollSpeed * Time.deltaTime * 90.0f);
        
        if (m_model.transform.localScale.x < m_startScale)
            m_model.transform.localScale += Vector3.one * Time.deltaTime * m_scaleSpeed;
    }
    private void OnDestroy()
    {
        GameObject vfx = Instantiate(m_explodeVFX, transform.position, Quaternion.identity);

    }

    private void OnTriggerEnter(Collider other)
    {
        if (m_hitList.Contains(other.gameObject))
            return;

        if (playerController.playerAttack.m_attackTargets == (playerController.playerAttack.m_attackTargets | (1 << other.gameObject.layer)))
        {
            playerController.playerAttack.DamageTarget(other.gameObject, m_data.damage, m_data.effectiveness, 0, CombatSystem.DamageType.Ability);
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                actor.KnockbackActor((actor.transform.position - transform.position).normalized * m_data.effectiveness);
                m_hitList.Add(actor.gameObject);
            }

            playerController.playerAttack.CreateVFX(other, transform.position);
        }
    }
}
