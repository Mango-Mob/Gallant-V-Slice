﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_OrbResource : UI_Element
{
    [SerializeField] private UI_Orb orb1;
    [SerializeField] private UI_Orb orb2;
    [SerializeField] private UI_Orb orb3;

    [Range(0.0f, 3.0f)]
    [SerializeField] private float m_value;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
    }
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
