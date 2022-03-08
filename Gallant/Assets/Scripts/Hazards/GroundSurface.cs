using ActorSystem.AI;
using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSurface : MonoBehaviour
{
     public enum SurfaceType
    {
        ICE,
        BOG,
        LAVA,
        SPEED,
        JUMP,
    }

    public SurfaceType m_surfaceType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Movement>())
        {
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Add(m_surfaceType);
        }
        if(other.GetComponent<Actor>() && m_surfaceType == SurfaceType.BOG)
        {
            other.GetComponentInChildren<StatusEffectContainer>().AddStatusEffect(new SlowStatus(0.5f, 0.1f));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.GetComponent<Actor>() && m_surfaceType == SurfaceType.BOG)
        {
            other.GetComponentInChildren<StatusEffectContainer>().AddStatusEffect(new SlowStatus(0.5f, 0.1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player_Movement>())
        {
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Remove(m_surfaceType);
        }
    }
}
