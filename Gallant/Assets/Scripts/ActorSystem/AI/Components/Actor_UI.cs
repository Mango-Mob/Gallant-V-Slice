using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActorSystem.AI.Components
{
    /****************
     * Actor_UI : A UI_Element container for this actor.
     * @author : Michael Jordan
     * @file : Actor_UI.cs
     * @year : 2021
     */
    [RequireComponent(typeof(Canvas))]
    public class Actor_UI : Actor_Component
    {
        public static bool m_HUDShow = true;

        protected UI_Element[] m_UIElements;
        private Canvas m_myCanvas;
        private CanvasGroup m_myGroup;
        private Animator m_animator;

        public void Awake()
        {
            m_UIElements = GetComponentsInChildren<UI_Element>();
            m_myCanvas = GetComponent<Canvas>();
            m_animator = GetComponentInChildren<Animator>();
            m_myGroup = GetComponentInChildren<CanvasGroup>();
        }

        public void Update()
        {
            m_myCanvas.enabled = m_HUDShow;
        }

        public override void SetEnabled(bool status)
        {
            if(m_myCanvas != null)
                m_myCanvas.enabled = status;
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
            Debug.LogWarning($"This actor {GetComponentInParent<Actor>().gameObject.name} does not contain an element required. (named: {name})");
            return null;
        }

        public void SetBar(string name, float value)
        {
            GetElement<UI_Bar>(name)?.SetValue(value);
        }

        public void Show()
        {
            m_animator?.SetTrigger("Reset");
        }
    }
}
