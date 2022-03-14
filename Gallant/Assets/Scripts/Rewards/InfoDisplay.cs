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

    [Header("General Information")]
    [SerializeField] private Image m_background;
    [SerializeField] private Image m_foreground;
    [SerializeField] private Text m_title;
    [SerializeField] private Text m_level;
    [SerializeField] private TagDetails[] m_allTags;

    [Header("Weapon Information")]
    [SerializeField] private Image m_weaponImageLoc;
    [SerializeField] private Image m_abilityImageLoc;
    [SerializeField] private Text m_damage;
    [SerializeField] private Text m_speed;
    [SerializeField] private Text m_knockback;
    [SerializeField] private GameObject m_passiveLocation;
    [SerializeField] private Text m_passiveText;
    [SerializeField] private GameObject m_tagLoc;

    [Header("Item Information")]
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

    private void Awake()
    {
        //m_defaultImage = Resources.Load<Sprite>("DefaultIcon");
        //playerController = FindObjectOfType<Player_Controller>();
    }
    private void Start()
    {
        playerController = GameManager.Instance.m_player.GetComponentInChildren<Player_Controller>();
    }
    private void Update()
    {
        //if (m_activeWeapon)
        //{
        //    m_levelText.text = "Level: " + m_activeWeapon.m_level;
        //    m_damage.text = m_activeWeapon.m_damage.ToString();
        //
        //    m_speed.text = m_activeWeapon.m_speed.ToString("0.0");
        //
        //    if (playerController.playerStats.m_attackSpeed != 1.0f)
        //        m_speed.text += $" (+{((playerController.playerStats.m_attackSpeed - 1.0f) * m_activeWeapon.m_speed).ToString("0.0")})";
        //
        //    m_knockback.text = m_activeWeapon.m_knockback.ToString("0.0");
        //
        //
        //    m_weaponImageLoc.sprite = m_activeWeapon.weaponIcon;
        //}

    }

    public void SetWeapon(WeaponData data)
    {
        //m_activeWeapon = data;
        //
        //for (int i = m_showingTagRows.Count - 1; i >= 0; i--)
        //{
        //    Destroy(m_showingTagRows[i]);
        //}
        //m_showingTagRows.Clear();
        //
        //for (int i = 0; i < 3; i++)
        //{
        //    m_stars[i].SetActive(true);
        //}
        //
        //if (data == null)
        //{
        //    m_title.text = "None";
        //    m_weaponImageLoc.sprite = m_defaultImage;
        //    m_abilityImageLoc.sprite = m_defaultImage;
        //    foreach (var item in m_stars)
        //    {
        //        item.SetActive(false);
        //    }
        //    m_levelText.text = "";
        //
        //    m_damage.text = "-";
        //    m_speed.text = "-";
        //    m_knockback.text = "-";
        //}
        //else
        //{
        //    if (data.abilityData != null)
        //    {
        //        m_title.text = WeaponReward.GetPrefix(data.abilityData.starPowerLevel) + " " + data.weaponName;
        //        m_abilityImageLoc.sprite = data.abilityData.abilityIcon;
        //
        //        for (int i = 0; i < 3 - data.abilityData.starPowerLevel; i++)
        //        {
        //            m_stars[i].SetActive(false);
        //        }
        //    }
        //    else
        //    {
        //        m_title.text = data.weaponName;
        //        m_abilityImageLoc.sprite = m_defaultImage;
        //        foreach (var item in m_stars)
        //        {
        //            item.SetActive(false);
        //        }
        //    }
        //
        //    m_levelText.text = "Level: " + data.m_level;
        //    m_damage.text = data.m_damage.ToString();
        //    m_speed.text = data.m_speed.ToString("0.0") + $"+ ({(playerController.playerStats.m_attackSpeed - 1.0f).ToString("0.0%")})";
        //    m_knockback.text = data.m_knockback.ToString("0.0");
        //
        //    m_weaponImageLoc.sprite = data.weaponIcon;
        //
        //    string taglist = WeaponData.GetTags(data.weaponType) + ", " + data.abilityData.tags;
        //    string[] tags = taglist.Split(',');
        //    List<TagDetails> activeTags = new List<TagDetails>();
        //    foreach (var tagDetail in m_allTags)
        //    {
        //        string tagString = String.Concat(tagDetail.m_tagTitle.Where(c => !Char.IsWhiteSpace(c))).ToLower();
        //        tagDetail.gameObject.SetActive(false);
        //        foreach (var weaponTag in tags)
        //        {
        //            if (tagString == String.Concat(weaponTag.Where(c => !Char.IsWhiteSpace(c))).ToLower())
        //            {
        //                activeTags.Add(tagDetail);
        //                break;
        //            }
        //        }
        //    }
        //    activeTags.Sort(TagDetails.Compare);
        //    LoadTags(activeTags);
        //    m_passiveText.text = data.GetPassiveEffectDescription();
        //    m_passiveObject.SetActive(m_passiveText.text == null);
        //}
    }

    private void LoadTags(List<TagDetails> tags)
    {
        //Get Total Width
        //float totalWidth = 0; //right spacing
        //foreach (var item in tags)
        //{
        //    totalWidth += (item.transform as RectTransform).rect.width;
        //    totalWidth += 20; //left spacing + padding
        //}
        //
        ////Calculate how many rows there are:
        //int rows = Mathf.CeilToInt(totalWidth / (m_tagRowTransform as RectTransform).rect.width); // div maxWidth
        //List<GameObject> m_rows = new List<GameObject>();
        //for (int i = 0; i < rows; i++)
        //{
        //    m_rows.Add(Instantiate(m_tagRowPrefab.gameObject, m_tagRowTransform));
        //    m_rows[m_rows.Count - 1].SetActive(true);
        //}
        //
        ////Generate rows:
        //float current = (m_tagRowTransform as RectTransform).rect.width;
        //while (totalWidth > 0 && tags.Count > 0)
        //{
        //    float width = (tags[0].transform as RectTransform).rect.width + 20;
        //    totalWidth -= width;
        //    current -= width;
        //    if (current < 0)
        //    {
        //        current = (m_tagRowTransform as RectTransform).rect.width;
        //        m_showingTagRows.Add(m_rows[0]);
        //        m_rows.RemoveAt(0);
        //    }
        //    Instantiate(tags[0].gameObject, m_rows[0].transform).SetActive(true);
        //    tags.RemoveAt(0);
        //}
        //
        //if(m_rows.Count > 0)
        //{
        //    m_showingTagRows.Add(m_rows[0]);
        //}
    }
}
