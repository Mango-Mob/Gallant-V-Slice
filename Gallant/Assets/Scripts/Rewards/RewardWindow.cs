using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RewardWindow : MonoBehaviour
{
    public GameObject m_window;
    public GameObject m_pannel;
    public GameObject m_weaponRewardOption;
    public GameObject m_itemRewardOption;

    [Header("Ability Description")]
    public Image m_abilityImage;
    public Text m_abilityDescription;
    public Text m_abilityCooldownText;

    public List<ItemData> m_items = new List<ItemData>();

    private const float probFirstWeapon = 1.0f;
    private const float probSecondWeapon = 0.6666f;
    private const float probThirdWeapon = 0.05f;

    public Player_Controller m_player;

    private int m_select = 0;
    
    private List<Reward> m_rewards = new List<Reward>();
    private void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.IsKeyDown(KeyType.O))
        {
            Show(1);
        }
    }

    public void Show(int level)
    {
        m_window.SetActive(true);
        m_rewards.Clear();
        if (level >= 0)
        {
            for (int i = ((m_pannel.transform as RectTransform).childCount) - 1; i >= 0; i--)
            {
                Destroy((m_pannel.transform as RectTransform).GetChild(i).gameObject);
            }

            //Generate a random selection of rewards
            int rollSize = 10000;
            float roll = Random.Range(0, rollSize);
            List<ScriptableObject> rewards = new List<ScriptableObject>();
            //First reward
            if(roll <= probFirstWeapon * rollSize)// < Max
            {
                rewards.Add(WeaponData.GenerateWeapon(level));
            }
            else
            {
                int select = Random.Range(0, m_items.Count);
                rewards.Add(m_items[select]);
            }
            //Second reward
            if (roll <= probSecondWeapon * rollSize)
            {
                rewards.Add(WeaponData.GenerateWeapon(level));
            }
            else
            {
                int select = Random.Range(0, m_items.Count);
                rewards.Add(m_items[select]);
            }
            //third reward
            if (roll <= probThirdWeapon * rollSize)
            {
                rewards.Add(WeaponData.GenerateWeapon(level));
            }
            else
            {
                int select = Random.Range(0, m_items.Count);
                rewards.Add(m_items[select]);
            }
            rewards.Sort((a, b) => { return 1 - 2 * Random.Range(0, 2); });

            foreach (var item in rewards)
            {
                if(item.GetType() == typeof(WeaponData))
                {
                    WeaponReward wReward = GameObject.Instantiate(m_weaponRewardOption, m_pannel.transform).GetComponent<WeaponReward>();
                    wReward.LoadWeapon(item as WeaponData, m_player);
                    m_rewards.Add(wReward);
                    m_rewards[m_rewards.Count - 1].m_id = m_rewards.Count - 1;
                }
                else
                {
                    ItemReward iReward = GameObject.Instantiate(m_itemRewardOption, m_pannel.transform).GetComponent<ItemReward>();
                    iReward.LoadItem(item as ItemData, m_player);
                    m_rewards.Add(iReward);
                    m_rewards[m_rewards.Count - 1].m_id = m_rewards.Count - 1;
                }
            }
        }
    }

    public void Select(int item)
    {
        m_rewards[m_select].Unselect();
        WeaponReward temp = m_rewards[item] as WeaponReward;
        if (temp != null)
        {
            m_abilityImage.sprite = temp.m_activeWeapon.abilityData.abilityIcon;
            m_abilityDescription.text = AbilityData.EvaluateDescription(temp.m_activeWeapon.abilityData);
            m_abilityCooldownText.text = temp.m_activeWeapon.abilityData.cooldownTime.ToString() + "s";
        }

        m_select = item;
    }

    public void Confirm()
    {
        m_rewards[m_select].GiveReward();
    }

    public void Hide()
    {
        m_window.SetActive(false);
    }
}
