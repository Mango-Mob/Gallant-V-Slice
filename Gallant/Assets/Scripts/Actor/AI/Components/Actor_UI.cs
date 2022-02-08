using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Actor.AI.Components
{
    /****************
     * Actor_UI : A UI_Element container for this actor.
     * @author : Michael Jordan
     * @file : Actor_UI.cs
     * @year : 2021
     */
    public class Actor_UI : MonoBehaviour
    {
        protected UI_Element[] m_UIElements;

        public void Awake()
        {
            m_UIElements = GetComponentsInChildren<UI_Element>();
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
    }
}
