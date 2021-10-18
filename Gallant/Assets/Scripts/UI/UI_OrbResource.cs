using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_OrbResource : UI_Element
{
    [Header("Orb 1")] 
    [SerializeField] private Image orbImage1;
    [SerializeField] private Image orbPulse1;
    [SerializeField] private Image orbFlicker1;

    [Header("Orb 2")]
    [SerializeField] private Image orbImage2;
    [SerializeField] private Image orbPulse2;
    [SerializeField] private Image orbFlicker2;

    [Header("Orb 3")]
    [SerializeField] private Image orbImage3;
    [SerializeField] private Image orbPulse3;
    [SerializeField] private Image orbFlicker3;

    [Range(0.0f, 3.0f)]
    [SerializeField] private float m_value;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        orbImage1.color = new Color(1, 1, 1, m_value);
        orbPulse1.enabled = orbImage1.color.a >= 1;
        orbFlicker1.enabled = orbImage1.color.a >= 1;

        orbImage2.color = new Color(1, 1, 1, m_value - 1.0f);
        orbPulse2.enabled = orbImage2.color.a >= 1;
        orbFlicker2.enabled = orbImage2.color.a >= 1;

        orbImage3.color = new Color(1, 1, 1, m_value - 2.0f);
        orbPulse3.enabled = orbImage3.color.a >= 1;
        orbFlicker3.enabled = orbImage3.color.a >= 1;
    }
    public void SetValue(float _value)
    {
        m_value = Mathf.Clamp(_value, 0.0f, 3.0f);
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
