using ActorSystem.AI.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScriptDisplay : UI_Element
{
    public MonoBehaviour m_reference;
    public Text m_className;
    private Button m_button;


    protected void Awake()
    {
        m_button = GetComponentInChildren<Button>();
    }

    public void Update()
    {
        m_className.text = m_reference.GetType().Name;
        m_button.GetComponent<Image>().color = (m_reference.enabled) ? new Color(0, 156, 0) : new Color(197, 0, 0);
        m_button.GetComponentInChildren<Text>().text = (m_reference.enabled) ? "Enabled" : "Disabled";
    }

    public void Toggle()
    {
        (m_reference as Actor_Component).SetEnabled(!m_reference.enabled);
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
