using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class RewardManager : Singleton<RewardManager>
{
    public static bool giveRewardUponLoad = false;
    public static bool isShowing { get { return Instance.m_window.activeInHierarchy; } }

    public GameObject m_window;
    public Text m_rewardTitle;
    public InfoDisplay[] m_rewardSlots;

    [Header("Ability Description")]
    public Image m_abilityImage;
    public Text m_abilityDescription;
    public Text m_abilityCooldownHeader;
    public Text m_abilityCooldownText;

    [Header("Confirm Buttons")]
    public GameObject m_keyboardButton;
    public GameObject m_gamePadButton;
    private Image m_pressDurationImage;

    public List<Extentions.WeightedOption<ItemData>> m_runes = new List<Extentions.WeightedOption<ItemData>>();
    public ItemData m_randomRune;
    public ItemData m_forgeItem;
    public ItemData m_orbItem;
    public List<AbilityData> m_abilityBooks = new List<AbilityData>();
    [Range(0.0f, 1.0f)]
    public float[] m_weaponProbability;

    public GameObject m_currentWeapons;
    public GameObject m_currentRunes;
    public InfoDisplay m_leftHand;
    public InfoDisplay m_rightHand;

    public Player_Controller m_player;
    public float m_pressDuration = 1.0f;

    public Animator m_inventoryAnimator;
    public Image m_inventoryImage;
    public GameObject m_currentlyEquip;
    
    [Header("Probabilities")]
    public AnimationCurve[] m_spellTierWeight;

    private int m_hover;
    private int m_select = -1;
    private float m_timer = 0.0f;
    private SoloAudioAgent m_audio;
    private bool m_isDialogueReward = false;
    private UnityAction<int> m_onResult;
    public bool IsVisible { get { return m_window.activeInHierarchy; } }
    public enum RewardType
    {
        STANDARD,   //One weapon garenteed
        GENERAL,    //Completely random
        WEAPONS,    //Three weapons garenteeds
        RUNE,       //No Weapon garenteed
        BOOK,       //Books only
    }

    public enum Utility
    {
        BOOK,
        FORGE,
        ORB,
    }
    protected void Start()
    {
        m_pressDurationImage = m_gamePadButton.GetComponent<Image>();
        m_pressDurationImage.fillAmount = 0.0f;
        m_player = GameManager.Instance.m_player.GetComponent<Player_Controller>();
        m_audio = GetComponent<SoloAudioAgent>();
        if (giveRewardUponLoad)
        {
            Show(Mathf.FloorToInt(GameManager.currentLevel), RewardType.WEAPONS);
            giveRewardUponLoad = false;
        }
        else
        {
            Hide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        GameManager.m_runTime += Time.deltaTime;
        EndScreenMenu.elapsedTimeInSeconds += Time.deltaTime;

        if(m_window.activeInHierarchy)
            m_player.m_isDisabledInput = true;

#if UNITY_EDITOR
        if (InputManager.Instance.IsKeyDown(KeyType.O))
        {
            Show(10, RewardType.RUNE);
        }
#endif
        m_keyboardButton.SetActive(!InputManager.Instance.isInGamepadMode && m_select != -1);
        m_gamePadButton.SetActive(InputManager.Instance.isInGamepadMode && m_select != -1);
        m_inventoryImage.gameObject.SetActive(InputManager.Instance.isInGamepadMode);
        if (m_window.activeInHierarchy)
        {
            if (InputManager.Instance.isInGamepadMode)
            {
                if (m_hover == -1)
                    m_hover = 0;

                if (InputManager.Instance.IsGamepadButtonDown(ButtonType.LB, 0))
                {
                    m_inventoryAnimator.SetTrigger("Visible");
                }
                if (InputManager.Instance.IsGamepadButtonDown(ButtonType.RIGHT, 0))
                {
                    m_hover++;
                }
                if (InputManager.Instance.IsGamepadButtonDown(ButtonType.LEFT, 0))
                {
                    m_hover--;
                }

                Mathf.Clamp(m_hover, 0, 2);

                for (int i = 0; i < m_rewardSlots.Length; i++)
                {
                    m_rewardSlots[i].Hover(m_hover == i);
                }

                if (InputManager.Instance.IsGamepadButtonDown(ButtonType.SOUTH, 0))
                {
                    Select(m_hover);
                }
            }
            else
            {
                m_hover = -1;
            }

            if (m_select >= 0)
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
            }
        }
    }

    public void ShowSolo(int level, RewardType type = RewardType.STANDARD, bool isDialogue = false)
    {
        m_audio.Play();
        m_rewardTitle.text = "Reward";
        m_window.SetActive(true);
        m_leftHand?.LoadWeapon(m_player.playerAttack.m_leftWeaponData);
        m_rightHand?.LoadWeapon(m_player.playerAttack.m_rightWeaponData);
        m_currentWeapons.SetActive(true);
        m_currentRunes.SetActive(false);
        m_player.m_isDisabledInput = true;
        m_onResult = GiveReward;
        m_isDialogueReward = isDialogue;

        m_currentlyEquip.SetActive(false);
        if (level >= 0)
        {
            List<ScriptableObject> rewards = new List<ScriptableObject>();
            switch (type)
            {
                case RewardType.STANDARD:
                case RewardType.GENERAL:
                    if (IsAWeaponReward())
                    {
                        rewards.Add(GenerateWeapon(rewards, level));
                    }
                    else
                    {
                        rewards.Add(GenerateUtility(rewards, level));
                    }
                    break;
                case RewardType.WEAPONS:
                    rewards.Add(GenerateWeapon(rewards, level));
                    break;
                case RewardType.RUNE:
                    rewards.Add(ExchangeData.CreateExchange(GenerateRune(rewards, 1), 3, m_randomRune, 3));
                    break;
                case RewardType.BOOK:
                    rewards.Add(GenerateBook(rewards, level));
                    break;
                default:
                    break;
            }

            foreach (var item in m_rewardSlots)
            {
                item.gameObject.SetActive(false);
            }
            m_rewardSlots[1].gameObject.SetActive(true);

            m_rewardSlots[1].GetComponent<Button>().interactable = false;
            if (rewards[0].GetType() == typeof(WeaponData))
            {
                m_rewardSlots[1].LoadWeapon(rewards[0] as WeaponData);
            }
            else if (rewards[0].GetType() == typeof(AbilityData))
            {
                m_rewardSlots[1].LoadAbility(rewards[0] as AbilityData);
            }
            else if (rewards[0].GetType() == typeof(ExchangeData))
            {
                m_rewardSlots[1].LoadExchange(rewards[0] as ExchangeData);
            }
            else
            {
                m_rewardSlots[1].LoadItem(rewards[0] as ItemData);
            }

            m_select = 1;
        }
    }

    public void ShowSolo(ScriptableObject data, UnityAction<int> onResult = null, bool isDialogue = false)
    {
        m_window.SetActive(true);
        m_rewardTitle.text = "Reward";
        m_leftHand?.LoadWeapon(m_player.playerAttack.m_leftWeaponData);
        m_rightHand?.LoadWeapon(m_player.playerAttack.m_rightWeaponData);
        m_player.m_isDisabledInput = true;

        m_rewardSlots[0].gameObject.SetActive(false);
        m_rewardSlots[1].gameObject.SetActive(true);
        m_rewardSlots[2].gameObject.SetActive(false);
        m_isDialogueReward = isDialogue;

        m_currentWeapons.SetActive(true);
        m_currentRunes.SetActive(false);
        m_currentlyEquip.SetActive(false);
        if (data.GetType() == typeof(WeaponData))
        {
            m_rewardSlots[1].LoadWeapon(data as WeaponData);
        }
        else if (data.GetType() == typeof(AbilityData))
        {
            m_rewardSlots[1].LoadAbility(data as AbilityData);
        }
        else if (data.GetType() == typeof(ExchangeData))
        {
            m_rewardSlots[1].LoadExchange(data as ExchangeData);
        }
        else if(data.GetType() == typeof(ItemData))
        {
            m_rewardSlots[1].LoadItem(data as ItemData);
        }

        if (onResult != null)
            m_onResult = onResult;
        else
            m_onResult = GiveReward;

        m_select = 1;
    }

    public void Show(WeaponData data1, WeaponData data2, WeaponData data3, UnityAction<int> onResult = null)
    {
        m_window.SetActive(true);
        m_leftHand?.LoadWeapon(m_player.playerAttack.m_leftWeaponData);
        m_rightHand?.LoadWeapon(m_player.playerAttack.m_rightWeaponData);
        m_player.m_isDisabledInput = true;

        m_currentlyEquip.SetActive(true);

        m_rewardSlots[0].LoadWeapon(data1);
        m_rewardSlots[1].LoadWeapon(data2);
        m_rewardSlots[2].LoadWeapon(data3);
        m_isDialogueReward = false;

        m_currentWeapons.SetActive(true);
        m_currentRunes.SetActive(false);

        foreach (var item in m_rewardSlots)
        {
            item.gameObject.SetActive(true);
        }

        m_onResult = onResult;

        EventSystem.current.SetSelectedGameObject(m_rewardSlots[0].gameObject);
    }

    public void Show(int level, RewardType type = RewardType.STANDARD, bool isDialogue = false)
    {
        m_audio.Play();
        m_rewardTitle.text = "Choose";
        m_window.SetActive(true);
        m_leftHand?.LoadWeapon(m_player.playerAttack.m_leftWeaponData);
        m_rightHand?.LoadWeapon(m_player.playerAttack.m_rightWeaponData);
        m_currentWeapons.SetActive(true);
        m_currentRunes.SetActive(false);
        m_player.m_isDisabledInput = true;
        m_onResult = GiveReward;
        m_isDialogueReward = isDialogue;

        foreach (var item in m_rewardSlots)
        {
            item.gameObject.SetActive(true);
        }
        m_currentlyEquip.SetActive(true);

        if (level >= 0)
        {
            //Generate a random selection of rewards
            List<ScriptableObject> rewards = new List<ScriptableObject>();
            switch (type)
            {
                case RewardType.STANDARD:
                    //Garenteed weapon
                    rewards.Add(GenerateWeapon(rewards, level));
                    for (int i = 0; i < 2; i++)
                    {
                        if (IsAWeaponReward())
                        {
                            rewards.Add(GenerateWeapon(rewards, level));
                        }
                        else
                        {
                            rewards.Add(GenerateUtility(rewards, level));
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
                            rewards.Add(GenerateUtility(rewards, level));
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
                    m_currentWeapons.SetActive(false);
                    m_currentRunes.SetActive(true);
                    for (int i = 0; i < 3; i++)
                    {
                        rewards.Add(ExchangeData.CreateExchange(GenerateRune(rewards, 1), 3, m_randomRune, 3));
                    }
                    m_rewardTitle.text = "Exchange";
                    m_currentlyEquip.SetActive(false);
                    break;
                case RewardType.BOOK:
                    //Only Abilities
                    for (int i = 0; i < 3; i++)
                    {
                        rewards.Add(GenerateBook(rewards, level));
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
                else if (rewards[i].GetType() == typeof(AbilityData))
                {
                    m_rewardSlots[i].LoadAbility(rewards[i] as AbilityData);
                }
                else if(rewards[i].GetType() == typeof(ExchangeData))
                {
                    m_rewardSlots[i].LoadExchange(rewards[i] as ExchangeData);
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
        int totalOptions = m_runes.Count + System.Enum.GetNames(typeof(Weapon)).Length;
        float probOfWeapon = (m_runes.Count - totalOptions) / totalOptions;

        return (Random.Range(0, magnitude) <= probOfWeapon * magnitude);
    }

    public WeaponData GenerateWeapon(List<ScriptableObject> currentList, int level)
    {
        List<Extentions.WeightedOption<int>> spellLevels = new List<Extentions.WeightedOption<int>>();
        for (int i = 0; i < m_spellTierWeight.Length; i++)
        {
            Extentions.WeightedOption<int> option = new Extentions.WeightedOption<int>();
            option.data = i + 1;
            option.weight = (uint)m_spellTierWeight[i].Evaluate(level);
            spellLevels.Add(option);
        }
        int spellLevel = Extentions.GetFromList<int>(spellLevels);
        WeaponData weapon;
        do
        {
            weapon = WeaponData.GenerateWeapon(level, spellLevel);
        } while (!IsUniqueWeapon(currentList, weapon));

        return weapon;
    }

    public ScriptableObject GenerateUtility(List<ScriptableObject> currentList, int level, float weaponWeight = 50f, float spellWeight = 50f)
    {
        float potent = m_player.playerResources.GetPotentialHealth();
        float max = m_player.playerResources.m_maxHealth + 5 * m_player.playerResources.m_adrenalineHeal;
        if (!IsUniqueItem(currentList, m_orbItem))
            potent = max;

        float weaponCurr = m_player.playerAttack.GetAverageLevel();
        float weaponMax = GameManager.currentLevel + 2;
        if (!IsUniqueItem(currentList, m_forgeItem))
            weaponCurr = weaponMax;

        //Orb, forge, weapon, spell
        (string, float)[] weights = { ("orb", 100 * (1.0f - potent / max)), ("forge", 100 * (1.0f - weaponCurr / weaponMax)), ("weapon", weaponWeight), ("spell", spellWeight) };
        System.Array.Sort(weights, (a, b) => { return (int)((100 * b.Item2) - (a.Item2 * 100)); });

        float weightTotal = 0;
        foreach (var option in weights)
            weightTotal += option.Item2;

        float select = Random.Range(1, weightTotal + 1);
        string selectName = "";
        for (int i = 0; i < weights.Length; i++)
        {
            select -= weights[i].Item2;
            if(select <= 0)
            {
                selectName = weights[i].Item1;
                break;
            }  
        }

        switch (selectName)
        {
            case "orb":
                return m_orbItem;
            case "forge":
                return m_forgeItem;
            case "weapon":
                return GenerateWeapon(currentList, level);
            case "spell":
                return GenerateBook(currentList, level);
            default:
                break;
        }
        return null;
    }

    public AbilityData GenerateBook(List<ScriptableObject> currentList, int level)
    {
        List<Extentions.WeightedOption<int>> spellLevels = new List<Extentions.WeightedOption<int>>();
        for (int i = 0; i < m_spellTierWeight.Length; i++)
        {
            Extentions.WeightedOption<int> option = new Extentions.WeightedOption<int>();
            option.data = i + 1;
            option.weight = (uint)m_spellTierWeight[i].Evaluate(level);
            spellLevels.Add(option);
        }
        int spellLevel = Extentions.GetFromList<int>(spellLevels);

        List<AbilityData> options = new List<AbilityData>(m_abilityBooks);
        for (int i = options.Count - 1; i >= 0; i--)
        {
            if(options[i].starPowerLevel != spellLevel)
            {
                options.RemoveAt(i);
            }
        }

        int select;
        do
        {
            select = Random.Range(0, options.Count);
        } while (!IsUniqueAbility(currentList, options[select]));

        return options[select];
    }

    public ItemData GenerateItem(List<ScriptableObject> currentList)
    {
        List<ItemData> options = new List<ItemData>();
        options.Add(m_orbItem);
        options.Add(m_forgeItem);

        int select;
        select = Random.Range(0, options.Count);
        return options[select];
    }

    public ItemData GenerateRune(List<ScriptableObject> currentList, int offset = 1, bool isInverse = false)
    {
        for (int i = m_runes.Count - 1; i >= 0; i--)
        {
            if(IsUniqueRune(currentList, m_runes[i].data.itemEffect))
            {
                uint newWeight = 0;
                if (isInverse)
                {
                    newWeight = (uint)m_player.playerStats.GetEffectQuantity(m_runes[i].data.itemEffect);
                    m_runes.SetWeightAt<ItemData>(i, (newWeight - offset < 0) ? 0 : newWeight + 1);
                }
                else
                {
                    newWeight = (uint)m_player.playerStats.GetEffectQuantity(m_runes[i].data.itemEffect);
                    m_runes.SetWeightAt<ItemData>(i, (newWeight + offset > 10) ? 0 : (10 - newWeight) + 1);
                }
            }
            else
            {
                m_runes.SetWeightAt<ItemData>(i, 0);
            }
        }

        return Extentions.GetFromList<ItemData>(m_runes);
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
        if(m_onResult != null)
        {
            m_onResult.Invoke(m_select);
            m_select = -1;
        }
        m_player.m_isDisabledInput = false;
        Hide();
    }

    public void GiveReward(int selected)
    {
        m_rewardSlots[selected].GiveReward();

        if (m_isDialogueReward)
        {
            DialogManager.Instance.Show();
            m_isDialogueReward = false;
        }
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
    private bool IsUniqueRune(List<ScriptableObject> list, ItemEffect data)
    {
        foreach (var reward in list)
        {
            ExchangeData exchangeReward = reward as ExchangeData;
            if (exchangeReward != null)
            {
                if (exchangeReward.m_gainRune.itemEffect == data)
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


    private bool IsUniqueAbility(List<ScriptableObject> list, AbilityData data)
    {
        foreach (var reward in list)
        {
            AbilityData abilityReward = reward as AbilityData;
            if (abilityReward != null)
            {
                if (abilityReward.abilityPower == data.abilityPower)
                {
                    return false;
                }
            }
        }
        return true;
    }

}
