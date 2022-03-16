using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardManager : Singleton<RewardManager>
{
    public static bool isShowing { get { return Instance.m_window.activeInHierarchy; } }

    public GameObject m_window;
    public InfoDisplay[] m_rewardSlots;
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

    public InfoDisplay m_leftHand;
    public InfoDisplay m_rightHand;

    private const float probFirstWeapon = 1.0f;
    private const float probSecondWeapon = 0.6666f;
    private const float probThirdWeapon = 0.05f;

    public Player_Controller m_player;
    public float m_pressDuration = 1.0f;

    private int m_select = -1;
    private float m_timer = 0.0f;

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

        if(m_window.activeInHierarchy)
            m_player.m_isDisabledInput = true;

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
                    EventSystem.current.SetSelectedGameObject(m_rewardSlots[0].transform.parent.gameObject);
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
        m_leftHand?.LoadWeapon(m_player.playerAttack.m_leftWeaponData);
        m_rightHand?.LoadWeapon(m_player.playerAttack.m_rightWeaponData);

        m_player.m_isDisabledInput = true;

        if (level >= 0)
        {
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
                m_rewardSlots[i].GetComponent<Button>().interactable = true;
                if (rewards[i].GetType() == typeof(WeaponData))
                {
                    m_rewardSlots[i].LoadWeapon(rewards[i] as WeaponData);
                }
                else
                {
                    m_rewardSlots[i].LoadItem(rewards[i] as ItemData);
                }
            }
            EventSystem.current.SetSelectedGameObject(m_rewardSlots[0].gameObject);
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

    public void Hover(int id)
    {
        ScriptableObject data;

        switch(id)
            {
            case 0: case 1: case 2:
                if(m_rewardSlots[id].IsAWeapon)
                    data = m_rewardSlots[id].m_weaponData;
                else
                    data = m_rewardSlots[id].m_itemData;
                break;
            default:
            case 3:
                data = m_leftHand.m_weaponData;
                break;
            case 4:
                data = m_rightHand.m_weaponData;
                break;
        }

        if(data is WeaponData)
        {
            WeaponData wData = data as WeaponData;
            if(wData.abilityData != null)
            {
                m_abilityImage.gameObject.SetActive(true);
                m_abilityDescription.text = AbilityData.EvaluateDescription(wData.abilityData);
                m_abilityCooldownText.text = wData.abilityData.cooldownTime.ToString() + "s";

                if (m_rewardSlots[id].m_weaponData.abilityData.cooldownTime > 0)
                {
                    m_abilityCooldownHeader.text = "Cooldown:";
                }
                else
                {
                    m_abilityCooldownHeader.text = "";
                    m_abilityCooldownText.text = "";
                }
            }
            else
            {
                m_abilityImage.gameObject.SetActive(false);
                m_abilityDescription.text = "";
                m_abilityCooldownHeader.text = "";
                m_abilityCooldownText.text = "";
            }
        }
        else if(data is ItemData)
        {
            ItemData iData = data as ItemData;
            m_abilityImage.sprite = m_rewardSlots[id].m_itemData.itemIcon;
            m_abilityCooldownHeader.text = "";
            m_abilityCooldownText.text = "";
            m_abilityDescription.text = iData.description;
        }
    }

    public void Select(int item)
    {      
        if(m_select != item)
        {
            m_select = item;
            for (int i = 0; i < m_rewardSlots.Length; i++)
            {
                m_rewardSlots[i].Select(i == item);
            }
        }
    }

    public void Confirm()
    {
        m_rewardSlots[m_select].GiveReward();
        m_player.m_isDisabledInput = false;
        m_select = -1;
    }

    public void Hide()
    {
        m_window.SetActive(false);
        m_player.m_isDisabledInput = false;
        m_select = -1;
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
