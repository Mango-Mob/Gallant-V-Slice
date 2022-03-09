using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Text))]
public class UI_SliderValuePrinter : MonoBehaviour
{
    public bool m_isInteger;
    private Slider m_slider;
    void Start()
    {
        m_slider = GetComponentInChildren<Slider>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_isInteger)
            GetComponent<Text>().text = Mathf.RoundToInt(m_slider.value).ToString();
        else
            GetComponent<Text>().text = m_slider.value.ToString();
    }
}
