using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    SWORD,
    SHIELD,
    BOOMERANG,
}
public enum Hand
{
    LEFT,
    RIGHT,
}
/****************
 * Player_Attack: Contains logic for different player weapon attacks
 * @author : William de Beer
 * @file : Player_Attack.cs
 * @year : 2021
 */
public class Player_Attack : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
    public float m_swingHeight = 1.0f;
    public LayerMask m_attackTargets;

    [Header("Hand Transforms")]
    public Transform m_leftHandTransform;
    public WeaponData m_leftWeapon;
    private GameObject m_leftWeaponObject;
    private bool m_leftWeaponInUse = false;

    public Transform m_rightHandTransform;
    public WeaponData m_rightWeapon;
    private GameObject m_rightWeaponObject;
    private bool m_rightWeaponInUse = false;

    private GameObject m_boomerangeProjectilePrefab;

    [Header("Weapon Icons")]
    [SerializeField] private UI_WeaponIcon m_leftWeaponIcon;
    [SerializeField] private UI_WeaponIcon m_rightWeaponIcon;

    private void Awake()
    {
        playerController = GetComponent<Player_Controller>();
        m_boomerangeProjectilePrefab = Resources.Load<GameObject>("Abilities/BoomerangProjectile");
    }

    /*******************
     * StartUsing : Begin the use of held weapon via animation.
     * @author : William de Beer
     * @param : (Hand) The hand of the weapon to be used
     */
    public void StartUsing(Hand _hand)
    {
        WeaponData thisData;
        string animatorTriggerName = "";

        switch (_hand)
        {
            case Hand.LEFT: // Left hand weapon
                if (m_leftWeaponInUse)
                    return;
                // Set weapon information
                thisData = m_leftWeapon;
                animatorTriggerName += "Left";
                break;
            case Hand.RIGHT: // Right hand weapon
                if (m_rightWeaponInUse)
                    return;
                // Set weapon information
                thisData = m_rightWeapon;
                animatorTriggerName += "Right";
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                return;
        }

        // If weapon is not in hand
        if (thisData == null)
            return;

        switch (thisData.weaponType)
        {
            case Weapon.SWORD: // Use sword
                animatorTriggerName += "Sword";
                break;
            case Weapon.SHIELD: // Use shield
                animatorTriggerName += "Shield";
                break;
            case Weapon.BOOMERANG: // Use boomerang
                animatorTriggerName += "Boomerang";
                break;
            default:
                Debug.Log("Weapon not implemented:" + thisData.weaponType);
                break;
        }

        playerController.animator.SetTrigger(animatorTriggerName);
    }

    /*******************
    * UseWeapon : Use a weapon's functionality from an animation event. This is why it uses a bool instead of a enum.
    * @author : William de Beer
    * @param : (bool) Is left hand, otherwise use right
    */
    public void UseWeapon(bool _left)
    {
        WeaponData thisData;
        Vector3 thisHandPosition;

        if (_left)
        {
            if (m_leftWeaponInUse)
                return;
            // Set weapon information
            thisData = m_leftWeapon;
            thisHandPosition = m_leftHandTransform.position;
        }
        else
        {
            if (m_rightWeaponInUse)
                return;
            // Set weapon information
            thisData = m_rightWeapon;
            thisHandPosition = m_rightHandTransform.position;
        }

        // If weapon is not in hand
        if (thisData == null)
            return;

        switch (thisData.weaponType)
        {
            case Weapon.SWORD: // Use sword
                WeaponAttack(thisData);
                break;
            case Weapon.SHIELD: // Use shield
                WeaponAttack(thisData);
                break;
            case Weapon.BOOMERANG: // Use boomerang
                ThrowBoomerang(thisHandPosition, thisData, _left ? Hand.LEFT : Hand.RIGHT);
                break;
            default:
                Debug.Log("Weapon not implemented:" + thisData.weaponType);
                break;
        }
    }
    /*******************
     * PickUpWeapon : Pick up a weapon and add it to hand.
     * @author : William de Beer
     * @param : (DroppedWeapon) The weapon to be picked up, (Hand) Hand to be given to.
     */
    public void PickUpWeapon(DroppedWeapon _weapon, Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                // Drop old weapon
                if (m_leftWeapon != null)
                {
                    DroppedWeapon.CreateDroppedWeapon(_weapon.transform.position, m_leftWeapon);
                    playerController.playerStats.RemoveEffect(m_leftWeapon.itemEffect); // Remove any passive effect the weapon had
                }

                // Set new weapon
                m_leftWeapon = _weapon.m_weaponData;
                ApplyWeaponData(Hand.LEFT);

                break;
            case Hand.RIGHT:
                // Drop old weapon
                if (m_rightWeapon != null)
                {
                    DroppedWeapon.CreateDroppedWeapon(_weapon.transform.position, m_rightWeapon);
                    playerController.playerStats.RemoveEffect(m_rightWeapon.itemEffect); // Remove any passive effect the weapon had
                }


                // Set new weapon
                m_rightWeapon = _weapon.m_weaponData;
                ApplyWeaponData(Hand.RIGHT);

                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                return;
        }

        Destroy(_weapon.gameObject);
    }

    public void ApplyWeaponData(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                // Delete old weapon from player
                Destroy(m_leftWeaponObject);
                if (m_leftWeapon != null)
                {
                    m_leftWeaponObject = Instantiate(m_leftWeapon.weaponModelPrefab, m_leftHandTransform);

                    if (m_leftWeaponIcon != null)
                        m_leftWeaponIcon.SetIconSprite(m_leftWeapon.weaponIcon);
                    else
                        Debug.LogWarning("Weapon icon not set");

                    playerController.playerAbilities.SetAbility(m_leftWeapon.abilityData, Hand.LEFT);
                }
                else
                {
                    if (m_leftWeaponIcon != null)
                        m_leftWeaponIcon.SetIconSprite(null);
                    playerController.playerAbilities.SetAbility(null, Hand.LEFT);
                }
                break;
            case Hand.RIGHT:
                // Delete old weapon from player
                Destroy(m_rightWeaponObject);
                if (m_rightWeapon != null)
                {
                    m_rightWeaponObject = Instantiate(m_rightWeapon.weaponModelPrefab, m_rightHandTransform);

                    if (m_rightWeaponIcon != null)
                        m_rightWeaponIcon.SetIconSprite(m_rightWeapon.weaponIcon);
                    else
                        Debug.LogWarning("Weapon icon not set");

                    playerController.playerAbilities.SetAbility(m_rightWeapon.abilityData, Hand.RIGHT);
                }
                else
                {
                    if (m_rightWeaponIcon != null)
                        m_rightWeaponIcon.SetIconSprite(null);
                    playerController.playerAbilities.SetAbility(null, Hand.RIGHT);
                }
                break;
        }
    }

    /*******************
     * SwapWeapons : Swap the weapons between hands.
     * @author : William de Beer
     */
    public void SwapWeapons()
    {
        if (m_leftWeaponInUse || m_rightWeaponInUse)
            return;

        // Store old left hand weapon for future use
        WeaponData _leftHandStore = m_leftWeapon;

        // Left hand weapon
        // Set new weapon
        m_leftWeapon = m_rightWeapon;
        ApplyWeaponData(Hand.LEFT);

        // Right hand weapon
        // Set new weapon
        m_rightWeapon = _leftHandStore;
        ApplyWeaponData(Hand.RIGHT);
    }

    #region Melee
    /*******************
     * WeaponAttack : Create sphere attack detection and damages enemies in it.
     * @author : William de Beer
     * @param : (WeaponData) 
     */
    private void WeaponAttack(WeaponData _data)
    {
        Collider[] colliders = Physics.OverlapSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * _data.hitCenterOffset, _data.hitSize, m_attackTargets);
        foreach (var collider in colliders)
        {
            Debug.Log("Hit " + collider.name + " with " + _data.weaponType + " for " + _data.m_damage);
            DamageTarget(collider.gameObject, _data.m_damage);
        }
    }

    /*******************
     * DamageTarget : Apply damage effects to enemy.
     * @author : William de Beer
     * @param : (GameObject) Target of attack, (float) Damage to deal
     */
    public void DamageTarget(GameObject _target, float _damage)
    {
        if (playerController.playerAbilities.m_leftAbility != null)
            playerController.playerAbilities.m_leftAbility.AbilityOnHitDealt(_target.gameObject, _damage);
        if (playerController.playerAbilities.m_rightAbility != null)
            playerController.playerAbilities.m_rightAbility.AbilityOnHitDealt(_target.gameObject, _damage);

        Actor actor = _target.GetComponent<Actor>();
        if (actor != null)
        {
            actor.DealDamage(_damage, transform.position);
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (playerController != null)
        {
            if (m_rightWeapon != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_rightWeapon.hitCenterOffset, m_rightWeapon.hitSize);
            }
            if (m_leftWeapon != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_leftWeapon.hitCenterOffset, m_leftWeapon.hitSize);
            }
        }
    }
    #endregion

    #region Boomerang
    /*******************
     * ThrowBoomerang : Launches projectile from specified hand.
     * @author : William de Beer
     * @param : (Vector3) Point which projectile spawns, (WeaponData), (Hand),
     */
    private void ThrowBoomerang(Vector3 _pos, WeaponData _data, Hand _hand)
    {
        // Create projectile
        GameObject projectile = Instantiate(m_boomerangeProjectilePrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        projectile.GetComponent<BoomerangProjectile>().SetReturnInfo(this, _data, _hand); // Set the information of the user to return to

        // Set activation booleans
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftWeaponObject.SetActive(false);
                m_leftWeaponInUse = true;
                break;
            case Hand.RIGHT:
                m_rightWeaponObject.SetActive(false);
                m_rightWeaponInUse = true;
                break;
            default:
                break;
        }
    }
    /*******************
     * CatchBoomerang : Returns boomerang to hand
     * @author : William de Beer
     * @param : (Hand) 
     */
    public void CatchBoomerang(Hand _hand)
    {
        // Set activation booleans
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftWeaponInUse = false;
                m_leftWeaponObject.SetActive(true);
                break;
            case Hand.RIGHT:
                m_rightWeaponInUse = false;
                m_rightWeaponObject.SetActive(true);
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                break;
        }
    }
    #endregion
}
