﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardManager : Singleton<RewardManager>
{
    public static bool isShowing { get { return Instance.m_window.activeInHierarchy; } }

    public GameObject m_window;
    public GameObject[] m_rewardSlots;
    public GameObject m_weaponRewardOption;
    public GameObject m_itemRewardOption;

    [Header("Ability Description")]
    public Image m_abilityImage;
    public Text m_abilityDescription;
    public Text m_abilityCooldownHeader;
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

    public enum RewardType
    {
        STANDARD,   //One weapon garenteed
        GENERAL,    //Completely random
        WEAPONS,    //Three weapons garenteeds
        RUNE,       //No Weapon garenteed
    }

    protected void Start()
    {
        m_pressDurationImage = m_gamePadButton.GetComponent<Image>();
        m_pressDurationImage.fillAmount = 0.0f;
        m_player = GameManager.Instance.m_player.GetComponent<Player_Controller>();
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        EndScreenMenu.elapsedTimeInSeconds += Time.deltaTime;

#if UNITY_EDITOR
        if (InputManager.Instance.IsKeyDown(KeyType.O))
        {
            Show(1);
        }
#endif

        m_keyboardButton.SetActive(!InputManager.Instance.isInGamepadMode && m_select != -1);
        m_gamePadButton.SetActive(InputManager.Instance.isInGamepadMode && m_select != -1);

        if(m_window.activeInHierarchy)
        {
            if(m_select >= 0)
            {
                if (InputManager.Instance.IsGamepadButtonPressed(ButtonType.WEST, 0))
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
                    m_timer = 0.0f;
                }

                if (InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject == null)
                {
                    EventSystem.current.SetSelectedGameObject(m_rewards[0].gameObject);
                }
                else if (!InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
                {
                    EventSystem.current.SetSelectedGameObject(null);
                }
            }
        }
    }

    public void Show(int level, RewardType type = RewardType.STANDARD)
    {
        m_window.SetActive(true);
        m_rewards.Clear();
        if (level >= 0)
        {
            m_player.m_isDisabledInput = true;
            for (int i = 0; i < m_rewardSlots.Length; i++)
            {
                for (int j = ((m_rewardSlots[i].transform as RectTransform).childCount) - 1; j >= 0; j--)
                {
                    Destroy((m_rewardSlots[i].transform as RectTransform).GetChild(j).gameObject);
                }
            }

            //Generate a random selection of rewards
            List<ScriptableObject> rewards = new List<ScriptableObject>();
            switch (type)
            {
                case RewardType.STANDARD:
                    //Garenteed weapon
                    rewards.Add(WeaponData.GenerateWeapon(level));
                    for (int i = 0; i < 2; i++)
                    {
                        if (IsAWeaponReward())
                        {
                            rewards.Add(GenerateWeapon(rewards, level));
                        }
                        else
                        {
                            rewards.Add(GenerateItem(rewards));
                        }
                    }
                    break;
                case RewardType.GENERAL:
                    //Complete random
                    for (int i = 0; i < 3; i++)
                    {
                        if (IsAWeaponReward())
                        {
                            rewards.Add(GenerateWeapon(rewards, level));
                        }
                        else
                        {
                            rewards.Add(GenerateItem(rewards));
                        }
                    }
                    break;
                case RewardType.WEAPONS:
                    //Three weapons
                    for (int i = 0; i < 3; i++)
                    {
                        rewards.Add(GenerateWeapon(rewards, level));
                    }
                    break;
                case RewardType.RUNE:
                    //No Weapons
                    for (int i = 0; i < 3; i++)
                    {
                        rewards.Add(GenerateItem(rewards));
                    }
                    break;
                default:
                    break;
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

    public bool IsAWeaponReward(int magnitude = 10000)
    {
        int totalOptions = m_items.Count + System.Enum.GetNames(typeof(Weapon)).Length;
        float probOfWeapon = (m_items.Count - totalOptions) / totalOptions;

        return (Random.Range(0, magnitude) <= probOfWeapon * magnitude);
    }

    public WeaponData GenerateWeapon(List<ScriptableObject> currentList, int level)
    {
        WeaponData weapon;
        do
        {
            weapon = WeaponData.GenerateWeapon(level);
        } while (!IsUniqueWeapon(currentList, weapon));

        return weapon;
    }
    public ItemData GenerateItem(List<ScriptableObject> currentList)
    {
        int select;
        do
        {
            select = Random.Range(0, m_items.Count);
        } while (!IsUniqueItem(currentList, m_items[select]));

        return m_items[select];
    }

    private void LoadRewards(RewardType type, int level)
    {
        
    }

    public void Hover(int id)
    {
        WeaponReward temp = m_rewards[id] as WeaponReward;
        if (temp != null)
        {
            m_abilityImage.gameObject.SetActive(true);

            m_abilityImage.sprite = temp.m_activeWeapon.abilityData.abilityIcon;
            m_abilityDescription.text = AbilityData.EvaluateDescription(temp.m_activeWeapon.abilityData);

            if (temp.m_activeWeapon.abilityData.cooldownTime > 0)
            {
                m_abilityCooldownHeader.text = "Cooldown:";
                m_abilityCooldownText.text = temp.m_activeWeapon.abilityData.cooldownTime.ToString() + "s";
            }
            else
            {
                m_abilityCooldownHeader.text = "";
                m_abilityCooldownText.text = "";
            }
            return;
        }
        ItemReward temp2 = m_rewards[id] as ItemReward;
        if (temp2 != null)
        {
            m_abilityImage.gameObject.SetActive(false);
            m_abilityDescription.text = temp2.m_currentlyLoaded.description;
            m_abilityCooldownHeader.text = "";
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
        m_player.m_isDisabledInput = false;
        m_select = -1;
    }

    public void Hide()
    {
        m_window.SetActive(false);
        EventSystem.current.SetSelectedGameObject(null);
    }

    public bool IsUniqueWeapon(List<ScriptableObject> list, WeaponData data)
    {
        foreach (var reward in list)
        {
            WeaponData weaponReward = reward as WeaponData;
            if (weaponReward != null)
            {
                if(weaponReward.weaponType == data.weaponType && weaponReward.abilityData.abilityPower == data.abilityData.abilityPower)
                {
                    return false;
                }
            }
        }
        return true;
    }

    public bool IsUniqueItem(List<ScriptableObject> list, ItemData data)
    {
        foreach (var reward in list)
        {
            ItemData itemReward = reward as ItemData;
            if (itemReward != null)
            {
                if (itemReward.itemEffect == data.itemEffect)
                {
                    return false;
                }
            }
        }
        return true;
    }
}