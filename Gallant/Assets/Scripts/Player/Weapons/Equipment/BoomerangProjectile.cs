using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * BoomerangProjectile: A boomerang projectile which can be launched by the player if they are holding a boomerang.
 * @author : William de Beer
 * @file : BoomerangProjectile.cs
 * @year : 2021
 */
public class BoomerangProjectile : BasePlayerProjectile
{
    private float m_rotateSpeed = 500.0f;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

        ApplyWeaponModel();

        m_projectileSpeed = m_projectileSpeed * m_weaponData.m_speed * m_projectileUser.playerController.playerStats.m_attackSpeed;
        m_rotateSpeed = 100.0f * m_projectileSpeed;
        m_throwDuration = 10.0f / (m_projectileSpeed);
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Rotate model as it moves
        m_modelTransform.Rotate(new Vector3(0, (m_hand == Hand.LEFT ? 1.0f : -1.0f) * m_rotateSpeed * Time.fixedDeltaTime, 0));

        ProjectileReturnUpdate();
    }
    protected override void EnvironmentCollision(Collider _other)
    {
        m_returning = true;
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
        }
    }
    private void OnDrawGizmosSelected()
    {
        if (m_weaponData != null)
        {
            Gizmos.color = Color.magenta;
            Gizmos.DrawWireSphere(transform.position, m_weaponData.hitSize);
        }
    }
}
