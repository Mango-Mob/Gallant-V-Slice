using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DevRoomTrigger : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            foreach (var actor in ActorManager.Instance.m_subscribed)
            {
                actor.SetTarget(other.gameObject);
            }
        }
    }
}
