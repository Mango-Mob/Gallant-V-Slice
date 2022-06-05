using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/****************
 * UI_OrbResource: Set of orbs which can be used to display adrenaline resources.
 * @author : William de Beer
 * @file : UI_OrbResource.cs
 * @year : 2021
 */
public class UI_OrbCount : UI_Element
{
    [SerializeField] private Transform m_orbContainer;
    [SerializeField] private GameObject m_orbPrefab;
    [SerializeField] private List<UI_Orb> m_orbList;

    [SerializeField] private int m_value;

    private void Start()
    {
        SetValue(m_value);
    }

    private void Update()
    {
        if (m_value > m_orbList.Count)
        {
            GameObject newObject = Instantiate(m_orbPrefab, m_orbContainer);

            UI_Orb orb = newObject.GetComponent<UI_Orb>();
            orb.SetValue(1.0f);

            m_orbList.Add(orb);
        }
        if (m_value < m_orbList.Count)
        {
            UI_Orb orb = m_orbList[m_orbList.Count - 1];
            if (orb != null)
            {
                m_orbList.Remove(orb);
                Destroy(orb.gameObject);
            }
        }
    }
    /*******************
     * SetValue : Sets the value of resource fill
     * @author : William de Beer
     * @param : (float) Value to be set
     */
    public void SetValue(int _value)
    {
        m_value = _value;
    }

    public int GetValue()
    {
        return m_value;
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
