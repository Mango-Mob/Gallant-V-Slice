using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/****************
 * UI_Orb: An orb to display one third of adrenaline resources.
 * @author : William de Beer
 * @file : UI_Orb.cs
 * @year : 2021
 */
public class UI_Orb : UI_Element
{
    [SerializeField] private Image orbImage;
    [SerializeField] private Image orbPulse;
    [SerializeField] private Image orbFlicker;

    private float m_value = 0.0f;

    /*******************
     * SetValue : Sets the value of resource fill
     * @author : William de Beer
     * @param : (float) Value to be set
     */
    public void SetValue(float _value)
    {
        if (m_value < 1.0f && _value >= 1.0f)
            orbFlicker.GetComponent<Animator>().SetTrigger("Flicker");

        m_value = _value;
        orbImage.color = new Color(1, 1, 1, m_value);
        orbPulse.gameObject.SetActive(m_value >= 1);
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
