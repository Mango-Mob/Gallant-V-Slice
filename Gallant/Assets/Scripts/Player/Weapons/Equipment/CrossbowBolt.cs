using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * CrossbowBolt: A crossbow projectile which can be launched by the player.
 * @author : William de Beer
 * @file : CrossbowBolt.cs
 * @year : 2021
 */
public class CrossbowBolt : BasePlayerProjectile
{
    private float m_lifeDuration = 1.0f;
    public bool m_destructOnHit = false;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

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
    protected override void EnvironmentCollision(Collider _other)
    {
        Destruct();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other.gameObject))
            return;
        //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)) || (m_canCollideWithEnvironment && other.gameObject.layer == LayerMask.NameToLayer("Environment")))
        {
            ProjectileCollide(other);
            if (other.gameObject.layer == LayerMask.NameToLayer("Attackable") && m_destructOnHit)
                Destruct();
        }
    }
}
