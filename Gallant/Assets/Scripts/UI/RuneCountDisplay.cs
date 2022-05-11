using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using TMPro;

public class RuneCountDisplay : MonoBehaviour
{
    private static Player_Controller playerController;
    public ItemData m_runeItem;
    public Image m_icon;
    [SerializeField] private RectTransform m_rectTransform;

    [Header("Tally Mode")]
    public Image[] m_tallyMarks;

    [Header("Number Mode")]
    public TextMeshProUGUI m_numberValue;
    public TextMeshProUGUI m_fractionNumberValue;
    public Image m_runeFill;

    [Header("Highlight Elements")]
    public CharSheetInfo[] m_hoverHighlightElements;

    // Start is called before the first frame update
    void Start()
    {
        if (playerController == null)
            playerController = FindObjectOfType<Player_Controller>();

        if (m_rectTransform)
            m_rectTransform = GetComponent<RectTransform>();
    }

    // Update is called once per frame
    void Update()
    {
        int effectCount = playerController.playerStats.GetEffectQuantity(m_runeItem.itemEffect);
        for (int i = 0; i < m_tallyMarks.Length; i++)
        {
            m_tallyMarks[i].enabled = i < effectCount;
        }

        m_numberValue.text = effectCount.ToString();
        m_fractionNumberValue.text = $"{effectCount}/10";
        m_runeFill.fillAmount = effectCount / 10.0f;
    }
    private void OnValidate()
    {
        if (m_runeItem != null)
        {
            name = $"{m_runeItem.name}Display";

            if (m_icon != null)
                m_icon.sprite = m_runeItem.itemIcon;
        }
    }
    public bool CheckJoystickCursorInRange(Vector3 _position)
    {
        return (_position.x < (m_rectTransform.position.x + 100)
            && _position.x > (m_rectTransform.position.x - 150)
            && _position.y < (m_rectTransform.position.y + m_rectTransform.rect.yMax)
            && _position.y > (m_rectTransform.position.y + m_rectTransform.rect.yMin));
    }
    public void SetHighlightElementsActive(bool _active)
    {
        foreach (var elements in m_hoverHighlightElements)
        {
            elements.SetHighlightActive(_active);
        }
    }
}
