using ActorSystem.AI;
using ActorSystem.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileObject : MonoBehaviour
{
    public float m_damage { private get; set; }
    public float m_duration { private get; set; } = -1;
    public Vector3 m_velocity { private get; set; } = Vector3.zero;

    public GameObject m_hitVfX;
    public AudioClip m_hitSound;
    public AttackData m_damageDetails;

    private float m_timer = 0;

    private void Update()
    {
        if(m_duration >= 0)
        {
            m_timer += Time.deltaTime;

            if(m_timer >= m_duration)
            {
                Instantiate(m_hitVfX, transform.position, Quaternion.identity);
                Destroy(gameObject);
            }
        }
    }

    private void FixedUpdate()
    {
        GetComponent<Rigidbody>().velocity = m_velocity;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Boss")
            return;

        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Player_Controller player = other.GetComponent<Player_Controller>();
            player.DamagePlayer(m_damage, CombatSystem.DamageType.Physical, gameObject);
            AttackData.ApplyEffect(player, transform, m_damageDetails.onHitEffect, m_damageDetails.effectPower);
            Instantiate(m_hitVfX, transform.position, Quaternion.identity);

            AudioManager.Instance.PlayAudioTemporary(transform.position, m_hitSound);
            Destroy(gameObject);
        }
    }
}
