using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CharSheetInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private TextMeshProUGUI m_info;
    [SerializeField] private Image m_highlight;
    public void SetInformation(string _info)
    {
        m_info.text = _info;
    }
    public void SetHighlightActive(bool _active)
    {
        m_highlight.enabled = _active;
    }
}
