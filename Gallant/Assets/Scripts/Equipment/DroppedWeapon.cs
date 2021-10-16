using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
 * DroppedWeapon by William de Beer
 * File: DroppedWeapon.cs
 * Description:
 *		A dropped weapon which contains information of a weapon and can be picked up by the player.
 */
public class DroppedWeapon : MonoBehaviour
{
    public WeaponData m_weaponData; // Data of contained weapon
    public static GameObject CreateDroppedWeapon(Vector3 _position, WeaponData _data) 
    {
        GameObject prefab = Resources.Load<GameObject>("BaseWeaponDrop"); // Get prefab from resources.
        GameObject droppedWeapon = Instantiate(prefab, _position, Quaternion.Euler(0, 0, 0)); // Instantiate object at given position

        if (droppedWeapon.GetComponent<DroppedWeapon>())
        {
            // Set weapon data
            droppedWeapon.GetComponent<DroppedWeapon>().m_weaponData = _data;
        }
        else
        {
            Debug.Log("No dropped weapon component found on dropped object. Wrong prefab/resource being loaded.");
        }
        return droppedWeapon;
    }
}
