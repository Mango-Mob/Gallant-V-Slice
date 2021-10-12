using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_AbilityIcon : UI_Element
{
    [SerializeField] public Image m_icon;
    [SerializeField] public Image m_cooldown;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetCooldownFill(float _fill)
    {
        if (m_cooldown != null)
            m_cooldown.fillAmount = _fill;
    }

    public void SetIconSprite(Sprite _sprite)
    {
        if (m_icon != null)
            m_icon.sprite = _sprite;
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
