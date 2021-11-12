using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Collectable : UI_Element
{
    public Image m_collectableIcon;
    public Button m_interactButton;

    private CollectableData m_data;
    private UI_CollectionList m_list;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetData(CollectableData data)
    {
        m_data = data;
        m_interactButton.interactable = PlayerPrefs.GetInt(data.collectableID, 0) == 1;
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
    }

    public override void OnMouseUpEvent()
    {

    }
}
