using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponInfoDisplay : MonoBehaviour
{
    public Text m_title;

    [SerializeField] private Image m_weaponImageLoc;
    [SerializeField] private Image m_abilityImageLoc;
    [SerializeField] private Text m_levelText;

    [SerializeField] private GameObject[] m_stars;

    [SerializeField] private Text m_damage;
    [SerializeField] private Text m_speed;
    [SerializeField] private Text m_knockback;

    private Sprite m_defaultImage;
    public WeaponData m_activeWeapon { get; private set; }

    private void Awake()
    {
        m_defaultImage = Resources.Load<Sprite>("DefaultIcon");
    }

    public void LoadWeapon(WeaponData data)
    {
        for (int i = 0; i < 3; i++)
        {
            m_stars[i].SetActive(true);
        }

        if (data == null)
        {
            m_title.text = "None";
            m_weaponImageLoc.sprite = m_defaultImage;
            m_abilityImageLoc.sprite = m_defaultImage;
            foreach (var item in m_stars)
            {
                item.SetActive(false);
            }
            m_levelText.text = "";

            m_damage.text = "-";
            m_speed.text = "-";
            m_knockback.text = "-";
        }
        else
        {
            if (data.abilityData != null)
            {
                m_title.text = WeaponReward.GetPrefix(data.abilityData.starPowerLevel) + " " + data.weaponName;
                m_abilityImageLoc.sprite = data.abilityData.abilityIcon;

                for (int i = 0; i < 3 - data.abilityData.starPowerLevel; i++)
                {
                    m_stars[i].SetActive(false);
                }
            }
            else
            {
                m_title.text = data.weaponName;
                m_abilityImageLoc.sprite = m_defaultImage;
                foreach (var item in m_stars)
                {
                    item.SetActive(false);
                }
            }

            m_levelText.text = "Level: " + data.m_level;
            m_damage.text = data.m_damage.ToString();
            m_speed.text = data.m_speed.ToString("0.0");
            m_knockback.text = data.m_knockback.ToString("0.0");

            m_weaponImageLoc.sprite = data.weaponIcon;
        }
        

    }
}
