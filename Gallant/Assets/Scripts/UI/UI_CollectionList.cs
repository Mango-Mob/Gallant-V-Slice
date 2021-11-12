using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_CollectionList : UI_Element
{
    public GameObject m_collectablePrefab;

    [Header("Display Window")]
    public GameObject m_window;
    public Text m_itemTitle;
    public Text m_itemDescription;
    public Image m_itemIcon;

    // Start is called before the first frame update
    void Start()
    {
        CollectableData[] data = Resources.LoadAll<CollectableData>("Data/Collectables");

        foreach (var item in data)
        {
            GameObject button = GameObject.Instantiate(m_collectablePrefab, this.transform);
            button.GetComponent<UI_Collectable>().SetData(item);
            button.GetComponent<UI_Collectable>().SetParentList(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ShowItem(CollectableData data)
    {
        m_itemTitle.text = data.itemName;
        m_itemDescription.text = data.description;
        m_itemIcon.sprite = data.itemIcon;
        m_window.SetActive(true);
    }

    public override bool IsContainingVector(Vector2 _pos)
    {
        return false;
    }

    public override void OnMouseDownEvent()
    {

    }

    public override void OnMouseUpEvent()
    {

    }

    public void Hide()
    {
        m_window.SetActive(false);
    }
}
