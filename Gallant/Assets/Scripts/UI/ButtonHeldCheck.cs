using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Events;
using UnityEngine.UI;

[RequireComponent(typeof(Button))]
public class ButtonHeldCheck : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IPointerExitHandler
{
    public UnityEvent m_interactFunction;
    public bool m_isButtonHeld = false;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isButtonHeld)
        {
            m_interactFunction.Invoke();
            Debug.Log("Button Held");
        }
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_isButtonHeld = false;
        Debug.Log("Button Leave");
    }
    public void OnPointerDown(PointerEventData eventData)
    {
        m_isButtonHeld = true;
        Debug.Log("Button Click");
    }
    public void OnPointerUp(PointerEventData eventData)
    {
        m_isButtonHeld = false;
        Debug.Log("Button Release");
    }
}
