﻿using System.Collections;
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
    // List of dropped weapons in range of player
    private List<DroppedWeapon> weaponsInRange = new List<DroppedWeapon>();

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
        weaponsInRange.Remove(closestWeapon); // Remove the weapon that is to be picked up
        return closestWeapon; // Return weapon 
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