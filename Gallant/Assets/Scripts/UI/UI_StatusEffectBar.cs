using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_StatusEffectBar : UI_Element
{
    [SerializeField] private Image m_ring;
    [SerializeField] private Image m_effect;

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        return;
    }

    public override void OnMouseUpEvent()
    {
        //return
    }

    public void SetValue(float start, float current)
    {
        m_ring.fillAmount = current/start;
    }

    public void SetImage(Sprite image)
    {
        if (image != null)
            m_effect.sprite = image;
        else
            Debug.LogError("UI_StatusEffectBar was given a null image.");
    }

    public void SetColor(Color newCol)
    {
        m_ring.color = newCol;
    }
}
