using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/****************
 * UI_Bar: Filled image that can be used to display a stat.
 * @author : William de Beer
 * @file : UI_Bar.cs
 * @year : 2021
 */
public class UI_Bar : UI_Element
{
    [Range(0.0f, 1.0f)]
    [SerializeField] private float m_value;
    [SerializeField] private Image m_barImage;
    private Color m_startColor;
    [SerializeField] private float m_chaseSpeed = 0.5f;

    [SerializeField] bool m_hasChasingBar;
    [SerializeField] private Image m_chasingBarImage;

    private void Start()
    {
        m_startColor = m_barImage.color;
    }
    // Update is called once per frame
    void Update()
    {
        m_barImage.fillAmount = m_value;

        if (m_chasingBarImage != null)
        {
            if (m_chasingBarImage.fillAmount < m_value)
                m_chasingBarImage.fillAmount = m_value;
            else if (m_chasingBarImage.fillAmount > m_value)
                m_chasingBarImage.fillAmount -= m_chaseSpeed * Time.deltaTime;
        }
    }

    /*******************
     * SetValue : Sets the value of resource fill
     * @author : William de Beer
     * @param : (float) Value to be set
     */
    public void SetValue(float _value)
    {
        m_value = Mathf.Clamp(_value, 0.0f, 1.0f);
    }

    public void InstantUpdate(float _value)
    {
        m_value = Mathf.Clamp(_value, 0.0f, 1.0f);

        m_barImage.fillAmount = m_value;

        if (m_chasingBarImage != null)
        {
            m_chasingBarImage.fillAmount = m_value;
        }
    }

    public float GetValue()
    {
        return m_value;
    }

    public void SetEnabled(bool _active)
    {
        Color newColor = m_startColor;
        newColor *= _active ? 1.0f : 0.5f;
        newColor.a = m_startColor.a;

        m_barImage.color = newColor;
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
