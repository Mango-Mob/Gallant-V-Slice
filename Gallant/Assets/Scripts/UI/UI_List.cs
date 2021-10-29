using System;
using System.Collections.Generic;
using UnityEngine;

public class UI_List : UI_Element
{
    private List<UI_Element> m_list = new List<UI_Element>();

    public UI_Element Instantiate(GameObject prefab)
    {
        if (prefab == null || prefab.GetComponent<UI_Element>() == null)
            return null;

        UI_Element addition = GameObject.Instantiate(prefab, transform).GetComponent<UI_Element>();
        m_list.Add(addition);
        return addition;
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        return;
    }

    public override void OnMouseUpEvent()
    {
        return;
    }

    /*******************
     * GetElement : A Generic function used to find a UI_Element within this container.
     * @author : Michael Jordan
     * @param : <T> typeof the UI_Element you are trying to find. Must me a child of the UI_Element class.
     * @param : (string) name of the UI_Element within the heirarchy (Default = "").
     */
    public T GetElement<T>(string name = "") where T : UI_Element
    {
        foreach (var element in m_list)
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

    /*******************
     * RemoveElement : Remove the element from this list, does NOT DESTROY the game object.
     * @author : Michael Jordan
     * @param : (UI_Element) element to remove.
     */
    public void RemoveElement(UI_Element item)
    {
        m_list.Remove(item);
    }
}
