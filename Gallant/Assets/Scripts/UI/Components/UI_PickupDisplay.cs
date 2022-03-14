using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;
using System.Linq;

public class UI_PickupDisplay : UI_Element
{
    [SerializeField] private Sprite m_defaultAbilityIcon;
    [SerializeField] private DroppedWeapon droppedWeapon;
    private WeaponData m_playerCurrentWeapon;

    public Text m_title;

    public Image m_weaponImageLoc;
    public Image m_abilityImageLoc;

    public Text m_levelText;

    public GameObject[] m_stars;

    public Text m_leftDamage;
    public Text m_leftSpeed;
    public Text m_leftKnockback;

    private Hand m_currentHandInUse = Hand.RIGHT;
    [Header("Hand Indicators")]
    [SerializeField] private Image m_leftPickupTimer;
    [SerializeField] private Image m_rightPickupTimer;

    [SerializeField] private TextMeshProUGUI m_currentHandText;

    [SerializeField] private GameObject[] m_rightIndicatorItems;
    [SerializeField] private GameObject[] m_leftIndicatorItems;

    [SerializeField] private GameObject[] m_controllerLabels;
    [SerializeField] private GameObject[] m_keyboardLabels;

    public HorizontalLayoutGroup m_tagRowPrefab;
    public Transform m_tagRowTransform;
    public int m_tagFontSize = 18;

    [SerializeField] private TagDetails[] m_allTags;

    private bool m_gamepadButtons = true;

    // Start is called before the first frame update
    void Start()
    {
        WeaponData thisWeapon = droppedWeapon.m_weaponData;

        if (thisWeapon.abilityData != null)
        {
            m_title.text = WeaponReward.GetPrefix(thisWeapon.abilityData.starPowerLevel) + " " + thisWeapon.weaponName;
            m_abilityImageLoc.sprite = thisWeapon.abilityData.abilityIcon;

            for (int i = 0; i < 3 - thisWeapon.abilityData.starPowerLevel; i++)
            {
                m_stars[i].SetActive(false);
            }
        }
        else
        {
            m_title.text = thisWeapon.weaponName;
            m_abilityImageLoc.sprite = m_defaultAbilityIcon;
            foreach (var item in m_stars)
            {
                item.SetActive(false);
            }
        }

        m_levelText.text = "Level: " + thisWeapon.m_level;
        m_weaponImageLoc.sprite = thisWeapon.weaponIcon;

        string taglist = WeaponData.GetTags(thisWeapon.weaponType) + ", " + thisWeapon.abilityData.tags;
        string[] tags = taglist.Split(',');
        List<TagDetails> activeTags = new List<TagDetails>();
        foreach (var tagDetail in m_allTags)
        {
            string tagString = String.Concat(tagDetail.m_tagTitle.Where(c => !Char.IsWhiteSpace(c))).ToLower();
            tagDetail.gameObject.SetActive(false);
            foreach (var weaponTag in tags)
            {
                if (tagString == String.Concat(weaponTag.Where(c => !Char.IsWhiteSpace(c))).ToLower())
                {
                    activeTags.Add(tagDetail);
                    break;
                }
            }
        }

        activeTags.Sort(TagDetails.Compare);
        LoadTags(activeTags);
    }

    private void LoadTags(List<TagDetails> tags)
    {
        //Get Total Width
        float totalWidth = 0; //right spacing
        foreach (var item in tags)
        {
            totalWidth += (item.transform as RectTransform).rect.width;
            totalWidth += 20; //left spacing + padding
        }

        //Calculate how many rows there are:
        int rows = Mathf.CeilToInt(totalWidth / (m_tagRowTransform as RectTransform).rect.width); // div maxWidth
        List<GameObject> m_rows = new List<GameObject>();
        for (int i = 0; i < rows; i++)
        {
            m_rows.Add(Instantiate(m_tagRowPrefab.gameObject, m_tagRowTransform));
            m_rows[m_rows.Count - 1].SetActive(true);
        }

        //Generate rows:
        float current = (m_tagRowTransform as RectTransform).rect.width;
        while (totalWidth > 0 && tags.Count > 0)
        {
            float width = (tags[0].transform as RectTransform).rect.width + 20;
            totalWidth -= width;
            current -= width;
            if (current < 0)
            {
                current = (m_tagRowTransform as RectTransform).rect.width;
                m_rows.RemoveAt(0);
            }
            GameObject tag = Instantiate(tags[0].gameObject, m_rows[0].transform);
            tag.SetActive(true);
            tag.GetComponentInChildren<TagDetails>().m_fontSize = m_tagFontSize;
            tags.RemoveAt(0);
        }
    }

    public void ResetPickupTimer()
    {
        m_leftPickupTimer.fillAmount = 0.0f;
        m_rightPickupTimer.fillAmount = 0.0f;
    }

    // Update is called once per frame
    void Update()
    {
        m_leftPickupTimer.fillAmount = Mathf.Clamp(m_leftPickupTimer.fillAmount - Time.deltaTime, 0.0f, 1.0f);
        m_rightPickupTimer.fillAmount = Mathf.Clamp(m_rightPickupTimer.fillAmount - Time.deltaTime, 0.0f, 1.0f);

        if (m_gamepadButtons != InputManager.Instance.isInGamepadMode)
        {
            m_gamepadButtons = InputManager.Instance.isInGamepadMode;

            if (m_gamepadButtons)
            {
                foreach (var label in m_controllerLabels)
                {
                    label.SetActive(true);
                }
                foreach (var label in m_keyboardLabels)
                {
                    label.SetActive(false);
                }
            }
            else
            {
                foreach (var label in m_keyboardLabels)
                {
                    label.SetActive(true);
                }
                foreach (var label in m_controllerLabels)
                {
                    label.SetActive(false);
                }
            }
        }    
    }

    public void InitDisplayValues(WeaponData _heldWeapon, Hand _hand)
    {
        float damage = (_heldWeapon != null) ? _heldWeapon.m_damage : 0;
        float speed = (_heldWeapon != null) ? _heldWeapon.m_speed : 0;
        float knockback = (_heldWeapon != null) ? _heldWeapon.m_knockback : 0;

        WeaponReward.UpdateCompareField(m_leftDamage, droppedWeapon.m_weaponData.m_damage, damage);
        WeaponReward.UpdateCompareField(m_leftSpeed, droppedWeapon.m_weaponData.m_speed, speed);
        WeaponReward.UpdateCompareField(m_leftKnockback, droppedWeapon.m_weaponData.m_knockback, knockback);
    }

    public bool UpdatePickupTimer(WeaponData _heldWeapon, Hand _hand)
    {
        if (m_currentHandInUse != _hand)
        {
            m_leftPickupTimer.fillAmount = 0.0f;
            m_rightPickupTimer.fillAmount = 0.0f;
            InitDisplayValues(_heldWeapon, _hand);

            bool usingLeft = _hand == Hand.LEFT;
            Color active = Color.white;
            Color deactive = Color.white;
            deactive.a = 0.5f;

            foreach (var item in m_leftIndicatorItems)
            {
                if (item.GetComponent<TextMeshProUGUI>())
                {
                    item.GetComponent<TextMeshProUGUI>().color = usingLeft ? active : deactive;
                }
                if (item.GetComponent<Image>())
                {
                    item.GetComponent<Image>().color = usingLeft ? active : deactive;
                }
            }
            foreach (var item in m_rightIndicatorItems)
            {
                if (item.GetComponent<TextMeshProUGUI>())
                {
                    item.GetComponent<TextMeshProUGUI>().color = !usingLeft ? active : deactive;
                }
                if (item.GetComponent<Image>())
                {
                    item.GetComponent<Image>().color = !usingLeft ? active : deactive;
                }
            }
        }
        m_currentHandInUse = _hand;

        Image timer = m_currentHandInUse == Hand.LEFT ? m_leftPickupTimer : m_rightPickupTimer;

        timer.fillAmount = Mathf.Clamp(timer.fillAmount + 2.0f * Time.deltaTime, 0.0f, 1.0f);
        if (timer.fillAmount >= 1.0f)
            return true;

        return false;
    }

    #region Parent override functions
    public override bool IsContainingVector(Vector2 _pos)
    {
        foreach (var element in GetComponentsInChildren<UI_Element>())
        {
            if (element == this)
                continue;

            if (element.IsContainingVector(_pos))
            {
                return true;
            }
        }
        return false;
    }

    public override void OnMouseDownEvent()
    {
        foreach (var element in GetComponentsInChildren<UI_Element>())
        {
            if (element == this)
                continue;

            element.OnMouseDownEvent();
        }
    }

    public override void OnMouseUpEvent()
    {
        foreach (var element in GetComponentsInChildren<UI_Element>())
        {
            if (element == this)
                continue;

            element.OnMouseUpEvent();
        }
    }
    #endregion
}
