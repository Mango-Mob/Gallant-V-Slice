using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class RuneCountDisplay : MonoBehaviour
{
    private static Player_Controller playerController;
    public ItemData m_runeItem;
    public Image m_icon;

    [Header("Tally Mode")]
    public Image[] m_tallyMarks;

    [Header("Number Mode")]
    public TextMeshProUGUI m_numberValue;
    public TextMeshProUGUI m_fractionNumberValue;
    public Image m_runeFill;

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

        m_numberValue.text = effectCount.ToString();
        m_fractionNumberValue.text = $"{effectCount}/10";
        m_runeFill.fillAmount = effectCount / 10.0f;
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
