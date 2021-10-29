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

    public GameObject m_starLoc;

    public Text m_leftDamage;
    public Text m_leftSpeed;

    public Text m_rightDamage;
    public Text m_rightSpeed;

    //For each diffScale more +/- are displayed.
    private int m_damageDiffScale = 2;
    private float m_speedDiffScale = 3.0f;

    public void LoadWeapon(WeaponData data, Player_Attack playerData)
    {
        m_weaponImageLoc.sprite = data.weaponIcon;
        m_abilityImageLoc.sprite = data.abilityData.abilityIcon;

        int damageDiff_L = data.m_damage - playerData.m_leftWeapon.m_damage;
        float speedDiff_L = data.m_speed - playerData.m_leftWeapon.m_speed;

        int damageDiff_R = data.m_damage - playerData.m_rightWeapon.m_damage;
        float speedDiff_R = data.m_speed - playerData.m_rightWeapon.m_speed;


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
