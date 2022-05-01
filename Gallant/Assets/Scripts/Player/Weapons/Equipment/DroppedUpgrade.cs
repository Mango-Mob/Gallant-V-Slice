using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/****************
 * DroppedWeapon: A dropped weapon which contains information of a weapon and can be picked up by the player.
 * @author : William de Beer
 * @file : DroppedWeapon.cs
 * @year : 2021
 */
public class DroppedUpgrade : MonoBehaviour
{
    public AbilityData m_abilityData;

    [ColorUsage(true, true)] public Color m_defaultColor;
    public bool m_outlineEnabled = false;

    [Header("Floating Object")]
    public GameObject m_defaultModel;
    public GameObject m_weaponModel;
    public Renderer m_weaponBubble;
    public ParticleSystem m_particleSystem;

    [Header("Display")]
    public InfoDisplay m_pickupDisplay;

    private void Start()
    {
        m_pickupDisplay = GetComponentInChildren<InfoDisplay>();

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
     * CreateSpellUpgrade : Create a spell upgrade drop
     * @author : William de Beer
     * @param : (Vector3) Spawn position
     * @return : (GameObject) Reference to created drop.
     */
    public static GameObject CreateSpellUpgrade(Vector3 _position)
    {
        GameObject prefab = Resources.Load<GameObject>("BaseWeaponUpgradeDrop"); // Get prefab from resources.
        GameObject droppedWeapon = Instantiate(prefab, _position, Quaternion.Euler(30, 0, 0)); // Instantiate object at given position
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
        GameObject prefab = Resources.Load<GameObject>("BaseSpellUpgradeDrop"); // Get prefab from resources.
        GameObject droppedWeapon = Instantiate(prefab, _position, Quaternion.Euler(30, 0, 0)); // Instantiate object at given position
        return droppedWeapon;
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawSphere(m_weaponBubble.transform.position, m_weaponBubble.transform.localScale.x / 2.0f);
    }
}
