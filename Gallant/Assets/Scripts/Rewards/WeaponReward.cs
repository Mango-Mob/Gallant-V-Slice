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

    public Text m_rightDamage;
    public Text m_rightSpeed;

    //For each diffScale more +/- are displayed.
    private int m_damageDiffScale = 2;
    private float m_speedDiffScale = 3.0f;

    public void LoadWeapon(WeaponData data, Player_Attack playerData)
    {
        m_title.text = GetPrefix(data.abilityData.starPowerLevel) + " " + data.name + " of "+ data.abilityData.weaponTitle;
        m_levelText.text = "Level: " + data.m_level;

        m_weaponImageLoc.sprite = data.weaponIcon;
        m_abilityImageLoc.sprite = data.abilityData.abilityIcon;

        UpdateCompareField(m_leftDamage, data.m_damage - playerData.m_leftWeapon.m_damage, (float)m_damageDiffScale);
        UpdateCompareField(m_leftSpeed, data.m_speed - playerData.m_leftWeapon.m_speed, m_speedDiffScale);

        UpdateCompareField(m_rightDamage, data.m_damage - playerData.m_rightWeapon.m_damage, (float)m_damageDiffScale);
        UpdateCompareField(m_rightSpeed, data.m_speed - playerData.m_rightWeapon.m_speed, m_speedDiffScale);
    }

    /*******************
    * UpdateCompareField : Updates a text field with the aproximate damage difference ('+' equates to 0f-1f full diff scale).
    * @author : Michael Jordan
    * @param : (Text) The field to insert the aproximate difference (in +, - or =)
    * @param : (float) The actual difference between the player and this weapon.
    * @param : (float) The current difference scale.
    */
    private void UpdateCompareField(Text compareField, float currentDiff, float diffScale)
    {
        if (diffScale == 0)
            return;

        int diff = Mathf.Clamp(Mathf.CeilToInt(currentDiff / diffScale), -3, 3);

        if(diff > 0)
        {
            compareField.text = new String('+', diff);
            compareField.color = Color.green;
        }
        else if(diff < 0)
        {
            compareField.text = new String('-', diff);
            compareField.color = Color.red;
        }
        else
        {
            compareField.text = "=";
            compareField.color = Color.yellow;
        }
    }

    private string GetPrefix(int abilityLevel)
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
        
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
