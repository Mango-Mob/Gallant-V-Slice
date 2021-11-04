using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WeaponReward : Reward
{
    public Text m_title;

    public Image m_weaponImageLoc;
    public Image m_abilityImageLoc;

    public Text m_levelText;

    public GameObject[] m_stars;

    public Text m_leftDamage;
    public Text m_leftSpeed;
    public Text m_leftKnockback;

    public Text m_rightDamage;
    public Text m_rightSpeed;
    public Text m_rightKnockback;

    //For each diffScale more +/- are displayed.
    static public int m_damageDiffScale = 2;
    static public float m_speedDiffScale = 3.0f;
    static public float m_knockDiffScale = 50.0f;

    private Player_Controller m_activePlayer;
    private WeaponData m_activeWeapon;

    public void LoadWeapon(WeaponData data, Player_Controller player)
    {
        m_activePlayer = player;
        m_activeWeapon = data;

        if (data.abilityData != null)
        {
            m_title.text = GetPrefix(data.abilityData.starPowerLevel) + " " + data.weaponName;
            m_abilityImageLoc.sprite = data.abilityData.abilityIcon;

            for (int i = 0; i < 3 - data.abilityData.starPowerLevel; i++)
            {
                m_stars[i].SetActive(false);
            }
        }
        else
        {
            m_title.text = data.weaponName;
            m_abilityImageLoc.sprite = null;
            foreach (var item in m_stars)
            {
                item.SetActive(false);
            }
        }

        m_levelText.text = "Level: " + data.m_level;

        m_weaponImageLoc.sprite = data.weaponIcon;

        CompareTo(data, player.playerAttack.m_leftWeapon, true);
        CompareTo(data, player.playerAttack.m_rightWeapon, false);
    }

    public void CompareTo(WeaponData rewardWeapon, WeaponData playerWeapon, bool isLeft = true)
    {
        float damage = (playerWeapon != null) ? playerWeapon.m_damage : 0;
        float speed = (playerWeapon != null) ? playerWeapon.m_speed : 0;
        float knockback = (playerWeapon != null) ? playerWeapon.m_knockback : 0;

        if(isLeft)
        {
            UpdateCompareField(m_leftDamage, rewardWeapon.m_damage, damage, (float)m_damageDiffScale);
            UpdateCompareField(m_leftSpeed, rewardWeapon.m_speed, speed, m_speedDiffScale);
            UpdateCompareField(m_leftKnockback, rewardWeapon.m_knockback, knockback, m_knockDiffScale);
        }
        else
        {
            UpdateCompareField(m_rightDamage, rewardWeapon.m_damage, damage, (float)m_damageDiffScale);
            UpdateCompareField(m_rightSpeed, rewardWeapon.m_speed, speed, m_speedDiffScale);
            UpdateCompareField(m_rightKnockback, rewardWeapon.m_knockback, knockback, m_knockDiffScale);
        }
    }

    /*******************
    * UpdateCompareField : Updates a text field with the aproximate damage difference ('+' equates to 0f-1f full diff scale).
    * @author : Michael Jordan
    * @param : (Text) The field to insert the aproximate difference (in +, - or =)
    * @param : (float) The actual difference between the player and this weapon.
    * @param : (float) The current difference scale.
    */
    public static void UpdateCompareField(Text compareField, float rewardStat, float currentStat, float diffScale)
    {
        if (diffScale == 0)
            return;
        
        int diff = Mathf.Clamp(Mathf.CeilToInt((rewardStat - currentStat) / diffScale), -3, 3);

        if(diff > 0)
        {
            compareField.text = new String('+', diff);
            compareField.color = Color.green;
        }
        else if(diff < 0)
        {
            compareField.text = new String('-', Mathf.Abs(diff));
            compareField.color = Color.red;
        }
        else
        {
            compareField.text = "=";
            compareField.color = Color.yellow;
        }
    }

    public static string GetPrefix(int abilityLevel)
    {
        switch (abilityLevel)
        {
            case 1:
                return "Lesser";
            default:
            case 2:
                return "";
            case 3:
                return "Greater";
        }
    }

    public override void GiveReward()
    {
        Vector3 pos = UnityEngine.Random.onUnitSphere;
        pos.y = 0;

        DroppedWeapon.CreateDroppedWeapon(m_activePlayer.transform.position + pos.normalized, m_activeWeapon);
        GetComponentInParent<RewardWindow>().Hide();
    }
}
