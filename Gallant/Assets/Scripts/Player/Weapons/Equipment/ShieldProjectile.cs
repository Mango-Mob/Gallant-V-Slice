using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

/****************
 * ShieldProjectile: A boomerang projectile which can be launched by the player if they are holding a boomerang.
 * @author : William de Beer
 * @file : ShieldProjectile.cs
 * @year : 2021
 */
public class ShieldProjectile : BasePlayerProjectile
{
    public int m_hitsLeft = 3;
    public float m_bounceDetectRange = 8.0f;

    [SerializeField] private float m_rotateSpeed = 50.0f;
    private bool m_canCatchOverride = false;
    private float m_storedProjectileSpeed;
    [SerializeField] private AnimationCurve m_shieldSpeedCurve = new AnimationCurve();
    private float m_pursueTimer = 0.0f;
    [SerializeField] private float m_pursueDuration = 2.0f;

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
        if (m_throwDuration > m_lifeTimer && hitList.Count < 1)
            m_projectileSpeed = m_storedProjectileSpeed * m_shieldSpeedCurve.Evaluate(m_lifeTimer / m_throwDuration);
        else if (m_throwDuration * 2.0f > m_lifeTimer && hitList.Count < 1)
            m_projectileSpeed = m_storedProjectileSpeed * m_shieldSpeedCurve.Evaluate(1 + ((m_throwDuration - m_lifeTimer) / m_throwDuration));
        else
            m_projectileSpeed = m_storedProjectileSpeed;

        // Rotate model as it moves
        m_modelTransform.Rotate(new Vector3(0, (m_hand == Hand.LEFT ? 1.0f : -1.0f) * m_rotateSpeed * Time.fixedDeltaTime, 0));

        if (m_canCatchOverride && !m_returning)
        {
            CatchProjectile(1.0f);
        }

        if (hitList.Count < 1 || m_returning)
        {
            ProjectileReturnUpdate();

            if (m_returning)
            {
                // Get direction towards target
                Vector3 direction = m_projectileUser.transform.position - transform.position;
                direction.y = 0;
                transform.forward = direction;
            }
        }
        else
        {
            m_pursueTimer += Time.deltaTime;

            Actor closestTarget = null;
            float closestDistance = Mathf.Infinity;

            Collider[] colliders = Physics.OverlapSphere(transform.position, m_bounceDetectRange);
            foreach (var collider in colliders)
            {
                Actor actor = collider.GetComponentInParent<Actor>();
                if (actor == null || hitList.Contains(actor.gameObject) || actor.m_myBrain.IsDead || !actor.m_myBrain.m_canBeTarget)
                    continue;

                if (m_pursueTimer > m_pursueDuration)
                {
                    hitList.Add(actor.gameObject);
                    m_pursueTimer = 0.0f;
                    continue;
                }

                float distance = Vector3.Distance(actor.transform.position, transform.position);
                if (distance < closestDistance)
                {
                    closestTarget = actor;
                    closestDistance = distance;
                }
            }

            if (closestTarget == null)
            {
                m_returning = true;
                return;
            }

            // Get direction towards target
            Vector3 direction = closestTarget.transform.position - transform.position;
            direction.y = 0;
            direction.Normalize();

            transform.position += m_projectileSpeed * direction * Time.fixedDeltaTime; // Move projectile
            transform.forward = direction;
        }

    }
    protected override void EnvironmentCollision(Collider _other)
    {
        //m_returning = true;
        Vector3 wallNormal = _other.ClosestPoint(transform.position) - transform.position;
        wallNormal.y = 0.0f;

        transform.forward = Vector3.Reflect(transform.forward, wallNormal.normalized);
        m_canCatchOverride = true;
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other.gameObject))
            return;
        //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)) || (m_canCollideWithEnvironment && other.gameObject.layer == LayerMask.NameToLayer("Environment")))
        {
            int hitCount = hitList.Count;
            ProjectileCollide(other);
            if (hitCount != hitList.Count)
                m_hitsLeft -= 1;
        }

        if (m_hitsLeft <= 0)
        {
            m_returning = true;
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
