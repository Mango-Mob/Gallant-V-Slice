using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class KillPlane : MonoBehaviour
{
    public bool canRespawnColliders = false;
    public float damageOnHit = 0.0f;
    public LayerMask layersToLookFor;

    public void Start()
    {
        GetComponent<Renderer>().enabled = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if(layersToLookFor == (layersToLookFor | (1 << other.gameObject.layer)))
        {
            if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
            {
                //Enemy
                if (canRespawnColliders)
                {
                    other.GetComponent<Actor>().DealDamage(damageOnHit, CombatSystem.DamageType.True);
                    other.GetComponent<Actor>().Respawn(false);
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
                    other.GetComponent<Player_Controller>().RespawnPlayerToGround(false);
                    other.GetComponent<Player_Controller>().DamagePlayer(damageOnHit,CombatSystem.DamageType.True, null, true);
                }
                else
                {
                    other.GetComponent<Player_Controller>().DamagePlayer(999999999, CombatSystem.DamageType.True, null, true);
                }
            }
        }
    }
}
