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
    public WeaponData m_weaponData; // Data of contained weapon

    [Header("Floating Object")]
    public GameObject m_defaultModel;
    public GameObject m_weaponModel;
    public Renderer m_weaponBubble;
    public ParticleSystem m_particleSystem;

    [Header("Display")]
    public UI_PickupDisplay m_pickupDisplay;

    private void Start()
    {
        //m_pickupDisplay = GetComponentInChildren<UI_PickupDisplay>();

        if (m_weaponData != null)
        {
            m_defaultModel.GetComponent<MeshRenderer>().enabled = false;
            m_weaponModel = Instantiate(m_weaponData.weaponModelPrefab, m_defaultModel.transform);
            
            if (m_weaponModel.transform.GetChild(0) != null)
            {
                m_weaponModel.transform.GetChild(0).localPosition = Vector3.zero;
            }
            m_weaponModel.transform.GetChild(0).rotation = Quaternion.Euler(-75, 0, 0);
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
                newColor.a = m_weaponBubble.material.color.a;

                m_weaponBubble.material.color = newColor;
                m_weaponBubble.material.SetColor("_EmissionColor", newColor);

                newColor.a = 1.0f;
                ParticleSystem.MainModule mainModule = m_particleSystem.main;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(newColor);

                m_particleSystem.Play();

                foreach (var meshRenderer in m_weaponModel.GetComponentsInChildren<MeshRenderer>())
                {
                    Outline outlineScript = meshRenderer.gameObject.AddComponent<Outline>();
                    outlineScript.OutlineColor = newColor;
                    outlineScript.OutlineWidth = 3.0f;
                    outlineScript.OutlineMode = Outline.Mode.OutlineVisible;
                }
            }
        }
        ToggleDisplay(false);
    }
    private void FixedUpdate()
    {
        m_defaultModel.transform.Rotate(new Vector3(0, 30, 0) * Time.fixedDeltaTime);
        m_defaultModel.transform.rotation = Quaternion.Euler(transform.rotation.eulerAngles.x, transform.rotation.eulerAngles.y, Mathf.Sin(Time.timeSinceLevelLoad * 1.0f) * 30.0f);
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
            droppedWeapon.GetComponent<DroppedWeapon>().m_weaponData = _data;
        }
        else
        {
            Debug.Log("No dropped weapon component found on dropped object. Wrong prefab/resource being loaded.");
        }
        return droppedWeapon;
    }
}
