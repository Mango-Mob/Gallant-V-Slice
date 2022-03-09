using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/****************
 * UI_WeaponIcon : Icon that displays weapon held.
 * @author : William de Beer
 * @file : UI_WeaponIcon.cs
 * @year : 2021
 */
public class UI_WeaponIcon : UI_Element
{
    [SerializeField] public Image m_icon;
    [SerializeField] public Image m_disabled;

    /*******************
     * SetIconSprite : Sets the sprite of the icon
     * @author : William de Beer
     * @param : (Sprite) Sprite of new weapon
     */
    public void SetIconSprite(Sprite _sprite)
    {
        if (_sprite != null)
        {
            m_icon.sprite = _sprite;
            m_icon.enabled = true;
        }
        else
            m_icon.enabled = false;
    }

    /*******************
     * SetDisabledState : Sets the disabled state
     * @author : William de Beer
     * @param : (bool) Disabled state
     */
    public void SetDisabledState(bool _disabled)
    {
        m_disabled.enabled = _disabled;
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
