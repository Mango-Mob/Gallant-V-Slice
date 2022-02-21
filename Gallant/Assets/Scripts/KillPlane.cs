using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillPlane : MonoBehaviour
{
    public bool canRespawnColliders = false;
    public float damageOnHit = 0.0f;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            //Enemy
            if(canRespawnColliders)
            {

            }
            else
            {
                other.GetComponent<Actor>().Kill();
            }
        }
        else if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            //Player
            if (canRespawnColliders)
            {

            }
            else
            {
                other.GetComponent<Player_Controller>().DamagePlayer(999999999, null, true);
            }
        }
    }
}
