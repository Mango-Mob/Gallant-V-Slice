using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    [Header("Current Information")]
    public bool IsADrop = false;
    public bool IsAWeapon = true;
    public bool IsEquip = false;
    public bool IsLeft = false;

    public WeaponData m_weaponData;
    public ItemData m_itemData;

    public Color m_greaterColor = Color.green;
    public Color m_sameColor = Color.yellow;
    public Color m_lesserColor = Color.red;
    public Color selectColor;
    public AudioClip m_collectAudio;
    public Button[] m_flipBtns;
    public Image[] m_controllerFlips;
    [Header("General Information")]
    [SerializeField] private Image[] m_backgrounds;
    [SerializeField] private Image m_foreground;
    [SerializeField] private Text m_title;
    [SerializeField] private Text m_level;
    [SerializeField] private TagDetails[] m_allTags;

    [Header("Weapon Information")]
    [SerializeField] private GameObject m_weaponDetailsLoc;
    [SerializeField] private Image m_weaponImageLoc;
    [SerializeField] private Image[] m_abilityStars;
    [SerializeField] private Image m_abilityImageLoc;
    [SerializeField] private Text m_damage;
    [SerializeField] private Text m_speed;
    [SerializeField] private Text m_impact;
    [SerializeField] private Text m_piercing;
    [SerializeField] private GameObject m_passiveLocation;
    [SerializeField] private Text m_passiveText;
    [SerializeField] private GameObject m_tagLoc;

    [Header("Back Information")]
    [SerializeField] private TMP_Text m_altTitle;
    [SerializeField] private Image m_altImage;
    [SerializeField] private TMP_Text m_altDescription;
    [SerializeField] private TMP_Text m_abilityTitle;
    [SerializeField] private TMP_Text m_abilityDescription;
    [SerializeField] private Image m_abilityBackImageLoc;

    [Header("Item Information")]
    [SerializeField] private GameObject m_itemDetailsLoc;
    [SerializeField] private Image m_itemImageLoc;
    [SerializeField] private Text m_itemDescription;

    [Header("Drop Information")]
    [SerializeField] private GameObject m_leftHand;
    [SerializeField] private Image m_leftKeyboard;
    [SerializeField] private Image m_leftController;
    [SerializeField] private Image m_leftBar;

    [SerializeField] private GameObject m_rightHand;
    [SerializeField] private Image m_rightKeyboard;
    [SerializeField] private Image m_rightController;
    [SerializeField] private Image m_rightBar;

    [SerializeField] private GameObject m_mainHand;
    [SerializeField] private Image m_mainKeyboard;
    [SerializeField] private Image m_mainController;
    [SerializeField] private Image m_mainBar;

    private Player_Controller playerController;
    private Animator m_animator;

    private Hand m_currentHandInUse = Hand.RIGHT;
    private bool m_selected = false;
    private void Start()
    {
        if(m_leftHand != null)
            m_leftHand.SetActive(IsADrop);
        if (m_rightHand != null)
            m_rightHand.SetActive(IsADrop);

        m_itemDetailsLoc.SetActive(!IsAWeapon);
        m_weaponDetailsLoc.SetActive(IsAWeapon);
        m_animator = GetComponent<Animator>();

        if (IsAWeapon && m_weaponData != null)
        {
            LoadWeapon(m_weaponData);
        }
        else if(m_itemData != null)
        {
            LoadItem(m_itemData);
        }
    }

    private void Update()
    {
        m_itemImageLoc.transform.parent.gameObject.SetActive(!IsAWeapon);
        m_weaponImageLoc.transform.parent.gameObject.SetActive(IsAWeapon);

        if(IsADrop)
        {
            m_leftKeyboard.gameObject.SetActive(!InputManager.Instance.isInGamepadMode);
            m_rightKeyboard.gameObject.SetActive(!InputManager.Instance.isInGamepadMode);
            m_leftController.gameObject.SetActive(InputManager.Instance.isInGamepadMode);
            m_leftController.sprite = InputManager.Instance.GetBindImage("Left_Pickup");
            m_rightController.gameObject.SetActive(InputManager.Instance.isInGamepadMode);
            m_rightController.sprite = InputManager.Instance.GetBindImage("Right_Pickup");

            m_mainKeyboard.gameObject.SetActive(!InputManager.Instance.isInGamepadMode);
            m_mainController.gameObject.SetActive(InputManager.Instance.isInGamepadMode);
            m_mainController.sprite = InputManager.Instance.GetBindImage("Right_Pickup");

            m_leftBar.fillAmount = Mathf.Clamp(m_leftBar.fillAmount - 0.5f * Time.deltaTime, 0.0f, 1.0f);
            m_rightBar.fillAmount = Mathf.Clamp(m_rightBar.fillAmount - 0.5f * Time.deltaTime, 0.0f, 1.0f);
            m_mainBar.fillAmount = Mathf.Clamp(m_mainBar.fillAmount - 0.5f * Time.deltaTime, 0.0f, 1.0f);

            m_animator?.SetBool("IsDrop", IsADrop || IsEquip);

            m_mainHand?.SetActive(m_weaponData.isTwoHanded && playerController.playerAttack.m_rightWeapon.m_weaponData.isTwoHanded);
            m_leftHand?.SetActive(!(m_weaponData.isTwoHanded && playerController.playerAttack.m_rightWeapon.m_weaponData.isTwoHanded));
            m_rightHand?.SetActive(!(m_weaponData.isTwoHanded && playerController.playerAttack.m_rightWeapon.m_weaponData.isTwoHanded));
        }

        if(m_selected || IsADrop || IsEquip)
        {
            foreach (var item in m_controllerFlips)
            {
                item.enabled = InputManager.Instance.isInGamepadMode && IsAWeapon;
            }

            foreach (var item in m_flipBtns)
            {
                item.image.enabled = !InputManager.Instance.isInGamepadMode && IsAWeapon;
            }

            if (IsAWeapon)
            {
                if (InputManager.Instance.IsGamepadButtonDown(ButtonType.RB, 0) && !IsLeft)
                {
                    Flip();
                }
                else if (InputManager.Instance.IsGamepadButtonDown(ButtonType.LB, 0) && IsLeft)
                {
                    Flip();
                }
            }
        }
    }

    public void LoadWeapon(WeaponData data)
    {
        IsAWeapon = true;
        m_animator?.SetBool("IsBack", false);
        m_weaponData = data;

        if (data == null)
        {
            gameObject.SetActive(false);
            return;
        }

        foreach (var btn in m_flipBtns)
        {
            btn.image.enabled = true;
        }

        gameObject.SetActive(true);
        m_itemDetailsLoc.SetActive(!IsAWeapon);
        m_weaponDetailsLoc.SetActive(IsAWeapon);

        m_title.text = data.weaponName;
        m_level.text = "Level: " + (data.m_level + 1).ToString();
        m_weaponImageLoc.sprite = data.weaponIcon;
        m_altTitle.SetText(data.m_altAttackName);
        m_altDescription.SetText(WeaponData.EvaluateDescription(data));

        if (m_altImage)
            m_altImage.sprite = data.altAttackIcon;

        if (data.abilityData != null)
        {
            m_abilityImageLoc.sprite = data.abilityData.abilityIcon;
            m_abilityBackImageLoc.sprite = data.abilityData.abilityIcon;
            m_abilityTitle.text = data.abilityData.abilityName;
            m_abilityDescription.SetText(AbilityData.EvaluateDescription(data.abilityData));
            m_abilityImageLoc?.gameObject.SetActive(true);
            m_abilityBackImageLoc?.gameObject.SetActive(true);
            m_abilityImageLoc.transform.parent.gameObject.SetActive(true);

            for (int i = 0; i < m_abilityStars.Length; i++)
            {
                m_abilityStars[i].gameObject.SetActive(i < data.abilityData.starPowerLevel);
            }
        }
        else
        {
            m_abilityImageLoc?.gameObject.SetActive(false);
            m_abilityBackImageLoc?.gameObject.SetActive(false);
            m_abilityImageLoc.transform.parent.gameObject.SetActive(false);
            m_abilityTitle.text = "No ability";
            m_abilityDescription.SetText("");
            for (int i = 0; i < m_abilityStars.Length; i++)
            {
                m_abilityStars[i].gameObject.SetActive(false);
            }
        }

        playerController = GameManager.Instance.m_player.GetComponentInChildren<Player_Controller>();

        m_damage.text = data.m_damage.ToString();
        m_speed.text = data.m_speed.ToString("0.00");
        m_impact.text = data.m_impact.ToString("0.00");
        m_piercing.text = data.m_piercing.ToString("0.00");
        m_passiveText.text = data.GetPassiveEffectDescription();
        m_passiveLocation.SetActive(data.itemEffect != ItemEffect.NONE);

        string taglist = WeaponData.GetTags(data.weaponType) + ", ";
        if(data.abilityData != null)
            taglist += data.abilityData.tags;

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

        for (int i = m_tagLoc.transform.childCount - 1; i >= 0; i--)
        {
            Destroy(m_tagLoc.transform.GetChild(i).gameObject);
        }

        activeTags.Sort(TagDetails.Compare);
        foreach (var tag in activeTags)
        {
            Instantiate(tag.gameObject, m_tagLoc.transform).SetActive(true);
        }
    }

    public void LoadItem(ItemData data)
    {
        IsAWeapon = false;
        m_animator?.SetBool("IsBack", false);
        m_itemData = data;

        foreach (var btn in m_flipBtns)
        {
            btn.image.enabled = false;
        }

        if (data == null)
            return;

        m_title.text = data.itemName;

        m_itemDetailsLoc.SetActive(!IsAWeapon);
        m_weaponDetailsLoc.SetActive(IsAWeapon);

        playerController = GameManager.Instance.m_player.GetComponentInChildren<Player_Controller>();

        m_level.text = "Collected: " + (playerController.playerStats.GetEffectQuantity(data.itemEffect)).ToString();
        m_itemImageLoc.sprite = data.itemIcon;
        m_itemDescription.text = data.description;

        m_passiveLocation.SetActive(false);
    }

    public void ResetPickupTimer()
    {
        if(IsADrop)
        {
            m_leftBar.fillAmount = 0.0f;
            m_rightBar.fillAmount = 0.0f;
            m_mainBar.fillAmount = 0.0f;
        }
    }

    public void InitDisplayValues(WeaponData data, Hand loc)
    {
        if (m_weaponData == null)
            return;

        if (data == null)
        {
            CompareVariables(m_weaponData.m_damage, 0, m_damage);
            CompareVariables(m_weaponData.m_speed, 0, m_speed);
            CompareVariables(m_weaponData.m_impact, 0, m_impact);
            CompareVariables(m_weaponData.m_piercing, 0, m_piercing);
            return;
        }

        CompareVariables(m_weaponData.m_damage, data.m_damage, m_damage);
        CompareVariables(m_weaponData.m_speed, data.m_speed, m_speed);
        CompareVariables(m_weaponData.m_impact, data.m_impact, m_impact);
        CompareVariables(m_weaponData.m_piercing, data.m_impact, m_piercing);
    }

    public bool UpdatePickupTimer(WeaponData _heldWeapon, Hand _hand)
    {
        m_currentHandInUse = _hand;
        
        if(m_mainHand.activeInHierarchy)
        {
            if(_hand == Hand.RIGHT)
            {
                Image timer = m_mainBar;

                InitDisplayValues(playerController.playerAttack.m_rightWeapon.m_weaponData, _hand);

                Color active = Color.white;
                m_mainController.color = active;

                timer.fillAmount = Mathf.Clamp(timer.fillAmount + 2.0f * Time.deltaTime, 0.0f, 1.0f);
                if (timer.fillAmount >= 1.0f)
                    return true;
            }
        }
        else
        {
            if (m_currentHandInUse != _hand)
            {
                m_leftBar.fillAmount = 0.0f;
                m_rightBar.fillAmount = 0.0f;
                m_mainBar.fillAmount = 0.0f;
                InitDisplayValues(_heldWeapon, _hand);

                bool usingLeft = _hand == Hand.LEFT;
                Color active = Color.white;
                Color deactive = Color.white;
                deactive.a = 0.5f;

                if (InputManager.Instance.isInGamepadMode)
                {
                    m_leftController.color = usingLeft ? active : deactive;
                    m_rightController.color = !usingLeft ? active : deactive;
                }
                else
                {
                    m_leftKeyboard.color = usingLeft ? active : deactive;
                    m_rightKeyboard.color = !usingLeft ? active : deactive;
                }
            }

            Image timer = m_currentHandInUse == Hand.LEFT ? m_leftBar : m_rightBar;

            timer.fillAmount = Mathf.Clamp(timer.fillAmount + 2.0f * Time.deltaTime, 0.0f, 1.0f);
            if (timer.fillAmount >= 1.0f)
                return true;
        }

        return false;
    }

    public void Select(bool status)
    {
        foreach (var item in m_backgrounds)
        {
            item.color = (status) ? selectColor : Color.white;
        }
        m_selected = status;
    }

    public void Hover(bool status)
    {
        foreach (var item in m_backgrounds)
        {
            item.color = (status) ? selectColor : (m_selected) ? selectColor : Color.white;
        }
    }

    public void GiveReward()
    {
        if(IsAWeapon)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere;
            pos.y = 0;
            DroppedWeapon.CreateDroppedWeapon(playerController.transform.position + pos.normalized * 0.5f, m_weaponData);

            if (playerController != null)
                AudioManager.Instance?.PlayAudioTemporary(playerController.transform.position, m_collectAudio);
        }
        else
        {
            playerController?.playerStats.AddEffect(m_itemData.itemEffect);

            if (playerController != null)
                AudioManager.Instance?.PlayAudioTemporary(playerController.transform.position, m_collectAudio);
        }
        RewardManager.Instance.Hide();
    }

    private void CompareVariables(float a, float b, Text display)
    {
        display.color = (a > b) ? m_greaterColor : (a < b) ? m_lesserColor : m_lesserColor;
    }

    public void Flip()
    {
        m_animator.SetBool("IsBack", !m_animator.GetBool("IsBack"));
    }
}
