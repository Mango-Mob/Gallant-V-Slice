using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shopkeeper : Actor
{
    public TextAsset m_dialog;
    public GameObject m_keyboardInput;
    public Image m_gamePadInput;

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
        m_keyboardInput.gameObject.SetActive(m_ShowUI && !InputManager.instance.isInGamepadMode);
        m_gamePadInput.gameObject.SetActive(m_ShowUI && InputManager.instance.isInGamepadMode);

        if(m_player != null && Vector3.Distance(m_player.transform.position, transform.position) > 30.0f)
        {
            gameObject.SetActive(false);
        }

        if (m_ShowUI)
        {
            InputManager.Bind[] binds = InputManager.instance.GetBinds("Interact");
            bool foundKey = false;
            bool foundButton = false;
            for (int i = 0; i < binds.Length; i++)
            {
                switch (InputManager.Bind.GetTypeID(binds[i].enumType))
                {
                    case 0:
                        {
                            if (!foundKey)
                            {
                                foundKey = true;
                                m_keyboardInput.GetComponentInChildren<Text>().text = InputManager.instance.GetKeyString((KeyType)binds[i].value);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (!foundKey)
                            {
                                foundKey = true;
                                m_keyboardInput.GetComponentInChildren<Text>().text = InputManager.instance.GetMouseButtonString((MouseButton)binds[i].value);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (!foundButton)
                            {
                                foundButton = true;
                                m_gamePadInput.sprite = InputManager.instance.GetGamepadSprite((ButtonType)binds[i].value);
                            }
                            break;
                        }

                    default:
                    case 3:
                        break;
                }
            }

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
