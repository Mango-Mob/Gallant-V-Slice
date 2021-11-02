using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardWindow : MonoBehaviour
{
    public GameObject m_window;

    public GameObject m_weaponRewardOption;
    public GameObject m_itemRewardOption;

    public List<ItemData> m_items = new List<ItemData>();

    private const float probFirstWeapon = 1.0f;
    private const float probSecondWeapon = 0.6666f;
    private const float probThirdWeapon = 0.05f;

    public Player_Controller m_player;

    private void Start()
    {
        Hide();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show(bool generate = false)
    {
        m_window.SetActive(true);

        if(generate)
        {
            for (int i = ((m_window.transform as RectTransform).childCount) - 1; i >= 0; i--)
            {
                Destroy((m_window.transform as RectTransform).GetChild(i).gameObject);
            }

            //Generate a random selection of rewards
            int rollSize = 10000;
            float roll = Random.Range(0, rollSize);
            List<ScriptableObject> rewards = new List<ScriptableObject>();
            //First reward
            if(roll <= probFirstWeapon * rollSize)// < Max
            {
                rewards.Add(WeaponData.GenerateWeapon(1));
            }
            else
            {
                int select = Random.Range(0, m_items.Count);
                rewards.Add(m_items[select]);
            }
            //Second reward
            if (roll <= probSecondWeapon * rollSize)
            {
                rewards.Add(WeaponData.GenerateWeapon(1));
            }
            else
            {
                int select = Random.Range(0, m_items.Count);
                rewards.Add(m_items[select]);
            }
            //third reward
            if (roll <= probThirdWeapon * rollSize)
            {
                rewards.Add(WeaponData.GenerateWeapon(1));
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
                    WeaponReward wReward = GameObject.Instantiate(m_weaponRewardOption, m_window.transform).GetComponent<WeaponReward>();
                    wReward.LoadWeapon(item as WeaponData, m_player);
                }
                else
                {
                    ItemReward iReward = GameObject.Instantiate(m_itemRewardOption, m_window.transform).GetComponent<ItemReward>();
                    iReward.LoadItem(item as ItemData, m_player);
                }
            }
        }
    }

    public void Hide()
    {
        m_window.SetActive(false);
    }
}
