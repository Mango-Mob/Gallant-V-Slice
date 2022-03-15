using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class InfoDisplay : MonoBehaviour
{
    [Header("Current Information")]
    public bool IsADrop = false;
    public bool IsAWeapon = true;

    public WeaponData m_weaponData;
    public ItemData m_itemData;

    public Color m_greaterColor = Color.green;
    public Color m_sameColor = Color.yellow;
    public Color m_lesserColor = Color.red;
    public AudioClip m_collectAudio;

    [Header("General Information")]
    [SerializeField] private Image m_background;
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
    [SerializeField] private Text m_knockback;
    [SerializeField] private GameObject m_passiveLocation;
    [SerializeField] private Text m_passiveText;
    [SerializeField] private GameObject m_tagLoc;

    [Header("Item Information")]
    [SerializeField] private GameObject m_itemDetailsLoc;
    [SerializeField] private Image m_itemImageLoc;
    [SerializeField] private Text m_itemDescription;

    [Header("Drop Information")]
    [SerializeField] private GameObject m_leftHand;
    [SerializeField] private Text m_leftKeyboard;
    [SerializeField] private Image m_leftController;
    [SerializeField] private Image m_leftBar;

    [SerializeField] private GameObject m_rightHand;
    [SerializeField] private Text m_rightKeyboard;
    [SerializeField] private Image m_rightController;
    [SerializeField] private Image m_rightBar;

    private Player_Controller playerController;

    private Hand m_currentHandInUse = Hand.RIGHT;

    private void Start()
    {
        if (IsADrop)
        {
            m_background.color = new Color(m_background.color.r, m_background.color.g, m_background.color.b, 1f);
            m_foreground.color = new Color(m_foreground.color.r, m_foreground.color.g, m_foreground.color.b, 1f);
        }

        m_leftHand.SetActive(IsADrop);
        m_rightHand.SetActive(IsADrop);

        m_itemDetailsLoc.SetActive(!IsAWeapon);
        m_weaponDetailsLoc.SetActive(IsAWeapon);

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
            m_leftKeyboard.text = InputManager.Instance.GetBindString("Left_Pickup");
            m_rightKeyboard.gameObject.SetActive(!InputManager.Instance.isInGamepadMode);
            m_rightKeyboard.text = InputManager.Instance.GetBindString("Right_Pickup");
            m_leftController.gameObject.SetActive(InputManager.Instance.isInGamepadMode);
            m_leftController.sprite = InputManager.Instance.GetBindImage("Left_Pickup");
            m_rightController.gameObject.SetActive(InputManager.Instance.isInGamepadMode);
            m_rightController.sprite = InputManager.Instance.GetBindImage("Right_Pickup");

            m_leftBar.fillAmount = Mathf.Clamp(m_leftBar.fillAmount - 0.5f * Time.deltaTime, 0.0f, 1.0f);
            m_rightBar.fillAmount = Mathf.Clamp(m_rightBar.fillAmount - 0.5f * Time.deltaTime, 0.0f, 1.0f);
        }
    }

    public void LoadWeapon(WeaponData data)
    {
        IsAWeapon = true;

        m_weaponData = data;

        if (data == null)
            return;

        m_itemDetailsLoc.SetActive(!IsAWeapon);
        m_weaponDetailsLoc.SetActive(IsAWeapon);

        m_title.text = data.weaponName;
        m_level.text = "Level: " + (data.m_level + 1).ToString();
        m_weaponImageLoc.sprite = data.weaponIcon;
        m_abilityImageLoc.sprite = data.abilityData.abilityIcon;

        for (int i = 0; i < m_abilityStars.Length; i++)
        {
            m_abilityStars[i].gameObject.SetActive(i < data.abilityData.starPowerLevel);
        }

        playerController = GameManager.Instance.m_player.GetComponentInChildren<Player_Controller>();

        m_damage.text = data.m_damage.ToString();
        m_speed.text = data.m_speed.ToString("0.0") + $"+ ({(playerController.playerStats.m_attackSpeed - 1.0f).ToString("0.0%")})";
        m_knockback.text = data.m_knockback.ToString("0.0");

        m_passiveText.text = data.GetPassiveEffectDescription();
        m_passiveLocation.SetActive(data.itemEffect != ItemEffect.NONE);

        string taglist = WeaponData.GetTags(data.weaponType) + ", " + data.abilityData.tags;
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
            Instantiate(tag.gameObject, m_tagLoc.transform);
        }
    }

    public void LoadItem(ItemData data)
    {
        IsAWeapon = false;

        m_itemData = data;

        if (data == null)
            return;

        m_title.text = data.itemName;

        m_itemDetailsLoc.SetActive(!IsAWeapon);
        m_weaponDetailsLoc.SetActive(IsAWeapon);

        playerController = GameManager.Instance.m_player.GetComponentInChildren<Player_Controller>();

        m_level.text = "Collected: " + (playerController.playerStats.GetEffectQuantity(data.itemEffect)).ToString();
        m_itemImageLoc.sprite = data.itemIcon;
        m_itemDescription.text = data.description;
    }

    public void ResetPickupTimer()
    {
        m_leftBar.fillAmount = 0.0f;
        m_rightBar.fillAmount = 0.0f;
    }

    public void InitDisplayValues(WeaponData data, Hand loc)
    {
        if (m_weaponData == null)
            return;

        if (data == null)
        {
            CompareVariables(m_weaponData.m_damage, 0, m_damage);
            CompareVariables(m_weaponData.m_speed, 0, m_speed);
            CompareVariables(m_weaponData.m_knockback, 0, m_knockback);
            return;
        }

        CompareVariables(m_weaponData.m_damage, data.m_damage, m_damage);
        CompareVariables(m_weaponData.m_speed, data.m_speed, m_speed);
        CompareVariables(m_weaponData.m_knockback, data.m_knockback, m_knockback);
    }

    public bool UpdatePickupTimer(WeaponData _heldWeapon, Hand _hand)
    {
        if (m_currentHandInUse != _hand)
        {
            m_leftBar.fillAmount = 0.0f;
            m_rightBar.fillAmount = 0.0f;
            InitDisplayValues(_heldWeapon, _hand);
        
            bool usingLeft = _hand == Hand.LEFT;
            Color active = Color.white;
            Color deactive = Color.white;
            deactive.a = 0.5f;
        
            if(InputManager.Instance.isInGamepadMode)
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

        m_currentHandInUse = _hand;
        
        Image timer = m_currentHandInUse == Hand.LEFT ? m_leftBar : m_rightBar;
        
        timer.fillAmount = Mathf.Clamp(timer.fillAmount + 2.0f * Time.deltaTime, 0.0f, 1.0f);
        if (timer.fillAmount >= 1.0f)
            return true;

        return false;
    }
    public void GiveReward()
    {
        if(IsAWeapon)
        {
            Vector3 pos = UnityEngine.Random.insideUnitSphere;
            pos.y = 0;
            DroppedWeapon.CreateDroppedWeapon(playerController.transform.position + pos.normalized * 0.5f, m_weaponData);
            RewardManager.Instance.Hide();

            if (playerController != null)
                AudioManager.Instance?.PlayAudioTemporary(playerController.transform.position, m_collectAudio);
        }
        else
        {
            playerController?.playerStats.AddEffect(m_itemData.itemEffect);

            if (playerController != null)
                AudioManager.Instance?.PlayAudioTemporary(playerController.transform.position, m_collectAudio);
        }  
    }

    private void CompareVariables(float a, float b, Text display)
    {
        display.color = (a > b) ? m_greaterColor : (a < b) ? m_lesserColor : m_lesserColor;
    }
}
