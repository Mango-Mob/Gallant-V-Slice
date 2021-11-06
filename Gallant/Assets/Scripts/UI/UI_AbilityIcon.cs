﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/****************
 * UI_AbilityIcon: Description
 * @author : William de Beer
 * @file : UI_AbilityIcon.cs
 * @year : 2021
 */
public class UI_AbilityIcon : UI_Element
{
    [SerializeField] public Image m_icon;
    [SerializeField] public Image m_cooldown;
    [SerializeField] public GameObject[] m_stars;

    /*******************
     * SetCooldownFill : Sets the value of resource fill
     * @author : William de Beer
     * @param : (float) Value to be set
     */
    public void SetCooldownFill(float _fill)
    {
        m_cooldown.fillAmount = _fill;
    }

    /*******************
     * SetIconSprite : Sets the sprite of the icon
     * @author : William de Beer
     * @param : (Sprite) Sprite of new ability
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
     * SetPowerLevel : Sets power level of display
     * @author : William de Beer
     * @param : (int) Power level integer
     */
    public void SetPowerLevel(int _powerLevel)
    {
        for (int i = 0; i < 3; i++)
        {
            m_stars[i].SetActive(true);
        }
        for (int i = 0; i < 3 - _powerLevel; i++)
        {
            m_stars[i].SetActive(false);
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
