using ActorSystem.AI;
using ActorSystem.AI.Users;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialManager : MonoBehaviour
{
    public Actor m_guide;

    public Transform[] m_tutorialPositions;
    public TextAsset[] m_tutorialDialog;

    public TextAsset[] m_classReSpecDialog;

    public ClassData m_warrior;
    public ClassData m_mage;
    public ClassData m_hunter;

    public Room m_combatSection;
    private int current = 0;
    // Start is called before the first frame update
    void Start()
    {
        if (GameManager.m_firstTime)
        {
            GameManager.Instance.m_player.transform.position = transform.position;
            GameManager.Instance.m_player.transform.forward = transform.forward;
            (m_guide as LoreKeeper).m_dialog = m_tutorialDialog[current];
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Vector3.Distance(m_guide.transform.position, m_tutorialPositions[current].position) < 0.5f)
        {
            m_guide.SetTargetOrientaion(m_tutorialPositions[current].position + m_tutorialPositions[current].forward);
            m_guide.m_myBrain.m_myOutline.enabled = true;
        }
        else
        {
            m_guide.SetTargetOrientaion(m_guide.transform.position + m_guide.m_myBrain.m_legs.velocity.normalized);
        }

        if(m_combatSection.IsComplete() && current == 3)
        {
            current++;
            m_guide.SetTargetLocation(m_tutorialPositions[current].position);
            m_guide.m_myBrain.m_myOutline.enabled = false;
            (m_guide as LoreKeeper).m_dialog = m_tutorialDialog[current - 1];
        }
    }

    public void AdvanceTutorial()
    {
        if (current < 3)
        {
            current++;
            m_guide.SetTargetLocation(m_tutorialPositions[current].position);
            m_guide.m_myBrain.m_myOutline.enabled = false;
            (m_guide as LoreKeeper).m_dialog = m_tutorialDialog[current];
        }

        if(current == 3)
        {
            RewardManager.Instance.Show(m_warrior.rightWeapon, m_mage.rightWeapon, m_hunter.rightWeapon, SelectClass);
        }
    }

    public void SelectClass(int selected)
    {
        switch(selected)
        {
            default:
            case 0:
                GameManager.Instance.m_player.GetComponent<Player_Controller>().SelectClass(m_warrior);
                (m_guide as LoreKeeper).m_dialog = m_classReSpecDialog[selected];
                break;
            case 1:
                GameManager.Instance.m_player.GetComponent<Player_Controller>().SelectClass(m_mage);
                (m_guide as LoreKeeper).m_dialog = m_classReSpecDialog[selected];
                break;
            case 2:
                GameManager.Instance.m_player.GetComponent<Player_Controller>().SelectClass(m_hunter);
                (m_guide as LoreKeeper).m_dialog = m_classReSpecDialog[selected];
                break;
        }
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
