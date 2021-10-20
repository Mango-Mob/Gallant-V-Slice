using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            healthBar.SetValue(m_health / m_maxHealth);

        if (adrenalineOrbs != null)
            adrenalineOrbs.SetValue(m_adrenaline);
    }

    public void ChangeHealth(float _amount)
    {
        m_health += _amount;
        if (m_health <= 0.0f && !m_dead)
        {
            // Kill
            m_dead = true;
        }
        m_health = Mathf.Clamp(m_health, 0.0f, m_maxHealth);
    }

    public void ChangeAdrenaline(float _amount)
    {
        m_adrenaline += _amount;
        if (_amount > 0) // Gain
        {

        }
        else // Drain
        {

        }
        m_adrenaline = Mathf.Clamp(m_adrenaline, 0.0f, 3.0f);
    }

    public void UseAdrenaline()
    {
        if (m_adrenaline >= 1.0f)
        {
            ChangeHealth(m_adrenalineHeal);
            ChangeAdrenaline(-1.0f);
        }
    }
}
