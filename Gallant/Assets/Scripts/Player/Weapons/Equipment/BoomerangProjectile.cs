using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

/****************
 * BoomerangProjectile: A boomerang projectile which can be launched by the player if they are holding a boomerang.
 * @author : William de Beer
 * @file : BoomerangProjectile.cs
 * @year : 2021
 */
public class BoomerangProjectile : BasePlayerProjectile
{
    [SerializeField] private float m_rotateSpeed = 50.0f;
    private float m_storedProjectileSpeed;
    [SerializeField] private AnimationCurve m_boomerangSpeedCurve = new AnimationCurve();

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();

        ApplyWeaponModel();

        m_projectileSpeed = m_projectileSpeed * m_weaponData.m_speed * m_projectileUser.playerController.playerStats.m_attackSpeed;
        m_rotateSpeed = m_rotateSpeed * m_projectileSpeed;
        m_throwDuration = 10.0f / (m_projectileSpeed);
        m_storedProjectileSpeed = m_projectileSpeed;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (m_throwDuration > m_lifeTimer)
            m_projectileSpeed = m_storedProjectileSpeed * m_boomerangSpeedCurve.Evaluate(m_lifeTimer / m_throwDuration);
        else if (m_throwDuration * 2.0f > m_lifeTimer)
            m_projectileSpeed = m_storedProjectileSpeed * m_boomerangSpeedCurve.Evaluate(1 + ((m_throwDuration - m_lifeTimer) / m_throwDuration));
        else
            m_projectileSpeed = m_storedProjectileSpeed;

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
