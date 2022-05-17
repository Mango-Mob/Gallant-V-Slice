using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HUDManager : Singleton<HUDManager>
{
    [Header("UI Objects")]
    public UI_Element[] m_UIElements;

    private void Start()
    {
        gameObject.name = $"HUDManager ({gameObject.name})";
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 mousePos = InputManager.Instance.GetMousePositionInScreen();
        if (InputManager.Instance.IsMouseButtonDown(MouseButton.LEFT))
        {
            foreach (var elements in m_UIElements)
            {
                if(elements.GetComponent<UI_Element>() != null 
                    && elements.GetComponent<UI_Element>().IsContainingVector(mousePos))
                {
                    elements.GetComponent<UI_Element>().OnMouseDownEvent();
                }
            }
        }
        else if(InputManager.Instance.IsMouseButtonUp(MouseButton.LEFT))
        {
            foreach (var elements in m_UIElements)
            {
                 elements.GetComponent<UI_Element>().OnMouseUpEvent();
            }
        }
    }

    public UI_Element GetElementUnderMouse()
    {
        Vector2 mousePos = InputManager.Instance.GetMousePositionInScreen();
        foreach (var elements in m_UIElements)
        {
            if (elements.GetComponent<UI_Element>() != null
                && elements.GetComponent<UI_Element>().IsContainingVector(mousePos))
            {
                return elements.GetComponent<UI_Element>();
            }
        }
        return null;
    }

    /*******************
     * GetElement : A Generic function used to find a UI_Element within this container.
     * @author : Michael Jordan
     * @param : <T> typeof the UI_Element you are trying to find. Must me a child of the UI_Element class.
     * @param : (string) name of the UI_Element within the heirarchy (Default = "").
     */
    public T GetElement<T>(string name = "") where T : UI_Element
    {
        foreach (var element in m_UIElements)
        {
            T item = element as T;
            if (item != null && (item.name == name || name == ""))
            {
                return item;
            }

            //Check inside panel
            UI_Panel panel = element as UI_Panel;
            if (panel != null)
            {
                T subItem = panel.GetElement<T>(name);
                if (subItem != null)
                    return subItem;
            }
        }
        return null;
    }

    public UI_DamageDisplay GetDamageDisplay()
    {
        foreach (var item in m_UIElements)
        {
            if (item.GetType() == typeof(UI_DamageDisplay))
                return item as UI_DamageDisplay;
        }
        return null;
    }
}
