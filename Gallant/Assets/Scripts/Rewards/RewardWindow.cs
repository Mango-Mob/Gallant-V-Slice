using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardWindow : MonoBehaviour
{
    public GameObject m_window;
    public GameObject[] m_rewardSlots;
    public GameObject m_weaponRewardOption;
    public GameObject m_itemRewardOption;

    [Header("Ability Description")]
    public Image m_abilityImage;
    public Text m_abilityDescription;
    public Text m_abilityCooldownText;

    [Header("Confirm Buttons")]
    public GameObject m_keyboardButton;
    public GameObject m_gamePadButton;
    private Image m_pressDurationImage;

    public List<ItemData> m_items = new List<ItemData>();
    [Range(0.0f, 1.0f)]
    public float[] m_weaponProbability;

    private const float probFirstWeapon = 1.0f;
    private const float probSecondWeapon = 0.6666f;
    private const float probThirdWeapon = 0.05f;

    public Player_Controller m_player;
    public float m_pressDuration = 1.0f;

    private int m_select = -1;
    private float m_timer = 0.0f;
    private List<Reward> m_rewards = new List<Reward>();
    private void Start()
    {
        m_pressDurationImage = m_gamePadButton.GetComponent<Image>();
        m_pressDurationImage.fillAmount = 0.0f;

        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.IsKeyDown(KeyType.O))
        {
            Show(1);
        }

        m_keyboardButton.SetActive(!InputManager.instance.isInGamepadMode);
        m_gamePadButton.SetActive(InputManager.instance.isInGamepadMode && m_select != -1);

        if(m_window.activeInHierarchy)
        {
            if (InputManager.instance.IsGamepadButtonPressed(ButtonType.WEST, 0))
            {
                m_timer += Time.unscaledDeltaTime;
                if (m_timer >= m_pressDuration)
                {
                    Confirm();
                }
                else
                {
                    m_pressDurationImage.fillAmount = m_timer / m_pressDuration;
                }
            }
            else
            {
                m_pressDurationImage.fillAmount = 0.0f;
            }

            if (InputManager.instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(m_rewards[0].gameObject);
            }
            else if(!InputManager.instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }
    }

    public void Show(int level)
    {
        m_window.SetActive(true);
        m_rewards.Clear();
        if (level >= 0)
        {
            for (int i = 0; i < m_rewardSlots.Length; i++)
            {
                for (int j = ((m_rewardSlots[i].transform as RectTransform).childCount) - 1; j >= 0; j--)
                {
                    Destroy((m_rewardSlots[i].transform as RectTransform).GetChild(j).gameObject);
                }
            }
            
            //Generate a random selection of rewards
            int rollSize = 10000;
            float roll = Random.Range(0, rollSize);
            List<ScriptableObject> rewards = new List<ScriptableObject>();

            for (int i = 0; i < m_rewardSlots.Length; i++)
            {
                if (roll <= m_weaponProbability[i] * rollSize)
                {
                    rewards.Add(WeaponData.GenerateWeapon(level));
                }
                else
                {
                    int select = Random.Range(0, m_items.Count);
                    rewards.Add(m_items[select]);
                }
            }
            //Shuffle
            rewards.Sort((a, b) => { return 1 - 2 * Random.Range(0, 2); });

            for (int i = 0; i < m_rewardSlots.Length; i++)
            {
                if (rewards[i].GetType() == typeof(WeaponData))
                {
                    WeaponReward wReward = GameObject.Instantiate(m_weaponRewardOption, m_rewardSlots[i].transform).GetComponent<WeaponReward>();
                    wReward.LoadWeapon(rewards[i] as WeaponData, m_player);
                    m_rewards.Add(wReward);
                    m_rewards[m_rewards.Count - 1].m_id = m_rewards.Count - 1;
                }
                else
                {
                    ItemReward iReward = GameObject.Instantiate(m_itemRewardOption, m_rewardSlots[i].transform).GetComponent<ItemReward>();
                    iReward.LoadItem(rewards[i] as ItemData, m_player);
                    m_rewards.Add(iReward);
                    m_rewards[m_rewards.Count - 1].m_id = m_rewards.Count - 1;
                }
            }
            EventSystem.current.SetSelectedGameObject(m_rewards[0].gameObject);
        }
    }

    public void Hover(int id)
    {
        WeaponReward temp = m_rewards[id] as WeaponReward;
        if (temp != null)
        {
            m_abilityImage.gameObject.SetActive(true);

            m_abilityImage.sprite = temp.m_activeWeapon.abilityData.abilityIcon;
            m_abilityDescription.text = AbilityData.EvaluateDescription(temp.m_activeWeapon.abilityData);

            if(temp.m_activeWeapon.abilityData.cooldownTime > 0)
                m_abilityCooldownText.text = temp.m_activeWeapon.abilityData.cooldownTime.ToString() + "s";
            else
                m_abilityCooldownText.text = "";
            return;
        }
        ItemReward temp2 = m_rewards[id] as ItemReward;
        if (temp2 != null)
        {
            m_abilityImage.gameObject.SetActive(false);
            m_abilityDescription.text = temp2.m_currentlyLoaded.description;
            m_abilityCooldownText.text = "";

        }
    }

    public void Select(int item)
    {
        if(m_select != -1)
            m_rewards[m_select].Unselect();

        m_select = item;
    }

    public void Confirm()
    {
        m_rewards[m_select].GiveReward();
    }

    public void Hide()
    {
        m_window.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }
}
