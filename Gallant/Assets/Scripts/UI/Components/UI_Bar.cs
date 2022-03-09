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

    // Update is called once per frame
    void Update()
    {
        m_barImage.fillAmount = m_value;
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
    public float GetValue()
    {
        return m_value;
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
