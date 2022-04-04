using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * WispProjectile: A wisp projectile which can be launched by the player.
 * @author : William de Beer
 * @file : CrossbowBolt.cs
 * @year : 2021
 */
public class WispProjectile : BasePlayerProjectile
{
    [SerializeField] private float m_targetbeamRange = 2.0f;
    [SerializeField] private float m_targetbeamHeight = 1.0f;
    private float m_lifeDuration = 1.0f;
    private bool m_activated = false;
    private Actor m_target;
    [SerializeField] private LineRenderer m_laserLine;

    private Vector3 m_movementVelocity = Vector3.zero;

    private Vector3 m_startScale;
    private float m_remainingDamage = 1.0f;
    private float m_baseDamage = 5.0f;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
        m_laserLine.enabled = false;

        if (m_weaponData != null)
        {
            m_projectileSpeed = m_projectileSpeed * m_weaponData.m_projectileSpeed; 
        }
        m_startScale = transform.localScale;
        m_remainingDamage = m_weaponData.m_damage * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f);

        m_movementVelocity = Random.onUnitSphere * 4.0f;
    }
    // Update is called once per frame
    void FixedUpdate()
    {
        // Lifetime timer for return
        m_lifeTimer += Time.fixedDeltaTime;

        m_laserLine.enabled = m_target != null && m_activated;
        transform.localScale = Vector3.Lerp(Vector3.zero, m_startScale, m_remainingDamage / m_weaponData.m_damage * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f));

        if (!m_activated)
        {
            transform.position += m_projectileSpeed * transform.forward * Time.fixedDeltaTime; // Move projectile
        }
        else
        {
            if (m_target != null)
            {
                m_laserLine.SetPosition(0, transform.position);
                m_laserLine.SetPosition(1, m_target.m_selfTargetTransform.transform.position);

                Vector3 targetPos = m_target.m_selfTargetTransform.transform.position;
                targetPos.y = 0.0f;
                Vector3 projectilePos = transform.position;
                projectilePos.y = 0.0f;

                Vector3 targetPosition = m_target.m_selfTargetTransform.transform.position + (projectilePos - targetPos).normalized * m_targetbeamRange;
                targetPosition.y += m_targetbeamRange;

                transform.position = Vector3.SmoothDamp(transform.position, targetPosition, ref m_movementVelocity, 0.1f);

                float damageTick = m_baseDamage * m_weaponData.m_speed * (m_hand == Hand.LEFT ? m_weaponData.m_altSpeedMult : 1.0f) * Time.fixedDeltaTime;
                m_target.DealDamageSilent(damageTick, CombatSystem.DamageType.Ability);
                m_remainingDamage -= damageTick;

                if (m_remainingDamage < 0.0f)
                {
                    m_activated = false;
                    Destruct();
                }
            }



            if (m_activated && (m_target == null || m_target.m_myBrain.IsDead))
            {
                Actor closestTarget = null;
                float closestDistance = Mathf.Infinity;

                Collider[] colliders = Physics.OverlapSphere(transform.position, m_targetbeamRange);
                foreach (var collider in colliders)
                {
                    Actor actor = collider.GetComponentInParent<Actor>();
                    if (actor == null || hitList.Contains(actor.gameObject) || actor.m_myBrain.IsDead)
                        continue;

                    float distance = Vector3.Distance(actor.transform.position, transform.position);
                    if (distance < closestDistance)
                    {
                        closestTarget = actor;
                        closestDistance = distance;
                    }
                }
                //m_weaponData.m_damage* m_charge *(m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f)
                if (closestTarget == null)
                {
                    Destruct();
                }
                m_target = closestTarget;
            }
        }

        if (!m_activated && m_lifeDuration < m_lifeTimer) // If projectile is moving away from player
        {
            Destruct();
        }
    }
    public void SetProjectileData(Player_Attack _user, WeaponData _data, float _charge, bool _canCharge = false)
    {
        m_projectileUser = _user;
        m_weaponData = _data;
        m_charge = _charge;
    }

    protected override void EnvironmentCollision()
    {
        // ¯\_(ツ)_/¯
    }
    private void OnTriggerEnter(Collider other)
    {
        if (m_activated)
            return;

        //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            bool enemyCheck = false;

            m_projectileUser.playerController.playerAudioAgent.PlayOrbPickup(); // Audio
            Actor actor = other.GetComponentInParent<Actor>();
            if (actor != null && !actor.m_myBrain.IsDead)
            {
                hitList.Add(other.gameObject);
                actor.KnockbackActor((actor.transform.position - transform.position).normalized * m_weaponData.m_knockback * m_charge);
                enemyCheck = true;
            }

            if (enemyCheck)
            {
                m_activated = true;
                if (hitList.Count > 0)
                    m_target = hitList[0].GetComponent<Actor>();
            }
        }
    }
}
