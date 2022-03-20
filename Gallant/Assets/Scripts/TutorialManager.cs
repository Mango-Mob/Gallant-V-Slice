using ActorSystem.AI;
using ActorSystem.AI.Users;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class TutorialManager : MonoBehaviour
{
    public Actor m_guide;

    public Transform[] m_tutorialPositions;
    public TextAsset[] m_tutorialDialog;

    public TextAsset[] m_classReSpecDialog;
    public TextAsset m_playerDeathDialog;

    public ClassData m_warrior;
    public ClassData m_mage;
    public ClassData m_hunter;

    public Room m_combatSection;

    public Transform m_respawn;

    public GameObject[] m_mainGameObject;
    private Image m_fade;

    private int current = 0;
    private bool m_isRespawning = false;
    private bool m_playerHasDied = false;
    // Start is called before the first frame update
    void Start()
    {
        m_fade = GetComponentInChildren<Image>();
        m_fade.enabled = false;
        if (GameManager.m_firstTime)
        {
            GameManager.Instance.m_player.transform.position = transform.position;
            GameManager.Instance.m_player.transform.forward = transform.forward;
            (m_guide as LoreKeeper).m_dialog = m_tutorialDialog[current];

            foreach (var item in m_mainGameObject)
            {
                item.SetActive(false);
            }
        }
        else
        {
            foreach (var item in m_mainGameObject)
            {
                item.SetActive(true);
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (m_guide == null)
            return;

        if(Vector3.Distance(m_guide.transform.position, m_tutorialPositions[current].position) < 0.5f)
        {
            m_guide.SetTargetOrientaion(m_tutorialPositions[current].position + m_tutorialPositions[current].forward);
            m_guide.m_myBrain.m_myOutline.enabled = true;
        }
        else
        {
            m_guide.SetTargetOrientaion(m_guide.transform.position + m_guide.m_myBrain.m_legs.velocity.normalized);
        }

        if(m_combatSection.IsComplete() && !GameManager.Instance.IsInCombat && current == 3)
        {
            current++;
            m_guide.SetTargetLocation(m_tutorialPositions[current].position);
            m_guide.m_myBrain.m_myOutline.enabled = false;
            (m_guide as LoreKeeper).m_dialog = m_tutorialDialog[current - 1];
        }

        if(GameManager.Instance.m_player.GetComponent<Player_Controller>().playerResources.m_dead && !m_isRespawning)
        {
            StartCoroutine(RespawnPlayer());
            m_playerHasDied = true;
        }

        if(m_playerHasDied)
        {
            (m_guide as LoreKeeper).m_dialog = m_playerDeathDialog;
            m_playerHasDied = false;
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

        if(current == 4)
        {
            current++;
            m_guide.SetTargetLocation(m_tutorialPositions[current].position);
            m_guide.m_myBrain.m_myOutline.enabled = false;
            (m_guide as LoreKeeper).m_dialog = m_tutorialDialog[current - 1];
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

    private IEnumerator RespawnPlayer()
    {
        m_isRespawning = true;
        float timeIn = 3.0f;
        float timeOut = 1.0f;
        m_fade.enabled = true;
        GameObject player = GameManager.Instance.m_player;
        while (timeIn > 0.0f)
        {
            timeIn -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            m_fade.color = new Color(0, 0, 0, 1.0f - timeIn / 3.0f);
        }       
        player.GetComponent<Player_Controller>().RespawnPlayerTo(m_respawn.position, true);
        player.GetComponent<Player_Controller>().m_isDisabledInput = true;
        while (timeOut > 0.0f)
        {
            timeOut -= Time.deltaTime;
            yield return new WaitForEndOfFrame();
            m_fade.color = new Color(0, 0, 0, timeOut / 1.0f);
        }
        m_fade.enabled = false;
        player.GetComponent<Player_Controller>().m_isDisabledInput = false;
        m_isRespawning = false;
        
        yield return null;
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
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(m_respawn.position, 0.5f);
    }
}
