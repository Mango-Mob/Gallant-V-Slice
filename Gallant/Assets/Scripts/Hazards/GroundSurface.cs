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

    public struct SurfaceInfo
    {
        public SurfaceType surfaceType;
        public float effectiveness;
    }

    public SurfaceInfo m_surfaceInfo;

    public SurfaceType m_surfaceType;
    public float m_effectiveness = 0.5f;

    private void Start()
    {
        m_surfaceInfo.surfaceType = m_surfaceType;
        m_surfaceInfo.effectiveness = m_effectiveness;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Movement>())
        {
            m_inPlayerVector = true;
            playerMovement = other.GetComponent<Player_Movement>();
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Add(m_surfaceInfo);
        }

        /*
         * ACTOR
         * VVVVV 
         */
        if (other.GetComponent<Actor>() && m_surfaceType == SurfaceType.BOG)
        {
            other.GetComponentInChildren<StatusEffectContainer>().AddStatusEffect(new SlowStatus(0.5f, 0.1f));
        }
    }

    private void OnTriggerStay(Collider other)
    {
        // PLAYER
        if (!m_inPlayerVector && other.GetComponent<Player_Movement>())
        {
            m_inPlayerVector = true;
            playerMovement = other.GetComponent<Player_Movement>();
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Add(m_surfaceInfo);
        }

        /*
         * ACTOR
         * VVVVV 
         */
        if (other.GetComponent<Actor>() && m_surfaceType == SurfaceType.BOG)
        {
            other.GetComponentInChildren<StatusEffectContainer>().AddStatusEffect(new SlowStatus(0.5f, 0.1f));
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // PLAYER
        if (other.GetComponent<Player_Movement>())
        {
            m_inPlayerVector = false;
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Remove(m_surfaceInfo);
        }
    }

    private void OnDestroy()
    {
        if (m_inPlayerVector && playerMovement)
        {
            m_inPlayerVector = false;
            playerMovement.m_touchedSurfaces.Remove(m_surfaceInfo);
        }
    }

    private void OnDisable()
    {
        if (m_inPlayerVector && playerMovement)
        {
            m_inPlayerVector = false;
            playerMovement.m_touchedSurfaces.Remove(m_surfaceInfo);
        }
    }
}
