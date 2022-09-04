using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class DaggerPull : BasePlayerProjectile
{
    private Collider m_hitTarget;
    private Vector3 m_relativeHitPosition;
    private bool m_grappledTarget = false;

    [SerializeField] private LineRenderer m_lineRenderer;
    private bool m_canCatchOverride = false;
    private float m_storedProjectileSpeed;
    [SerializeField] private AnimationCurve m_speedCurve = new AnimationCurve();

    // Start is called before the first frame update
    void Awake()
    {

    }
    new private void Start()
    {
        base.Start();
        ApplyWeaponModel();

        m_weaponModel.transform.localPosition += Vector3.forward * 0.5f;
        m_weaponModel.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);

        m_projectileSpeed = m_projectileSpeed * m_weaponData.m_speed * m_projectileUser.playerController.playerStats.m_attackSpeed;
        m_throwDuration = 10.0f / (m_projectileSpeed);
        m_storedProjectileSpeed = m_projectileSpeed;
    }

    private void FixedUpdate()
    {
        m_lineRenderer.SetPosition(0, m_handTransform.position);
        m_lineRenderer.SetPosition(1, transform.position);

        if (m_throwDuration > m_lifeTimer && hitList.Count < 1)
            m_projectileSpeed = m_storedProjectileSpeed * m_speedCurve.Evaluate(m_lifeTimer / m_throwDuration);
        else if (m_throwDuration * 2.0f > m_lifeTimer && hitList.Count < 1)
            m_projectileSpeed = m_storedProjectileSpeed * m_speedCurve.Evaluate(1 + ((m_throwDuration - m_lifeTimer) / m_throwDuration));
        else
            m_projectileSpeed = m_storedProjectileSpeed;

        if (m_canCatchOverride && !m_returning)
        {
            CatchProjectile(1.0f);
        }

        if (m_grappledTarget)
        {
            transform.position = m_hitTarget.transform.position + m_relativeHitPosition;
            if (!m_projectileUser.playerController.playerMovement.IsDashing())
            {
                m_grappledTarget = false;
                m_returning = true;
            }
        }
        else if (m_returning)
        {
            ProjectileReturnUpdate();
        }
        else
        {
            // Lifetime timer for return
            m_lifeTimer += Time.fixedDeltaTime;

            transform.position += m_projectileSpeed * transform.forward * Time.fixedDeltaTime; // Move projectile
            
            if (m_throwDuration < m_lifeTimer)
            {
                m_returning = true;
            }
        }



        //// Get direction towards target
        //Vector3 direction = m_projectileUser.transform.position - transform.position;
        //direction.y = 0;
        //transform.forward = direction;
    }
    protected override void EnvironmentCollision(Collider _other)
    {
        SetTarget(_other);
        m_grappledTarget = true;
    }
    private void EnemyCollision(Collider _other)
    {
        m_projectileUser.DamageTarget(_other.gameObject, m_weaponData.m_damage * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f), m_weaponData.m_impact * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altImpactMult : 1.0f), 0, CombatSystem.DamageType.Physical, m_weaponData.abilityData != null ? m_weaponData.abilityData.m_tags : null);

        if (_other.GetComponent<Actor>())
        {
            SetTarget(_other);
            m_grappledTarget = true;
        }
    }
    private void SetTarget(Collider _other)
    {
        m_hitTarget = _other;
        m_relativeHitPosition = transform.position - _other.transform.position;

        m_projectileUser.StopUseWeapon(true);
    }
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Hit");

        if (hitList.Contains(other.gameObject) || m_returning || m_grappledTarget)
            return;

        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            EnemyCollision(other);
        }
        if (m_canCollideWithEnvironment && other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            EnvironmentCollision(other);
        }
    }
}
