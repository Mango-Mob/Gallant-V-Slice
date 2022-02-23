using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UI_Text : UI_Element
{
    public string m_myText { get { return m_text.text; } set { m_text.text = value; } }

    private Text m_text { get { return GetComponent<Text>(); } }

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
