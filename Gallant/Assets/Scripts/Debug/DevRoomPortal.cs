using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevRoomPortal : MonoBehaviour
{
    public Transform playerSendTo;

    // Start is called before the first frame update
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            other.GetComponent<Player_Controller>().RespawnPlayerTo(playerSendTo.position, true);
            foreach (var actor in ActorManager.Instance.m_subscribed)
            {
                actor.SetTarget(null);
            }
        }
    }
    
}
