using ActorSystem.AI;
using ActorSystem.AI.Users;
using ActorSystem.Spawning;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class TutorialManager : SingletonPersistent<TutorialManager>
{
    public static bool isNewPlayer = false;
    public Actor m_guide;
    public ActorSpawner m_spawner;

    public LevelData m_tutorialLevel;
    public SceneData[] m_sceneData;

    public int tutorialPosition = 0;
    public int targetDialog = 0;
    public TextAsset m_playerDeathDialog;

    public ClassData m_warrior;
    public ClassData m_mage;
    public ClassData m_hunter;

    public Transform m_respawn;

    public GameObject[] m_mainGameObject;
    private Image m_fade;

    private bool m_isRespawning = false;
    private bool m_playerHasDied = false;

    protected override void Awake()
    {
        base.Awake();
    }

    public void OnLevelWasLoaded(int level)
    {
        if(SceneManager.GetSceneByBuildIndex(level).name != "Tutorial" && TutorialManager.Instance != null)
        {
            Destroy(TutorialManager.Instance.gameObject);
        }
    }

    // Start is called before the first frame update
    public void Start()
    {
        m_fade = GetComponentInChildren<Image>();
        m_fade.enabled = false;

        (int, int)[] conData = new (int, int)[m_sceneData.Length - 1];
        for (int i = 0; i < m_sceneData.Length - 1; i++)
        {
            conData[i] = (i, i + 1);
        }
        NavigationManager.Instance.Generate(m_tutorialLevel);
        NavigationManager.Instance.UpdateMap(0);
        NavigationManager.Instance.ConstructScene();
    }

    // Update is called once per frame
    public void Update()
    {
        if(GameManager.Instance.m_player.GetComponent<Player_Controller>().playerResources.m_dead && !m_isRespawning)
        {
            StartCoroutine(RespawnPlayer());
        }

        if(NavigationManager.Instance.index == 3 && tutorialPosition == 3)
        {
            tutorialPosition = 4;
            targetDialog = 0;
        }
    }
    public void InteractFunction()
    {
        if (tutorialPosition == 3)
            RewardManager.Instance.Show(m_warrior.startWeapon, m_mage.startWeapon, m_hunter.startWeapon, SelectClass);
    }

    public bool AdvanceTutorial()
    {
        if (tutorialPosition < 3 || tutorialPosition == 4)
        {
            tutorialPosition++;
            return true;
        }
        
        if(tutorialPosition == 3)
        {
            //RewardManager.Instance.Show(m_warrior.startWeapon, m_mage.startWeapon, m_hunter.startWeapon, SelectClass);
            return false;
        }
        
        return false;
    }

    public void SelectClass(int selected)
    {
        targetDialog = selected;
        switch (selected)
        {
            default:
            case 0:
                GameManager.Instance.m_player.GetComponent<Player_Controller>().SelectClass(m_warrior);
                break;
            case 1:
                GameManager.Instance.m_player.GetComponent<Player_Controller>().SelectClass(m_mage);
                break;
            case 2:
                GameManager.Instance.m_player.GetComponent<Player_Controller>().SelectClass(m_hunter);
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
        player.GetComponent<Player_Controller>().RespawnPlayerTo(Vector3.zero, true);
        m_fade.enabled = false;
        NavigationManager.Instance.UpdateMap(1);
        LevelManager.Instance.LoadNewLevel("Tutorial");
        tutorialPosition = 3;
        targetDialog = 3;
        yield return null;
    }

    private void OnDrawGizmos()
    {
        //Gizmos.color = Color.green;
        //Gizmos.DrawSphere(transform.position, 0.5f);
        //Gizmos.DrawLine(transform.position, transform.position + transform.forward);
        //
        //for (int i = 0; i < m_tutorialPositions.Length; i++)
        //{
        //    Gizmos.color = Color.yellow;
        //    Gizmos.DrawSphere(m_tutorialPositions[i].position, 0.5f);
        //    Gizmos.DrawLine(m_tutorialPositions[i].position, m_tutorialPositions[i].position + m_tutorialPositions[i].forward);
        //}
        //Gizmos.color = Color.red;
        //Gizmos.DrawSphere(m_respawn.position, 0.5f);
    }
}
