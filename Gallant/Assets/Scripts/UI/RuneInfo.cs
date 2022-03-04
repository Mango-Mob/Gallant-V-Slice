using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class RuneInfo : MonoBehaviour
{
    [HideInInspector] public EffectData m_effect;
    public Image m_icon;
    public TextMeshProUGUI m_name;
    public TextMeshProUGUI m_number;

    private void Start()
    {
        m_icon.sprite = m_effect.icon;
    }
}
