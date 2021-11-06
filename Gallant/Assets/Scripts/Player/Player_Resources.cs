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
    public float m_adrenaline { get; private set; } = 0.0f;

    private Player_Controller playerController;
    public UI_Bar healthBar;
    public UI_OrbResource adrenalineOrbs;

    public float m_adrenalineHeal = 40.0f;
    public bool m_dead { get; private set; } = false;


    // Start is called before the first frame update
    void Start()
    {
        playerController = GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        if (healthBar != null)
            healthBar.SetValue(m_health / (m_maxHealth * playerController.playerStats.m_maximumHealth));

        if (adrenalineOrbs != null)
            adrenalineOrbs.SetValue(m_adrenaline);
    }

    /*******************
    * ChangeHealth : Changes health value
    * @author : William de Beer
    * @param : (float) Amount to add to health
    * @return : (type) 
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
            StartCoroutine(BackToMenu());
        }
        m_health = Mathf.Clamp(m_health, 0.0f, (m_maxHealth * playerController.playerStats.m_maximumHealth));
    }

    IEnumerator BackToMenu()
    {
        yield return new WaitForSecondsRealtime(3);
        SceneManager.LoadScene(0);   
    }

    /*******************
     * ChangeAdrenaline : Changes adrenaline value
     * @author : William de Beer
     * @param : (float) Amount to add to adrenaline
     * @return : (type) 
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
