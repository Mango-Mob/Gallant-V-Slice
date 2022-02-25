using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Pause_Tome : MonoBehaviour
{
    public Image m_image;
    public Text m_amountText;

    public void SetTome(Sprite _effect)
    {
        if (m_image != null)
            m_image.sprite = _effect;
    }

    public void SetAmount(int _amount)
    {
        if(m_amountText != null)
            m_amountText.text = _amount.ToString();
    }
}
