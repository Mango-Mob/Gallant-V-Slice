using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Collectable : MonoBehaviour
{
    public CollectableData m_data;
    public GameObject m_keyboardInput;
    public Image m_gamePadInput;

    public AudioClip m_pickupSound;

    public bool m_testMode = false;

    private bool m_ShowUI = false;
    private bool m_hasBeenCollected;

    // Start is called before the first frame update
    void Start()
    {
        m_hasBeenCollected = PlayerPrefs.GetInt(m_data.collectableID, 0) == 1;
        if(m_hasBeenCollected && !m_testMode)
        {
            Destroy(gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        m_keyboardInput.SetActive(m_ShowUI && !InputManager.Instance.isInGamepadMode);
        m_gamePadInput.gameObject.SetActive(m_ShowUI && InputManager.Instance.isInGamepadMode);

        if(m_ShowUI)
        {
            InputManager.Bind[] binds = InputManager.Instance.GetBinds("Interact");
            bool foundKey = false;
            bool foundButton = false;
            for (int i = 0; i < binds.Length; i++)
            {
                switch (InputManager.Bind.GetTypeID(binds[i].enumType))
                {
                    case 0:
                        {
                            if(!foundKey)
                            {
                                foundKey = true;
                                m_keyboardInput.GetComponentInChildren<Text>().text = InputManager.Instance.GetKeyString((KeyType)binds[i].value);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (!foundKey)
                            {
                                foundKey = true;
                                m_keyboardInput.GetComponentInChildren<Text>().text = InputManager.Instance.GetMouseButtonString((MouseButton)binds[i].value);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (!foundButton)
                            {
                                foundButton = true;
                                m_gamePadInput.sprite = InputManager.Instance.GetGamepadSprite((ButtonType)binds[i].value);
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

    public void Collect()
    {
        PlayerPrefs.SetInt(m_data.collectableID, 1);
        AudioManager.Instance.PlayAudioTemporary(transform.position, m_pickupSound);
        Destroy(gameObject);
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.tag == "Player")
        {
            m_ShowUI = true;
        }
    }

    public void OnTriggerExit(Collider other)
    {
        if (other.tag == "Player")
        {
            m_ShowUI = false;
        }
    }
}
