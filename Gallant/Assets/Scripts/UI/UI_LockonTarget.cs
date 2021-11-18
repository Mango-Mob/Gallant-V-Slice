using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_LockonTarget : UI_Element
{
    private Image m_image;
    private WorldToCanvas m_worldToCanvas;

    private void Awake()
    {
        m_image = GetComponent<Image>();
        m_worldToCanvas = GetComponent<WorldToCanvas>();
    }
    public void UpdateTarget(GameObject _target)
    {
        if (_target != null)
        {
            m_worldToCanvas.m_anchorTransform = _target.transform;
            m_image.enabled = true;
        }
        else
        {
            m_worldToCanvas.m_anchorTransform = null;
            m_image.enabled = false;
        }
        m_worldToCanvas.ForceUpdate();
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
