using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class CharSheetInfo : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI m_title;
    [SerializeField] private TextMeshProUGUI m_info;
    public void SetInformation(string _info)
    {
        m_info.text = _info;
    }
}
