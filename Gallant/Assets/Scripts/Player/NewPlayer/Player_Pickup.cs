using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Pickup : MonoBehaviour
{
    private List<DroppedWeapon> weaponsInRange = new List<DroppedWeapon>();

    public DroppedWeapon GetClosestWeapon()
    {
        if (weaponsInRange.Count < 1)
        {
            Debug.Log("No weapon close enough to be picked up");
            return null;
        }
        DroppedWeapon closestWeapon = weaponsInRange[0];
        float closestDistance = 100.0f;
        foreach (var weapon in weaponsInRange)
        {
            float distance = Vector3.Distance(weapon.gameObject.transform.position, closestWeapon.transform.position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestWeapon = weapon;
            }
        }
        weaponsInRange.Remove(closestWeapon);
        return closestWeapon;
    }

    private void OnTriggerEnter(Collider other)
    {
        DroppedWeapon weapon = other.GetComponent<DroppedWeapon>();
        if (weapon != null)
        {
            // Create weapon information panel

            // Add to list
            weaponsInRange.Add(weapon);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        DroppedWeapon weapon = other.GetComponent<DroppedWeapon>();
        if (weapon != null)
        {
            // Destroy weapon information panel


            // Remove from list
            weaponsInRange.Remove(weapon);
        }
    }
}
