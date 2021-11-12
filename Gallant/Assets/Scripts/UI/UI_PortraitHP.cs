using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_PortraitHP : UI_Element
{
    [SerializeField] private Image m_face;
    [SerializeField] private Sprite m_portraitSprite1;
    [SerializeField] private Sprite m_portraitSprite2;
    [SerializeField] private Sprite m_portraitSprite3;
    [SerializeField] private float m_deathFadeDuration = 1.0f;
    private bool m_dead = false;
    private void Update()
    {
        if (m_dead)
        {
            Color newColor = m_face.color;
            newColor.a = Mathf.Max(0, newColor.a - Time.deltaTime * m_deathFadeDuration);
            m_face.color = newColor;
        }
    }
    public void UpdatePortrait(float _health)
    {
        if (_health <= 0.0f)
        {
            m_dead = true;
        }
        else if (_health < 0.3f)
        {
            m_face.sprite = m_portraitSprite3;
        }
        else if (_health < 0.6f)
        {
            m_face.sprite = m_portraitSprite2;
        }
        else
        {
            m_face.sprite = m_portraitSprite1;
        }
    }

    #region Parent override functions
    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        //Do Nothing
    }

    public override void OnMouseUpEvent()
    {
        //Do Nothing
    }
    #endregion
}
