using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * DroppedWeapon: A dropped weapon which contains information of a weapon and can be picked up by the player.
 * @author : William de Beer
 * @file : DroppedWeapon.cs
 * @year : 2021
 */
public class DroppedWeapon : MonoBehaviour
{
    public bool m_weaponMoves = true;

    public DropSpawner.DropType m_dropType;

    public WeaponData m_weaponData; // Data of contained weapon
    public AbilityData m_abilityData; // Data of contained spell
    [ColorUsage(true, true)] public Color m_defaultColor;
    public bool m_outlineEnabled = false;

    [Header("Floating Object")]
    public GameObject m_defaultModel;
    public GameObject m_weaponModel;
    public Renderer m_weaponBubble;
    public ParticleSystem m_particleSystem;

    [Header("Floating Book (Only if spell drop)")]
    public MeshRenderer m_bookCoverMesh;
    public SpriteRenderer m_bookIconSprite;

    [Header("Display")]
    public InfoDisplay m_pickupDisplay;

    private void Start()
    {
        m_pickupDisplay = GetComponentInChildren<InfoDisplay>();

        if (m_weaponData != null)
        {
            m_defaultModel.GetComponent<MeshRenderer>().enabled = false;
            m_weaponModel = Instantiate(m_weaponData.weaponModelPrefab, m_defaultModel.transform);

            m_pickupDisplay.m_weaponData = m_weaponData;

            if (m_weaponModel.transform.GetChild(0) != null)
            {
                m_weaponModel.transform.GetChild(0).localPosition = Vector3.zero;
            }
            if (m_weaponMoves)
            {
                m_weaponModel.transform.GetChild(0).rotation = Quaternion.Euler(-75, 0, 0);
            }
            else
            {
                m_weaponModel.transform.localRotation = Quaternion.Euler(100, 100, 40);
                m_weaponModel.transform.position += Vector3.up;
            }
            m_weaponModel.transform.GetChild(0).localScale *= m_weaponData.m_dropScaleMultiplier; 

            if (m_weaponData.abilityData != null)
            {
                if (m_weaponData.weaponType == Weapon.STAFF)
                {
                    MeshRenderer[] meshRenderers = m_weaponModel.GetComponentsInChildren<MeshRenderer>();
                    int meshCount = meshRenderers.Length;

                    if (meshCount > 1 && m_weaponData.abilityData != null)
                        meshRenderers[meshCount - 1].material.color = m_weaponData.abilityData.droppedEnergyColor;
                }


                Color newColor = m_weaponData.abilityData.droppedEnergyColor;

                m_weaponBubble.material.color = newColor;
                m_weaponBubble.material.SetColor("_Emission", newColor);


                newColor.a = 1.0f;
                ParticleSystem.MainModule mainModule = m_particleSystem.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(newColor);

                m_particleSystem.Play();
            }
            if (m_outlineEnabled)
            {
                foreach (var meshRenderer in m_weaponModel.GetComponentsInChildren<MeshRenderer>())
                {
                    //Outline outlineScript = meshRenderer.gameObject.AddComponent<Outline>();
                    ////outlineScript.OutlineColor = newColor;m_defaultColor
                    //outlineScript.OutlineColor = m_defaultColor;
                    //outlineScript.OutlineWidth = 2.5f;
                    //outlineScript.OutlineMode = Outline.Mode.OutlineVisible;
                }
            }
        }
        else if (m_abilityData)
        {
            m_bookCoverMesh.material.color = m_abilityData.droppedEnergyColor;
            m_bookIconSprite.sprite = m_abilityData.abilityIcon;
            m_pickupDisplay.LoadAbility(m_abilityData);
        }
        ToggleDisplay(false);
    }
    private void FixedUpdate()
    {
        if (m_weaponMoves)
        {
            m_defaultModel.transform.Rotate(new Vector3(0, 30, 0) * Time.fixedDeltaTime);
            m_defaultModel.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Sin(Time.timeSinceLevelLoad * 1.0f) * 30.0f);
        }
    }

    public void ToggleDisplay(bool _enabled)
    {
        m_pickupDisplay.gameObject.SetActive(_enabled);

        if (_enabled)
            m_pickupDisplay.ResetPickupTimer();
    }

    /*******************
     * CreateDroppedWeapon : Create a weapon drop containing weapon data
     * @author : William de Beer
     * @param : (Vector3) Spawn position, (WeaponData) Weapon data
     * @return : (GameObject) Reference to created dropped weapon.
     */
    public static GameObject CreateDroppedWeapon(Vector3 _position, WeaponData _data) 
    {
        GameObject prefab = Resources.Load<GameObject>("BaseWeaponDrop"); // Get prefab from resources.
        GameObject droppedWeapon = Instantiate(prefab, _position, Quaternion.Euler(30, 0, 0)); // Instantiate object at given position

        if (droppedWeapon.GetComponent<DroppedWeapon>())
        {
            // Set weapon data
            droppedWeapon.GetComponent<DroppedWeapon>().m_dropType = DropSpawner.DropType.WEAPON;
            droppedWeapon.GetComponent<DroppedWeapon>().m_weaponData = _data;
        }
        else
        {
            Debug.LogError("No dropped weapon component found on dropped object. Wrong prefab/resource being loaded.");
        }
        return droppedWeapon;
    }

    /*******************
     * CreateSpellUpgrade : Create a spell upgrade drop
     * @author : William de Beer
     * @param : (Vector3) Spawn position
     * @return : (GameObject) Reference to created drop.
     */
    public static GameObject CreateSpellUpgrade(Vector3 _position, AbilityData _data)
    {
        GameObject prefab = Resources.Load<GameObject>("SpellbookDrop"); // Get prefab from resources.
        GameObject droppedWeapon = Instantiate(prefab, _position, Quaternion.Euler(30, 0, 0)); // Instantiate object at given position

        if (droppedWeapon.GetComponent<DroppedWeapon>())
        {
            // Set weapon data
            droppedWeapon.GetComponent<DroppedWeapon>().m_dropType = DropSpawner.DropType.SPELLBOOK;
            droppedWeapon.GetComponent<DroppedWeapon>().m_abilityData = _data;
        }
        else
        {
            Debug.LogError("No dropped weapon component found on dropped object. Wrong prefab/resource being loaded.");
        }

        return droppedWeapon;
    }

    /*******************
     * CreateWeaponUpgrade : Create a weapon upgrade drop
     * @author : William de Beer
     * @param : (Vector3) Spawn position
     * @return : (GameObject) Reference to created drop.
     */
    public static GameObject CreateWeaponUpgrade(Vector3 _position)
    {
        GameObject prefab = Resources.Load<GameObject>("WeaponUpgradeDrop"); // Get prefab from resources.
        GameObject droppedWeapon = Instantiate(prefab, _position, Quaternion.Euler(30, 0, 0)); // Instantiate object at given position

        if (droppedWeapon.GetComponent<DroppedWeapon>())
        {
            droppedWeapon.GetComponent<DroppedWeapon>().m_dropType = DropSpawner.DropType.UPGRADE;
        }
        else
        {
            Debug.LogError("No dropped weapon component found on dropped object. Wrong prefab/resource being loaded.");
        }

        return droppedWeapon;
    }

}
