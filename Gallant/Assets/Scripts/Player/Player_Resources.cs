﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

/****************
 * Player_Resources: Manages player resources health and adrenaline.
 * @author : William de Beer
 * @file : Player_Resources.cs
 * @year : 2021
 */
public class Player_Resources : MonoBehaviour
{
    public float m_health { get; private set; } = 100.0f;
    public float m_maxHealth { get; private set; } = 100.0f;
    public float m_barrier { get; private set; } = 0.0f;
    public float m_maxBarrier { get; private set; } = 50.0f;
    public float m_adrenaline { get; private set; } = 0.0f;

    private Player_Controller playerController;
    public UI_Bar healthBar { get; private set; }
    public UI_PortraitHP portrait { get; private set; }
    public UI_Bar barrierBar { get; private set; }
    public UI_OrbResource adrenalineOrbs { get; private set; }

    public float m_adrenalineHeal = 40.0f;
    public bool m_dead { get; private set; } = false;

    [Header("Barrier")]
    private float m_barrierDecayTimer = 0.0f;
    public float m_barrierDecayDelay = 0.5f;
    public float m_barrierDecayRate = 50.0f;

    private void Awake()
    {
        healthBar = HUDManager.instance.GetElement<UI_Bar>("HP");
        barrierBar = HUDManager.instance.GetElement<UI_Bar>("Barrier");
        portrait = HUDManager.instance.GetElement<UI_PortraitHP>("Portrait");
        adrenalineOrbs = HUDManager.instance.GetElement<UI_OrbResource>("Adrenaline");
    }
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (m_barrierDecayTimer >= 0.0f)
        {
            m_barrierDecayTimer -= Time.deltaTime;
        }
        else
        {
            m_barrier -= m_barrierDecayRate * Time.deltaTime;
            if (m_barrier < 0.0f)
                m_barrier = 0.0f;
        }

        float healthPercentage = m_health / (m_maxHealth * playerController.playerStats.m_maximumHealth);
        if (healthBar != null)
            healthBar.SetValue(healthPercentage);

        if (portrait != null)
            portrait.UpdatePortrait(healthPercentage);

        if (barrierBar != null)
            barrierBar.SetValue(m_barrier / m_maxBarrier);

        if (adrenalineOrbs != null)
            adrenalineOrbs.SetValue(m_adrenaline);
    }

    /*******************
    * ChangeHealth : Changes health value
    * @author : William de Beer
    * @param : (float) Amount to add to health
    */
    public void ChangeHealth(float _amount)
    {
        m_health += _amount;
        if (m_health <= 0.0f && !m_dead)
        {
            // Kill
            m_dead = true;
            playerController.playerAttack.ShowWeapons(false);
            playerController.animator.SetTrigger("KillPlayer");
            LevelLoader.instance.LoadNewLevel("EndScreen", LevelLoader.Transition.YOUDIED);
            //StartCoroutine(BackToMenu());
        }
        m_health = Mathf.Clamp(m_health, 0.0f, (m_maxHealth * playerController.playerStats.m_maximumHealth));
    }

    /*******************
    * ChangeBarrier : Changes barrier value
    * @author : William de Beer
    * @param : (float) Amount to add to health
    * @return : (float) Damage to overflow into health bar
    */
    public float ChangeBarrier(float _amount)
    {
        m_barrier += _amount;
        float overflow = (m_barrier < 0) ? -m_barrier : 0.0f;

        m_barrier = Mathf.Clamp(m_barrier, 0.0f, 50.0f);

        m_barrierDecayTimer = m_barrierDecayDelay;

        return overflow;
    }

    public void ResetBarrier()
    {
        m_barrier = 0.0f;
    }
    IEnumerator BackToMenu()
    {
        yield return new WaitForSecondsRealtime(3);
        LevelLoader.instance.LoadNewLevel("MainMenu", LevelLoader.Transition.YOUDIED);
    }

    /*******************
     * ChangeAdrenaline : Changes adrenaline value
     * @author : William de Beer
     * @param : (float) Amount to add to adrenaline
     */
    public void ChangeAdrenaline(float _amount)
    {
        m_adrenaline += _amount;
        m_adrenaline = Mathf.Clamp(m_adrenaline, 0.0f, 3.0f);
    }
    /*******************
     * UseAdrenaline : Consumes adrenaline to consume health if player has enough adrenaline.
     * @author : William de Beer
     */
    public void UseAdrenaline()
    {
        if (m_adrenaline >= 1.0f)
        {
            playerController.playerAudioAgent.PlayUseAdrenaline(); // Audio
            ChangeHealth(m_adrenalineHeal);
            ChangeAdrenaline(-1.0f);
        }
    }
}
