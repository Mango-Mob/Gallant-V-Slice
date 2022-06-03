using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
/****************
 * UI_AbilityIcon: Description
 * @author : William de Beer
 * @file : UI_AbilityIcon.cs
 * @year : 2021
 */

public enum FrameType
{
    NONE,
    PASSIVE,
    ACTIVE,
}

public class UI_AbilityIcon : UI_Element
{
    [Header("Settings")]
    [SerializeField] private float m_vibeSpeed = 1.0f;

    [Space(15.0f)]
    [SerializeField] private float m_fillOpacityRange = 0.05f;
    private float m_fillStartOpacity = 0.0f;

    [Space(15.0f)]
    [SerializeField] private float m_lineThickness = 0.05f;
    [SerializeField] private float m_lineOpacityRange = 0.05f;
    private float m_lineStartOpacity = 0.0f;

    [Header("Components")]
    [SerializeField] private Image m_icon;
    [SerializeField] private Image m_disabled;
    [SerializeField] private Image m_cooldown;
    [SerializeField] private Image m_cooldownShad;
    [SerializeField] private GameObject[] m_stars;
    [SerializeField] private TextMeshProUGUI m_text;
    [SerializeField] private TextMeshProUGUI m_textshad;
    [SerializeField] private GameObject[] m_bindButtonObjects;
    [SerializeField] private Animator m_animator;

    [Header("Frames")]
    [SerializeField] private GameObject m_activeFrame;
    [SerializeField] private GameObject m_passiveFrame;
    bool m_isCharged = true;
    bool m_isBindActive = false;

    private void Awake()
    {
        m_fillStartOpacity = m_cooldown.color.a - m_fillOpacityRange;
        m_lineStartOpacity = m_cooldownShad.color.a - m_fillOpacityRange;
    }
    /*******************
     * SetCooldown : Sets the value of resource fill and text
     * @author : William de Beer
     * @param : (float) Fill value for bar, (float) Max cooldown for ability.
     */
    public void SetCooldown(float _fill, float _maxCooldown)
    {
        // Hide/Show bindings
        foreach (var item in m_bindButtonObjects)
        {
            item.SetActive(m_isBindActive && _fill >= 1.0f);
        }

        m_cooldown.fillAmount = _fill;
        m_cooldownShad.fillAmount = _fill + m_lineThickness;
        m_cooldown.enabled = _fill < 1.0f;
        m_cooldownShad.enabled = _fill < 1.0f;

        m_text.text = (_fill * _maxCooldown).ToString("0.0");
        m_textshad.text = (_fill * _maxCooldown).ToString("0.0");

        if (_fill >= 1.0f)
        {
            m_icon.color = Color.white;
            if (!m_isCharged)
            {
                m_isCharged = true;
                m_animator.SetTrigger("Bounce");
            }
        }
        else
        {
            Color barColor = m_cooldown.color;
            barColor.a = m_fillStartOpacity + Mathf.Sin(Time.realtimeSinceStartup * m_vibeSpeed) * m_fillOpacityRange * 0.5f;
            m_cooldown.color = barColor;

            barColor = m_cooldownShad.color;
            barColor.a = m_lineStartOpacity + Mathf.Sin(Time.realtimeSinceStartup * m_vibeSpeed) * m_lineOpacityRange * 0.5f;
            m_cooldownShad.color = barColor;

            m_icon.color = Color.grey;
            m_isCharged = false;
        }

        m_text.enabled = _fill > 0.0f;
        m_textshad.enabled = _fill > 0.0f;
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

    public void SetFrame(FrameType _frameType)
    {
        switch (_frameType)
        {
            case FrameType.NONE:
                m_activeFrame.SetActive(false);
                m_passiveFrame.SetActive(false);
                break;
            case FrameType.PASSIVE:
                m_activeFrame.SetActive(false);
                m_passiveFrame.SetActive(true);
                break;
            case FrameType.ACTIVE:
                m_activeFrame.SetActive(true);
                m_passiveFrame.SetActive(false);
                break;
        }
    }

    public void SetBindDisplayActive(bool _active)
    {
        m_isBindActive = _active;
        foreach (var item in m_bindButtonObjects)
        {
            item.SetActive(_active);
        }
    }

    public void SetDisabledState(bool _disabled)
    {
        if (m_disabled)
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
