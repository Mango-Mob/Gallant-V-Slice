using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AreaEffect: MonoBehaviour
{
    public GameObject m_EffectVFX;
    public AudioClip m_clip;

    public float delay;
    public float damage;
    public float radius;

    private float timer = 0.0f;
    public AttackData m_data;

    public void Update()
    {
        timer += Time.deltaTime;

        if(timer > delay)
        {
            ApplyEffect();
            Destroy(gameObject);
        }
    }

    private void ApplyEffect()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius);

        foreach (var hit in hits)
        {
            if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
            {
                Player_Controller player = hit.GetComponent<Player_Controller>();
                if (player != null)
                {
                    player.DamagePlayer(damage);
                    AttackData.ApplyEffect(player, transform, m_data.effectAfterwards, m_data.effectPower);
                }
            }
        }

        Instantiate(m_EffectVFX, transform.position, transform.rotation);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.white;
        Gizmos.DrawWireSphere(transform.position, radius);
    }
}
