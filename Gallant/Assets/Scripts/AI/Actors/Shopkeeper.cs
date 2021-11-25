using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Shopkeeper : Actor
{
    public TextAsset m_dialog;
    public GameObject m_keyboardInput;
    public GameObject m_gamePadInput;

    public RewardWindow m_reward;
    public DialogDisplay m_display;

    private Player_Controller m_player;
    private bool m_ShowUI = false;
    private bool m_hasGivenReward = false;

    protected override void Awake()
    {
        base.Awake();
        m_player = FindObjectOfType<Player_Controller>();
    }

    // Start is called before the first frame update
    protected override void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    protected override void Update()
    {
        base.Update();
        m_keyboardInput.SetActive(m_ShowUI && !InputManager.instance.isInGamepadMode);
        m_gamePadInput.SetActive(m_ShowUI && InputManager.instance.isInGamepadMode);

        if(m_player != null && Vector3.Distance(m_player.transform.position, transform.position) > 30.0f)
        {
            gameObject.SetActive(false);
        }
    }

    public void TalkTo()
    {
        m_display.LoadDialog(m_dialog);
        if (!m_hasGivenReward)
        {
            m_display.m_interact = new UnityEngine.Events.UnityEvent();
            m_display.m_interact.AddListener(Reward);
        }
        GetComponentInChildren<Interactable>().m_isReady = false;
        m_display.Show();
        
    }
    public void Reward()
    {
        m_reward.Show(Mathf.FloorToInt(GameManager.currentLevel));
        m_display.m_interact = null;
        m_hasGivenReward = true;
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_ShowUI = true;
            GetComponentInChildren<Interactable>().m_isReady = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_ShowUI = false;
            GetComponentInChildren<Interactable>().m_isReady = false;
        }
    }
}
