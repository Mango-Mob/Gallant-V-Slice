using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/****************
 * UI_OrbResource: Set of orbs which can be used to display adrenaline resources.
 * @author : William de Beer
 * @file : UI_OrbResource.cs
 * @year : 2021
 */
public class UI_OrbResource : UI_Element
{
    [SerializeField] private UI_Orb orb1;
    [SerializeField] private UI_Orb orb2;
    [SerializeField] private UI_Orb orb3;

    [Range(0.0f, 3.0f)]
    [SerializeField] private float m_value;

    /*******************
     * SetValue : Sets the value of resource fill
     * @author : William de Beer
     * @param : (float) Value to be set
     */
    public void SetValue(float _value)
    {
        m_value = Mathf.Clamp(_value, 0.0f, 3.0f);
        orb1.SetValue(m_value);
        orb2.SetValue(m_value - 1.0f);
        orb3.SetValue(m_value - 2.0f);
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
