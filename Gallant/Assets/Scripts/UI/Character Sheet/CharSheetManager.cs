using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CharSheetManager : MonoBehaviour
{
    private Player_Controller playerController;
    private bool m_isActive = false;

    [Header("Run Information")]
    [SerializeField] private CharSheetInfo m_class;
    [SerializeField] private CharSheetInfo m_time;
    [SerializeField] private CharSheetInfo m_kills;
    [SerializeField] private CharSheetInfo m_damageDone;
    [SerializeField] private CharSheetInfo m_healingDone;

    [Header("Statistics")]
    [SerializeField] private CharSheetInfo m_maxHealth;
    [SerializeField] private CharSheetInfo m_maxStamina;
    [SerializeField] private CharSheetInfo m_attackSpeed;
    [SerializeField] private CharSheetInfo m_moveSpeed;
    [SerializeField] private CharSheetInfo m_armor;
    [SerializeField] private CharSheetInfo m_ward;
    [SerializeField] private CharSheetInfo m_physicalDamage;
    [SerializeField] private CharSheetInfo m_magicalDamage;
    [SerializeField] private CharSheetInfo m_cooldown;

    [Header("Main Hand")]
    [Header("Weapons")]
    [SerializeField] private Image m_mainIcon;
    [SerializeField] private CharSheetInfo m_damageMainHand;
    [SerializeField] private CharSheetInfo m_speedMainHand;
    [SerializeField] private CharSheetInfo m_impactMainHand;
    [SerializeField] private CharSheetInfo m_pierceMainHand;

    [Header("Off Hand")]
    [SerializeField] private Image m_offIcon;
    [SerializeField] private CharSheetInfo m_damageOffHand;
    [SerializeField] private CharSheetInfo m_speedOffHand;
    [SerializeField] private CharSheetInfo m_impactOffHand;
    [SerializeField] private CharSheetInfo m_pierceOffHand;
    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<Player_Controller>();
    }
    public void SetActive(bool _active)
    {
        m_isActive = _active;
    }
    // Update is called once per frame
    void Update()
    {
        if (m_isActive)
        {
            if (playerController.m_inkmanClass != null)
            {
                m_class.SetInformation(playerController.m_inkmanClass.m_className);
            }

            // Run info
            m_time.SetInformation(GameManager.CalculateTimerString(GameManager.m_runTime));
            m_kills.SetInformation("0");
            m_damageDone.SetInformation("0");
            m_healingDone.SetInformation("0");

            //m_time.SetInformation("00:00:00");
            //m_kills.SetInformation("0");
            //m_damageDone.SetInformation("0");
            //m_healingDone.SetInformation("0");

            // Stats
            m_maxHealth.SetInformation($"{playerController.playerResources.m_maxHealth}");
            m_maxStamina.SetInformation($"{playerController.playerResources.m_maxStamina}");
            m_attackSpeed.SetInformation((playerController.m_dualWieldBonus * playerController.playerStats.m_attackSpeed * playerController.playerSkills.m_attackSpeedStatusBonus).ToString("0.0%"));
            m_moveSpeed.SetInformation(playerController.animator.GetFloat("MovementSpeed").ToString("0.0%"));
            m_armor.SetInformation($"{playerController.playerSkills.m_physicalDefenceIncrease + playerController.playerStats.m_physicalDefence}");
            m_ward.SetInformation($"{playerController.playerSkills.m_magicalDefenceIncrease + playerController.playerStats.m_abilityDefence}");
            m_physicalDamage.SetInformation((playerController.playerStats.m_physicalDamage).ToString("0.0%"));
            m_magicalDamage.SetInformation((playerController.playerStats.m_abilityDamage
                + (playerController.playerStats.m_arcaneFocus * ((playerController.playerAttack.m_leftWeaponEffect == ItemEffect.ARCANE_FOCUS ? playerController.playerAttack.m_leftWeaponData.m_damage : 0.0f))
                + (playerController.playerStats.m_arcaneFocus * ((playerController.playerAttack.m_rightWeaponEffect == ItemEffect.ARCANE_FOCUS ? playerController.playerAttack.m_rightWeaponData.m_damage : 0.0f))))).ToString("0.0%"));
            m_cooldown.SetInformation(playerController.playerStats.m_abilityCD.ToString("0.0%"));

            bool isTwohanding = false;

            // Attacks
            if (playerController.playerAttack.m_rightWeaponData != null)
            {
                if (playerController.playerAttack.m_rightWeaponData.isTwoHanded)
                {
                    isTwohanding = true;

                    if (m_offIcon)
                    {
                        m_offIcon.enabled = true;
                        m_offIcon.sprite = playerController.playerAttack.m_rightWeaponData.altAttackIcon;
                    }

                    m_damageOffHand.SetInformation($"{playerController.playerAttack.m_rightWeaponData.m_damage * playerController.playerAttack.m_rightWeaponData.m_altDamageMult * playerController.playerStats.m_physicalDamage}");
                    m_speedOffHand.SetInformation($"{playerController.animator.GetFloat("LeftAttackSpeed")}");
                    m_impactOffHand.SetInformation($"{playerController.playerAttack.m_rightWeaponData.m_impact * playerController.playerAttack.m_rightWeaponData.m_altImpactMult}");
                    m_pierceOffHand.SetInformation($"{playerController.playerAttack.m_rightWeaponData.m_piercing}");
                }

                if (m_mainIcon)
                {
                    m_mainIcon.enabled = true;
                    m_mainIcon.sprite = playerController.playerAttack.m_rightWeaponData.weaponIcon;
                }

                m_damageMainHand.SetInformation($"{playerController.playerAttack.m_rightWeaponData.m_damage * playerController.playerStats.m_physicalDamage}");
                m_speedMainHand.SetInformation($"{playerController.animator.GetFloat("RightAttackSpeed")}");
                m_impactMainHand.SetInformation($"{playerController.playerAttack.m_rightWeaponData.m_impact}");
                m_pierceMainHand.SetInformation($"{playerController.playerAttack.m_rightWeaponData.m_piercing}");
            }
            else
            {
                if (m_mainIcon)
                    m_mainIcon.enabled = false;

                m_damageMainHand.SetInformation("0");
                m_speedMainHand.SetInformation("0");
                m_impactMainHand.SetInformation("0");
                m_pierceMainHand.SetInformation("0");
            }

            if (!isTwohanding)
            {
                if (playerController.playerAttack.m_leftWeaponData != null)
                {
                    if (m_offIcon)
                    {
                        m_offIcon.enabled = true;
                        m_offIcon.sprite = playerController.playerAttack.m_leftWeaponData.altAttackIcon;
                    }

                    m_damageOffHand.SetInformation($"{playerController.playerAttack.m_leftWeaponData.m_damage * playerController.playerAttack.m_leftWeaponData.m_altDamageMult * playerController.playerStats.m_physicalDamage}");
                    m_speedOffHand.SetInformation($"{playerController.animator.GetFloat("LeftAttackSpeed")}");
                    m_impactOffHand.SetInformation($"{playerController.playerAttack.m_leftWeaponData.m_impact * playerController.playerAttack.m_leftWeaponData.m_altImpactMult}");
                    m_pierceOffHand.SetInformation($"{playerController.playerAttack.m_leftWeaponData.m_piercing}");
                }
                else
                {
                    if (m_offIcon)
                        m_offIcon.enabled = false;

                    m_damageOffHand.SetInformation("0");
                    m_speedOffHand.SetInformation("0");
                    m_impactOffHand.SetInformation("0");
                    m_pierceOffHand.SetInformation("0");
                }
            }
        }
    }
}
