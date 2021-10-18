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
    public GameObject m_defaultModel;
    public GameObject m_weaponModel;

    private void Start()
    {
        if (m_weaponData != null)
        {
            m_defaultModel.GetComponent<MeshRenderer>().enabled = false;
            m_weaponModel = Instantiate(m_weaponData.weaponModelPrefab, m_defaultModel.transform);
            
            if (m_weaponModel.transform.GetChild(0) != null)
            {
                m_weaponModel.transform.GetChild(0).localPosition = Vector3.zero;
            }
            m_weaponModel.transform.GetChild(0).rotation = Quaternion.Euler(-75, 0, 0);
        }
    }
    private void FixedUpdate()
    {
        m_defaultModel.transform.Rotate(new Vector3(0, 30, 0) * Time.fixedDeltaTime);
        m_defaultModel.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Sin(Time.timeSinceLevelLoad * 1.0f) * 30.0f);
    }
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
