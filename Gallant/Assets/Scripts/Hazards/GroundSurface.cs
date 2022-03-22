using ActorSystem.AI;
using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundSurface : MonoBehaviour
{
    private Player_Movement playerMovement;
    private bool m_inPlayerVector = false;
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
            m_inPlayerVector = true;
            playerMovement = other.GetComponent<Player_Movement>();
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Add(m_surfaceType);
        }
        if(other.GetComponent<Actor>() && m_surfaceType == SurfaceType.BOG)
        {
            other.GetComponentInChildren<StatusEffectContainer>().AddStatusEffect(new SlowStatus(0.5f, 0.1f));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (!m_inPlayerVector)
        {
            m_inPlayerVector = true;
            playerMovement = other.GetComponent<Player_Movement>();
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Add(m_surfaceType);
        }

        if (other.GetComponent<Actor>() && m_surfaceType == SurfaceType.BOG)
        {
            other.GetComponentInChildren<StatusEffectContainer>().AddStatusEffect(new SlowStatus(0.5f, 0.1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player_Movement>())
        {
            m_inPlayerVector = false;
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Remove(m_surfaceType);
        }
    }

    private void OnDestroy()
    {
        if (m_inPlayerVector)
        {
            m_inPlayerVector = false;
            playerMovement.m_touchedSurfaces.Remove(m_surfaceType);
        }
    }

    private void OnDisable()
    {
        if (m_inPlayerVector)
        {
            m_inPlayerVector = false;
            playerMovement.m_touchedSurfaces.Remove(m_surfaceType);
        }
    }
}
