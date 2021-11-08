using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * Player_Pickup: Allows the player to pickup dropped weapons
 * @author : William de Beer
 * @file : Player_Pickup.cs
 * @year : 2021
 */
public class Player_Pickup : MonoBehaviour
{
    private Player_Controller playerController;
    // List of dropped weapons in range of player
    private List<DroppedWeapon> weaponsInRange = new List<DroppedWeapon>();

    private void Start()
    {
        playerController = GetComponentInParent<Player_Controller>();
    }

    /*******************
     * FunctionName : Gets the closest weapon to the player inside the trigger box
     * @author : William de Beer
     * @return : (DroppedWeapon) Closest weapon to player.
     */
    public DroppedWeapon GetClosestWeapon() 
    {
        if (weaponsInRange.Count < 1) // Check if there are no weapons in range
        {
            Debug.Log("No weapon close enough to be picked up");
            return null;
        }
        // Set first weapon index as the default
        DroppedWeapon closestWeapon = weaponsInRange[0];
        float closestDistance = 100.0f;
        foreach (var weapon in weaponsInRange) 
        {
            float distance = Vector3.Distance(weapon.gameObject.transform.position, closestWeapon.transform.position);
            if (distance < closestDistance) // Compare current closest to current in list
            {
                closestDistance = distance;
                closestWeapon = weapon;
            }
        }
        return closestWeapon; // Return weapon 
    }

    public void RemoveDropFromList(DroppedWeapon _weapon)
    {
        weaponsInRange.Remove(_weapon); // Remove the weapon that is to be picked up
    }

    private void OnTriggerEnter(Collider other)
    {
        DroppedWeapon weapon = other.GetComponent<DroppedWeapon>();
        if (weapon != null)
        {
            // Toggle on weapon information panel
            weapon.ToggleDisplay(true);
            weapon.m_pickupDisplay.InitDisplayValues(playerController.playerAttack.m_rightWeapon, Hand.RIGHT);

            // Add to list
            weaponsInRange.Add(weapon);
        }
    }
    private void OnTriggerExit(Collider other)
    {
        DroppedWeapon weapon = other.GetComponent<DroppedWeapon>();
        if (weapon != null)
        {
            // Toggle off weapon information panel
            weapon.ToggleDisplay(false);

            // Remove from list
            weaponsInRange.Remove(weapon);
        }
    }
}
