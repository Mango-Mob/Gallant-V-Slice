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
    }

    public SurfaceType m_surfaceType;

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Movement>())
        {
            other.GetComponent<Player_Movement>().m_touchedSurfaces.Add(m_surfaceType);
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
