using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Debug_ActorField : MonoBehaviour
{
    public string m_actorName;
    [SerializeField] private Text m_nameLoc;
    [SerializeField] private Text m_count;

    // Update is called once per frame
    void Update()
    {
        int count = 0;
        foreach (var actor in ActorManager.Instance.m_subscribed)
        {
            if (actor.m_name == m_actorName)
            {
                count++;
            }
        }
        m_nameLoc.text = m_actorName;
        m_count.text = $"({count.ToString()})";
    }
}
