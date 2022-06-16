using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UI_CollectionList : UI_Element
{
    public GameObject m_collectablePrefab;
    public Image m_selectFrame;

    [Header("Display Window")]
    public GameObject m_window;
    public Text m_itemTitle;
    public Text m_itemDescription;
    public Image m_itemIcon;

    public Button m_prevBtn;
    public Button m_nextBtn;

    private List<Button> m_collectionButtons = new List<Button>();
    public GameObject m_returnButton;

    private int m_currentPage;
    private CollectableData m_currentCollectable;
    // Start is called before the first frame update
    void Start()
    {
        CollectableData[] data = Resources.LoadAll<CollectableData>("Data/Collectables");

        foreach (var item in data)
        {
            GameObject button = GameObject.Instantiate(m_collectablePrefab, this.transform);
            button.GetComponent<UI_Collectable>().SetData(item);
            button.GetComponent<UI_Collectable>().SetParentList(this);
            m_collectionButtons.Add(button.GetComponentInChildren<Button>());
        }

        ShowItem(null);
    }

    // Update is called once per frame
    void Update()
    { 
        if (m_window.activeInHierarchy && InputManager.Instance.isInGamepadMode && (EventSystem.current.currentSelectedGameObject == null || EventSystem.current.currentSelectedGameObject.GetComponent<SkillButton>()))
        {
            if (m_collectionButtons.Count > 0)
            {
                EventSystem.current.SetSelectedGameObject(m_collectionButtons[0].gameObject);
            }
            else
            {
                EventSystem.current.SetSelectedGameObject(m_returnButton);
            }
        }

        if (m_window.activeInHierarchy && InputManager.Instance.isInGamepadMode)
        {
            if (InputManager.Instance.IsGamepadButtonDown(ButtonType.LB, 0))
            {
                PreviousPage();
            }
            if (InputManager.Instance.IsGamepadButtonDown(ButtonType.RB, 0))
            {
                NextPage();
            }
        }

        if(m_currentCollectable != null)
        {
            m_prevBtn.gameObject.SetActive(m_currentPage != 0 && m_currentCollectable.descriptions.Count > 1);
            m_nextBtn.gameObject.SetActive(m_currentPage != m_currentCollectable.descriptions.Count - 1 && m_currentCollectable.descriptions.Count > 1);
        }
    }

    public void ShowItem(CollectableData data, int page = 0)
    {
        if (page < 0)
            return;

        m_currentCollectable = data;

        if (data != null)
        {
            m_currentPage = page;
            m_itemTitle.text = data.itemName;
            m_itemDescription.text = data.descriptions[page];
            m_itemIcon.sprite = data.itemIcon;

            m_nextBtn.interactable = data.descriptions.Count > page;
            m_prevBtn.interactable = page > 0;

            if (PlayerPrefs.GetInt(data.collectableID, 0) > 1)
            {
                PlayerPrefs.SetInt(data.collectableID, 1);
            }
        }
        else
        {
            m_itemTitle.text = "";
            m_itemDescription.text = "";
            m_itemIcon.sprite = null;
            m_currentPage = 0;

            m_nextBtn.interactable = false;
            m_prevBtn.interactable = false;
        }
        m_window.SetActive(true);
    }

    public void PreviousPage()
    {
        if(m_currentPage > 0)
            ShowItem(m_currentCollectable, m_currentPage - 1);
    }

    public void NextPage()
    {
        if(m_currentCollectable.descriptions.Count > m_currentPage + 1)
            ShowItem(m_currentCollectable, m_currentPage + 1);
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
