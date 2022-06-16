using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Collectable : UI_Element
{
    public Image m_collectableIcon;
    public Button m_interactButton;
    public Image m_newIcon;

    public bool m_unlocked = false;

    public CollectableData m_data;
    private UI_CollectionList m_list;
    // Start is called before the first frame update
    void Start()
    {
#if UNITY_EDITOR
        m_unlocked = true;
#endif
    }

    // Update is called once per frame
    void Update()
    {
        m_interactButton.interactable = m_unlocked;
        m_newIcon.gameObject.SetActive(PlayerPrefs.GetInt(m_data.collectableID, 0) == 2);
    }

    public void SetData(CollectableData data)
    {
        m_data = data;
        m_unlocked = PlayerPrefs.GetInt(data.collectableID, 0) >= 1;
        m_newIcon.gameObject.SetActive(PlayerPrefs.GetInt(m_data.collectableID, 0) == 2);
        m_collectableIcon.sprite = m_data.itemIcon;
    }
    public void SetParentList(UI_CollectionList list)
    {
        m_list = list;
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {
        m_list.ShowItem(m_data);
        m_list.m_selectFrame.enabled = true;
        m_list.m_selectFrame.transform.SetParent(transform);
        m_list.m_selectFrame.transform.position = transform.position;
    }

    public override void OnMouseUpEvent()
    {

    }
}
