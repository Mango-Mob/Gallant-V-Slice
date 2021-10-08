using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedWeapon : MonoBehaviour
{
    public WeaponData m_weaponData;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public static GameObject CreateDroppedWeapon(Vector3 _position, WeaponData _data)
    {
        GameObject prefab = Resources.Load<GameObject>("BaseWeaponDrop");
        GameObject droppedWeapon = Instantiate(prefab, _position, Quaternion.Euler(0, 0, 0));

        if (droppedWeapon.GetComponent<DroppedWeapon>())
        {
            droppedWeapon.GetComponent<DroppedWeapon>().m_weaponData = _data;
        }
        else
        {
            Debug.Log("No dropped weapon component found on dropped object. Wrong prefab/resource being loaded.");
        }
        return droppedWeapon;
    }
}
