using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * BoomerangProjectile: A boomerang projectile which can be launched by the player if they are holding a boomerang.
 * @author : William de Beer
 * @file : BoomerangProjectile.cs
 * @year : 2021
 */
public class BoomerangProjectile : MonoBehaviour
{
    // The model transform of the boomerang to animate it
    public Transform m_modelTransform;
    public GameObject m_weaponModel;
    public GameObject[] m_effects;

    private Player_Attack m_projectileUser; // The user of the projectile so the boomerang has a target to return to
    public WeaponData m_weaponData;
    private Hand m_hand; // Hand to return to
    private Transform m_handTransform;

    private float m_lifeTimer = 0.0f;
    private float m_throwDuration = 1.0f;
    private float m_boomerangSpeed = 10.0f;
    private float m_boomerangRotateSpeed = 1000.0f;

    // Start is called before the first frame update
    void Start()
    {
        // Set hand transform to be returned to
        m_handTransform = (m_hand == Hand.LEFT ? m_projectileUser.m_leftHandTransform : m_projectileUser.m_rightHandTransform);
        GetComponent<SphereCollider>().radius = m_weaponData.hitSize;

        if (m_weaponData != null)
        {
            m_weaponModel = Instantiate(m_weaponData.weaponModelPrefab, m_modelTransform);

            GameObject model = m_weaponModel.transform.GetChild(0).gameObject;
            if (model != null)
            {
                model.transform.parent = m_modelTransform;
                Destroy(m_weaponModel);
                m_weaponModel = model;

                m_weaponModel.transform.localPosition = Vector3.zero;
                m_weaponModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
            }

            m_boomerangSpeed = 10.0f * m_weaponData.m_speed * m_projectileUser.playerController.playerStats.m_attackSpeed;
            m_boomerangRotateSpeed = 100.0f * m_boomerangSpeed;
            m_throwDuration = 10.0f / (m_boomerangSpeed);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Lifetime timer for return
        m_lifeTimer += Time.fixedDeltaTime;

        // Rotate model as it moves
        m_modelTransform.Rotate(new Vector3(0, (m_hand == Hand.LEFT ? 1.0f : -1.0f) * m_boomerangRotateSpeed * Time.fixedDeltaTime, 0));

        if (m_throwDuration > m_lifeTimer) // If projectile is moving away from player
        {
            transform.position += m_boomerangSpeed * transform.forward * Time.fixedDeltaTime; // Move projectile
        }
        else // If projectile is moving back towards the player
        {
            // Get direction towards player
            Vector3 direction = ((m_handTransform.position) - transform.position);
            direction.y = 0;
            direction.Normalize();

            transform.position += m_boomerangSpeed * direction * Time.fixedDeltaTime; // Move projectile
            //transform.rotation = Quaternion.LookRotation(direction, transform.up); // Face projectile towards player
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), // Check distance between player and projectile
                new Vector3(m_handTransform.position.x, 0, m_handTransform.position.z)) < 0.2f) 
            {
                // "Catch" the projectile when close enough to player.
                m_projectileUser.CatchBoomerang(m_hand);

                foreach (var effect in m_effects)
                {
                    effect.transform.SetParent(null);
                    if (effect.GetComponent<VFXTimerScript>() != null)
                        effect.GetComponent<VFXTimerScript>().m_startedTimer = true;
                }

                Destroy(gameObject);
            }
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Debug.Log("Hit " + other.name + " with " + m_weaponData.weaponType + " for " + m_weaponData.m_damage);

            m_projectileUser.DamageTarget(other.gameObject, m_weaponData.m_damage);

            Actor actor = other.GetComponent<Actor>();
            if (actor != null)
            {
                actor.KnockbackActor((actor.transform.position - transform.position).normalized * m_weaponData.m_knockback);
            }
        }
    }

    /*******************
     * SetReturnInfo : Sets the information of the user who threw the boomerang and who it should be returned to.
     * @author : William de Beer
     * @param : (Player_Attack) The Player_Attack component of player, (WeaponData) The data of weapon, (Hand) The hand it originated from.
     * @return : (type) 
     */
    public void SetReturnInfo(Player_Attack _user, WeaponData _data, Hand _hand)
    {
        m_projectileUser = _user;
        m_weaponData = _data;
        m_hand = _hand;
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
