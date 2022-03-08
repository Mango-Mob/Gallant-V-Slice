using System.Collections;
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
    public float m_defaultHealth { get; private set; } = 100.0f;

    public float m_barrier { get; private set; } = 0.0f;
    public float m_maxBarrier { get; private set; } = 50.0f;
    public int m_adrenaline { get; private set; } = 0;
    public int m_startingAdrenaline = 3;
    public int m_defaultAdrenaline { get; private set; } = 3;

    private Player_Controller playerController;
    public UI_Bar healthBar { get; private set; }
    public UI_PortraitHP portrait { get; private set; }
    public UI_Bar barrierBar { get; private set; }
    public UI_OrbCount adrenalineOrbs { get; private set; }

    public float m_adrenalineHeal = 40.0f;
    [SerializeField] private GameObject healVFXPrefab;
    public bool m_dead { get; private set; } = false;

    [Header("Barrier")]
    private float m_barrierDecayTimer = 0.0f;
    public float m_barrierDecayDelay = 0.5f;
    public float m_barrierDecayRate = 50.0f;

    private void Awake()
    {
        healthBar = HUDManager.Instance.GetElement<UI_Bar>("HP");
        barrierBar = HUDManager.Instance.GetElement<UI_Bar>("Barrier");
        portrait = HUDManager.Instance.GetElement<UI_PortraitHP>("Portrait");
        adrenalineOrbs = HUDManager.Instance.GetElement<UI_OrbCount>("Adrenaline");

        m_defaultAdrenaline = m_startingAdrenaline;
        m_adrenaline = m_startingAdrenaline;

    }
    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<Player_Controller>();

        // Skill implementation.
        m_adrenalineHeal *= 1.0f + playerController.playerSkills.m_healPowerIncrease;
        m_maxHealth += playerController.playerSkills.m_healthIncrease;
        m_health = m_maxHealth;
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
        {
            float healthDiff = healthPercentage - healthBar.GetValue();
            healthBar.SetValue(((Mathf.Abs(healthDiff) > 0.05f) // Check if health lerp difference is beyond range.
                ? (healthBar.GetValue() + Mathf.Sign(healthDiff) * Time.deltaTime) :  // If beyond range, lerp towards target.
                healthPercentage)); // If within range, set value.
        }

        if (portrait != null)
            portrait.UpdatePortrait(healthPercentage);

        if (barrierBar != null)
            barrierBar.SetValue(m_barrier / m_maxBarrier);

        if (adrenalineOrbs != null)
            adrenalineOrbs.SetValue(m_adrenaline);
    }


    public void ResetResources()
    {
        m_adrenaline = m_startingAdrenaline;
        m_health = m_maxHealth;
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
            playerController.playerAudioAgent.PlayDeath();
            playerController.playerAttack.ShowWeapons(false);
            playerController.animator.SetTrigger("KillPlayer");
            LevelManager.Instance.LoadNewLevel("EndScreen", LevelManager.Transition.YOUDIED);
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
        LevelManager.Instance.LoadNewLevel("MainMenu", LevelManager.Transition.YOUDIED);
    }

    /*******************
     * ChangeAdrenaline : Changes adrenaline value
     * @author : William de Beer
     * @param : (int) Amount to add to adrenaline
     */
    public void ChangeAdrenaline(int _amount)
    {
        if (_amount > 0)
            playerController.playerAudioAgent.PlayOrbPickup();

        m_adrenaline += _amount;
        m_adrenaline = m_adrenaline;
    }
    /*******************
     * UseAdrenaline : Consumes adrenaline to consume health if player has enough adrenaline.
     * @author : William de Beer
     */
    public void UseAdrenaline()
    {
        if (m_adrenaline >= 1)
        {
            playerController.playerAudioAgent.PlayUseAdrenaline(); // Audio
            ChangeHealth(m_adrenalineHeal);
            ChangeAdrenaline(-1);

            // Create VFX
            if (healVFXPrefab)
                Instantiate(healVFXPrefab, transform);
        }
    }

    public void FullHeal()
    {
        m_health = m_maxHealth;
    }
}
