using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;

public class RuneCountDisplay : MonoBehaviour
{
    private static Player_Controller playerController;
    public ItemData m_runeItem;
    public Image m_icon;
    public Image[] m_tallyMarks;

    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<Player_Controller>();
    }

    // Update is called once per frame
    void Update()
    {
        int effectCount = playerController.playerStats.GetEffectQuantity(m_runeItem.itemEffect);
        for (int i = 0; i < m_tallyMarks.Length; i++)
        {
            m_tallyMarks[i].enabled = i < effectCount;
        }
    }
    private void OnValidate()
    {
        if (m_runeItem != null)
        {
            name = $"{m_runeItem.name}Display";

            if (m_icon != null)
                m_icon.sprite = m_runeItem.itemIcon;
        }
    }
}
