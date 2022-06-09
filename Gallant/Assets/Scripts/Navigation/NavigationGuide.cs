using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

public class NavigationGuide : MonoBehaviour
{
    public bool isReady { 
        get { 
            if(GetComponent<NavMeshAgent>().isOnNavMesh)
                return GetComponent<NavMeshAgent>().remainingDistance < 0.5f;
            return true;
        }
    }
    public float m_spawnDist = 1f;

    public void StartTrace(Vector3 targetPosition)
    {
        Vector3 start = GameManager.Instance.m_player.transform.position;
        Vector3 direct = (targetPosition - start).normalized;
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if(agent.Warp(start + direct * m_spawnDist))
        {
            agent.SetDestination(targetPosition);
        }
        GetComponentInChildren<Animator>().SetBool("IsVisible", true);
        GetComponentInChildren<TrailRenderer>().Clear();
    }

    public void SetColor(Color _newColor)
    {
        //Color endColor = new Color(_newColor.r, _newColor.g, _newColor.b, _newColor.a * 0.5f);
        //
        //GetComponentInChildren<Renderer>().material.color = _newColor;
        //GetComponentInChildren<TrailRenderer>().startColor = _newColor;
        //GetComponentInChildren<TrailRenderer>().endColor = endColor;
    }
}
