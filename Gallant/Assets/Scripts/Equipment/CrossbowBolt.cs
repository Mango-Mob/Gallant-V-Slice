using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrossbowBolt : MonoBehaviour
{
    public GameObject[] m_effects;

    private Player_Attack m_projectileUser; // The user of the projectile
    private float m_projectileSpeed = 10.0f;
    public WeaponData m_weaponData;

    private float m_lifeTimer = 0.0f;
    private float m_lifeDuration = 1.0f;

    // Start is called before the first frame update
    void Start()
    {
        GetComponent<SphereCollider>().radius = m_weaponData.hitSize;

        if (m_weaponData != null)
        {
            m_projectileSpeed = m_projectileSpeed * m_weaponData.m_projectileSpeed; 
        }
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Lifetime timer for return
        m_lifeTimer += Time.fixedDeltaTime;

        transform.position += m_projectileSpeed * transform.forward * Time.fixedDeltaTime; // Move projectile
        
        if (m_lifeDuration < m_lifeTimer) // If projectile is moving away from player
        {
            Destruct();
        }
    }
    public void SetProjectileData(Player_Attack _user, WeaponData _data)
    {
        m_projectileUser = _user;
        m_weaponData = _data;
    }

    private void Destruct()
    {
        foreach (var effect in m_effects)
        {
            effect.transform.SetParent(null);
            if (effect.GetComponent<VFXTimerScript>() != null)
                effect.GetComponent<VFXTimerScript>().m_startedTimer = true;
        }

        Destroy(gameObject);
    }

    private void OnTriggerEnter(Collider other)
    {
        //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            Debug.Log("Hit " + other.name + " with " + m_weaponData.weaponType + " for " + m_weaponData.m_damage);

            m_projectileUser.DamageTarget(other.gameObject, m_weaponData.m_damage, m_weaponData.m_knockback);

            m_projectileUser.playerController.playerAudioAgent.PlayWeaponHit(m_weaponData.weaponType); // Audio

            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null)
            {
                actor.KnockbackActor((actor.transform.position - transform.position).normalized * m_weaponData.m_knockback);
            }

            m_projectileUser.CreateVFX(other, transform.position);

            if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
                Destruct();
        }
    }
}
