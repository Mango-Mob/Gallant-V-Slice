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
public class Player_Attack : MonoBehaviour
{
    [Header("Hand Transforms")]
    public Transform m_leftHandTransform;
    public Transform m_rightHandTransform;

    private WeaponData m_leftWeapon;
    private WeaponData m_rightWeapon;

    private GameObject m_leftWeaponObject;
    private GameObject m_rightWeaponObject;

    private void Start()
    {
        
    }

    private void Update()
    {
        
    }

    public void SwingWeapon(Hand _hand)
    {

    }

    public void PickUpWeapon(DroppedWeapon _weapon, Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                // Drop old weapon
                if (m_leftWeapon != null)
                    DroppedWeapon.CreateDroppedWeapon(transform.position, m_leftWeapon);

                // Delete old weapon from player
                Destroy(m_leftWeaponObject);

                // Set new weapon
                m_leftWeapon = _weapon.m_weaponData;
                m_leftWeaponObject = Instantiate(m_leftWeapon.weaponModelPrefab, m_leftHandTransform);
                break;
            case Hand.RIGHT:
                // Drop old weapon
                if (m_rightWeapon != null)
                    DroppedWeapon.CreateDroppedWeapon(transform.position, m_rightWeapon);

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
}
