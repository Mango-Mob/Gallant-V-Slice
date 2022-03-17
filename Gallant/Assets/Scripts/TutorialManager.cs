using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Actor m_guide;

    public Transform[] m_tutorialPositions;

    private int current = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.m_firstTime)
        {
            GameManager.Instance.m_player.transform.position = transform.position;
            GameManager.Instance.m_player.transform.forward = transform.forward;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(m_guide.transform.position, m_tutorialPositions[current].position) < 1.5f)
        {
            m_guide.SetTargetOrientaion(m_tutorialPositions[current].position + m_tutorialPositions[current].forward);
        }
    }

    public void TutorialOne()
    {
        current++;
        m_guide.SetTargetLocation(m_tutorialPositions[current].position, true);
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.5f);
        Gizmos.DrawLine(transform.position, transform.position + transform.forward);

        for (int i = 0; i < m_tutorialPositions.Length; i++)
        {
            Gizmos.color = Color.yellow;
            Gizmos.DrawSphere(m_tutorialPositions[i].position, 0.5f);
            Gizmos.DrawLine(m_tutorialPositions[i].position, m_tutorialPositions[i].position + m_tutorialPositions[i].forward);
        }
        
    }
}
