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
    private float m_rotateSpeed = 1000.0f;

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
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

            m_projectileSpeed = m_projectileSpeed * m_weaponData.m_speed * m_projectileUser.playerController.playerStats.m_attackSpeed;
            m_rotateSpeed = 100.0f * m_projectileSpeed;
            m_throwDuration = 10.0f / (m_projectileSpeed);
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        // Rotate model as it moves
        m_modelTransform.Rotate(new Vector3(0, (m_hand == Hand.LEFT ? 1.0f : -1.0f) * m_rotateSpeed * Time.fixedDeltaTime, 0));

        ProjectileReturnUpdate();
    }
    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other.gameObject))
            return;
        //if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            ProjectileCollide(other);
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
