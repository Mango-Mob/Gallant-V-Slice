using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGround : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<Player_Movement>())
        {
            other.GetComponent<Player_Movement>().m_onIce = true;
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.GetComponent<Player_Movement>())
        {
            other.GetComponent<Player_Movement>().m_onIce = false;
        }
    }
}
