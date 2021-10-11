using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    SWORD,
    SHIELD,
    BOOMERANG,
}
public enum Hand
{
    LEFT,
    RIGHT,
}
/*
 * Player_Attack by William de Beer
 * File: Player_Attack
 * Description:
 *		Contains logic for different player weapon attacks
 */
public class Player_Attack : MonoBehaviour
{
    private Player_Controller playerController;
    public float m_swingHeight = 1.0f;

    [Header("Hand Transforms")]
    public Transform m_leftHandTransform;
    public WeaponData m_leftWeapon;
    private GameObject m_leftWeaponObject;
    private bool m_leftWeaponInUse = false;

    public Transform m_rightHandTransform;
    public WeaponData m_rightWeapon;
    private GameObject m_rightWeaponObject;
    private bool m_rightWeaponInUse = false;

    private GameObject m_boomerangeProjectilePrefab;

    private void Start()
    {
        playerController = GetComponent<Player_Controller>();
        m_boomerangeProjectilePrefab = Resources.Load<GameObject>("BoomerangProjectile");
    }

    public void UseWeapon(Hand _hand)
    {
        WeaponData thisData;
        GameObject thisObject;
        Vector3 thisHandPosition;

        switch (_hand)
        {
            case Hand.LEFT: // Left hand weapon
                if (m_leftWeaponInUse)
                    return;
                // Set weapon information
                thisData = m_leftWeapon;
                thisObject = m_leftWeaponObject;
                thisHandPosition = m_leftHandTransform.position;

                break;
            case Hand.RIGHT: // Right hand weapon
                if (m_rightWeaponInUse)
                    return;
                // Set weapon information
                thisData = m_rightWeapon;
                thisObject = m_rightWeaponObject;
                thisHandPosition = m_rightHandTransform.position;

                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                return;
        }

        // If weapon is not in hand
        if (thisData == null)
            return;

        switch (thisData.weaponType)
        {
            case Weapon.SWORD: // Use sword
                WeaponAttack(thisData);
                break;
            case Weapon.SHIELD: // Use shield
                WeaponAttack(thisData);
                break;
            case Weapon.BOOMERANG: // Use boomerang
                ThrowBoomerang(thisHandPosition, thisData, _hand);
                break;
            default:
                Debug.Log("Weapon not implemented:" + thisData.weaponType);
                break;
        }
    }
    public void PickUpWeapon(DroppedWeapon _weapon, Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                // Drop old weapon
                if (m_leftWeapon != null)
                    DroppedWeapon.CreateDroppedWeapon(_weapon.transform.position, m_leftWeapon);

                // Delete old weapon from player
                Destroy(m_leftWeaponObject);

                // Set new weapon
                m_leftWeapon = _weapon.m_weaponData;
                m_leftWeaponObject = Instantiate(m_leftWeapon.weaponModelPrefab, m_leftHandTransform);
                break;
            case Hand.RIGHT:
                // Drop old weapon
                if (m_rightWeapon != null)
                    DroppedWeapon.CreateDroppedWeapon(_weapon.transform.position, m_rightWeapon);

                // Delete old weapon from player
                Destroy(m_rightWeaponObject);

                // Set new weapon
                m_rightWeapon = _weapon.m_weaponData;
                m_rightWeaponObject = Instantiate(m_rightWeapon.weaponModelPrefab, m_rightHandTransform);
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                return;
        }

        Destroy(_weapon.gameObject);
    }

    #region Sword
    private void WeaponAttack(WeaponData _data)
    {
        //switch (_hand)
        //{
        //    case Hand.LEFT:
        //        break;
        //    case Hand.RIGHT:
        //        break;
        //    default:
        //        Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
        //        return;
        //}

        Collider[] colliders = Physics.OverlapSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * _data.hitCenterOffset, _data.hitSize/*,Enemy Layer*/);
        foreach (var collider in colliders)
        {
            Debug.Log("Hit " + collider.name + " with " + _data.weaponType + " for " + _data.m_damage);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerController != null)
        {
            if (m_rightWeapon != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_rightWeapon.hitCenterOffset, m_rightWeapon.hitSize);
            }
            if (m_leftWeapon != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_leftWeapon.hitCenterOffset, m_leftWeapon.hitSize);
            }
        }
    }

    #endregion

    #region Shield

    #endregion

    #region Boomerang
    private void ThrowBoomerang(Vector3 _pos, WeaponData _data, Hand _hand)
    {
        // Create projectile
        GameObject projectile = Instantiate(m_boomerangeProjectilePrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        projectile.GetComponent<BoomerangProjectile>().SetReturnInfo(this, _data, _hand); // Set the information of the user to return to

        // Set activation booleans
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftWeaponObject.SetActive(false);
                m_leftWeaponInUse = true;
                break;
            case Hand.RIGHT:
                m_rightWeaponObject.SetActive(false);
                m_rightWeaponInUse = true;
                break;
            default:
                break;
        }
    }
    public void CatchBoomerang(Hand _hand)
    {
        // Set activation booleans
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftWeaponInUse = false;
                m_leftWeaponObject.SetActive(true);
                break;
            case Hand.RIGHT:
                m_rightWeaponInUse = false;
                m_rightWeaponObject.SetActive(true);
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                break;
        }
    }
    #endregion
}
