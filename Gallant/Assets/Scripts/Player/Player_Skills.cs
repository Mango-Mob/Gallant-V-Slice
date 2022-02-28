using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_Skills : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }

    private float m_healthMult = 1.0f;
    private float m_attackSpeedMult = 1.0f;
    private float m_damageMult = 1.0f;
    private float m_moveSpeedMult = 1.0f;
    private float m_defenceMult = 1.0f;
    private float m_attackMoveMult = 1.0f;
    private float m_cooldownMult = 1.0f;

    // Start is called before the first frame update
    private void Start()
    {
        playerController = GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
