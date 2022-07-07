using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LavaFloor : MonoBehaviour
{
    public float m_damagePerSecond;

    public float m_delayTimeUp = 3f;
    public float m_delayTimeDown = 3f;
    public float m_transitionTime = 3f;
    public AnimationCurve m_fillCurve;
    public AnimationCurve m_emptyCurve;

    private bool m_isRising = false;
    private float m_initialY;
    private float m_timer;
    private int m_direction = 0;
    private GameObject m_player;
    // Start is called before the first frame update
    void Start()
    {
        m_initialY = transform.position.y;
        m_player = GameManager.Instance.m_player;
    }

    // Update is called once per frame
    void Update()
    {
        m_timer += Time.deltaTime;

        switch (m_direction)
        {
            default:
            case 0:
                if ((m_isRising) ? m_timer >= m_delayTimeUp : m_timer >= m_delayTimeDown)
                {
                    m_timer = 0;
                    m_isRising = !m_isRising;

                    m_direction = (m_isRising) ? 1 : -1;
                }
                break;
            case 1:
                transform.position = new Vector3(transform.position.x, m_initialY + m_fillCurve.Evaluate(m_timer / m_transitionTime), transform.position.z);
                if (m_timer >= m_transitionTime)
                {
                    m_timer = 0;
                    m_direction = 0;
                }
                break;
            case -1:
                transform.position = new Vector3(transform.position.x, m_initialY + m_emptyCurve.Evaluate(m_timer / m_transitionTime), transform.position.z);
                if (m_timer >= m_transitionTime)
                {
                    m_timer = 0;
                    m_direction = 0;
                }
                break;
        }

        if(m_player.transform.position.y - 0.5 <= transform.position.y)
        {
            m_player.GetComponent<Player_Controller>().DamagePlayer(m_damagePerSecond * Time.deltaTime, CombatSystem.DamageType.Ability, null, true);
        }
    }
}
