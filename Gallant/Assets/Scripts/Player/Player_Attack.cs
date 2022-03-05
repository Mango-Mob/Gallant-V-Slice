﻿using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    SWORD,
    SPEAR,
    BOOMERANG,
    SHIELD,
    CROSSBOW,
    BRICK,
    AXE,
    STAFF,
    GREATSWORD,
    BOW,
}
public enum Hand
{
    LEFT,
    RIGHT,
    NONE,
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
    private bool m_attackedThisFrame = false;

    [Header("Hand Transforms")]
    public Transform m_leftHandTransform;
    public WeaponData m_leftWeaponData;
    public WeaponBase m_leftWeapon { private set; get; }
    private bool m_leftWeaponInUse = false;

    public Transform m_rightHandTransform;
    public WeaponData m_rightWeaponData;
    public WeaponBase m_rightWeapon { private set; get; }
    private bool m_rightWeaponInUse = false;

    [Header("Weapon Icons")]
    private UI_WeaponIcon m_leftWeaponIcon;
    private UI_WeaponIcon m_rightWeaponIcon;

    [Header("Held Gameobjects")]
    private GameObject m_shieldBlockPrefab;
    private GameObject m_rightHeldObjectInstance;
    private GameObject m_leftHeldObjectInstance;

    [Header("Blocking")]
    public float m_blockingAngle = 45.0f;
    public bool m_isBlocking { private set; get; } = false;

    private void Awake()
    {
        playerController = GetComponent<Player_Controller>();
        //m_boomerangeProjectilePrefab = Resources.Load<GameObject>("Abilities/BoomerangProjectile");

        m_leftWeaponIcon = HUDManager.Instance.GetElement<UI_WeaponIcon>("WeaponL");
        m_rightWeaponIcon = HUDManager.Instance.GetElement<UI_WeaponIcon>("WeaponR");
    }

    private void Start()
    {
    }

    private void Update()
    {
        m_attackedThisFrame = false;
    }

    /*******************
     * StartUsing : Begin the use of held weapon via animation.
     * @author : William de Beer
     * @param : (Hand) The hand of the weapon to be used
     */
    public void StartUsing(Hand _hand)
    {
        WeaponBase thisWeapon;
        string animatorTriggerName = "";

        switch (_hand)
        {
            case Hand.LEFT: // Left hand weapon
                if (m_leftWeaponInUse)
                    return;
                // Set weapon information
                thisWeapon = m_leftWeapon;
                animatorTriggerName += "Left";
                break;
            case Hand.RIGHT: // Right hand weapon
                if (m_rightWeaponInUse)
                    return;
                // Set weapon information
                thisWeapon = m_rightWeapon;
                animatorTriggerName += "Right";
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                return;
        }

        // If weapon is not in hand
        if (thisWeapon == null)
            return;

        if (thisWeapon.m_isInUse)
            return;

        animatorTriggerName += " " + thisWeapon.GetWeaponName();

        playerController.playerAudioAgent.PlayWeaponSwing();
        //playerController.animator.SetBool(animatorTriggerName, true);
        playerController.playerCombatAnimator.PlayAttack(animatorTriggerName);
        //playerController.animator.CrossFade(animatorTriggerName, 0.1f);
    }

    /*******************
    * UseWeapon : Use a weapon's functionality from an animation event. This is why it uses a bool instead of a enum.
    * @author : William de Beer
    * @param : (bool) Is left hand, otherwise use right
    */
    public void UseWeapon(bool _left)
    {
        if (m_attackedThisFrame)
            return;

        m_attackedThisFrame = true;

        if (_left)
        {
            if (m_leftWeapon)
                m_leftWeapon.TriggerWeapon();
        }
        else
        {
            if (m_rightWeapon)
                m_rightWeapon.TriggerWeapon();
        }

        //WeaponData thisData;
        //Vector3 thisHandPosition;

        //if (_left)
        //{
        //    if (m_leftWeaponInUse)
        //        return;
        //    // Set weapon information
        //    thisData = m_leftWeaponData;
        //    thisHandPosition = m_leftHandTransform.position;
        //}
        //else
        //{
        //    if (m_rightWeaponInUse)
        //        return;
        //    // Set weapon information
        //    thisData = m_rightWeaponData;
        //    thisHandPosition = m_rightHandTransform.position;
        //}

        //// If weapon is not in hand
        //if (thisData == null)
        //    return;

        //switch (thisData.weaponType)
        //{
        //    case Weapon.SWORD: // Use sword
        //        WeaponAttack(thisData, transform.position);
        //        break;
        //    case Weapon.SHIELD: // Use shield
        //        WeaponAttack(thisData, transform.position);
        //        BeginBlock(_left ? Hand.LEFT : Hand.RIGHT);
        //        break;
        //    case Weapon.BOOMERANG: // Use boomerang
        //        ThrowBoomerang(thisHandPosition, thisData, _left ? Hand.LEFT : Hand.RIGHT);
        //        break;
        //    default:
        //        Debug.Log("Weapon not implemented:" + thisData.weaponType);
        //        break;
        //}


        //playerController.animator.SetBool("LeftShield", false);
        //playerController.animator.SetBool("RightShield", false);
        //playerController.animator.SetBool("LeftSword", false);
        //playerController.animator.SetBool("RightSword", false);
        //playerController.animator.SetBool("LeftBoomerang", false);
        //playerController.animator.SetBool("RightBoomerang", false);
    }
    public bool IsDuelWielding()
    {
        if (m_leftWeaponData != null && m_rightWeaponData != null)
            return m_leftWeaponData.weaponType == m_rightWeaponData.weaponType;
        else
            return false;
    }
    public Hand GetCurrentUsedHand()
    {
        Hand castState = GetCurrentCastingHand();
        Hand attackState = GetCurrentAttackingHand();

        if (castState != Hand.NONE && attackState != Hand.NONE)
        {
            if (castState != attackState)
                return Hand.NONE;
            else
                return castState;
        }
        else
        {
            if (castState == Hand.NONE)
                return attackState;
            else
                return castState;
        }
    }
    public Hand GetCurrentCastingHand()
    {
        bool usingRight = false;
        bool usingLeft = false;

        if (playerController.animator.GetBool("RightCast"))
            usingRight = true;
        if (playerController.animator.GetBool("LeftCast"))
            usingLeft = true;

        if (usingRight == usingLeft)
            return Hand.NONE;
        else if (usingRight)
            return Hand.RIGHT;
        else // Using Left
            return Hand.LEFT;
    }
    public Hand GetCurrentAttackingHand()
    {
        bool usingRight = false;
        bool usingLeft = false;
        // thisData.weaponType.ToString()[0] + thisData.weaponType.ToString().Substring(1)
        
        if (playerController.animator.GetBool("UsingRight"))
        {
            usingRight = true;
        }
        if (playerController.animator.GetBool("UsingLeft"))
        {
            usingLeft = true;
        }

        //foreach (string name in System.Enum.GetNames(typeof(Weapon)))
        //{
        //    if (name == "BRICK")
        //        continue;

        //    if (playerController.animator.GetBool("Right" + name[0] + name.Substring(1).ToLower()))
        //    {
        //        usingRight = true;
        //    }
        //    if (playerController.animator.GetBool("Left" + name[0] + name.Substring(1).ToLower()))
        //    {
        //        usingLeft = true;
        //    }
        //}

        //if (playerController.animator.GetBool("RightShield")
        //    || playerController.animator.GetBool("RightSword")
        //    || playerController.animator.GetBool("RightBoomerang")
        //    || playerController.animator.GetBool("RightCrossbow")
        //    || playerController.animator.GetBool("RightSpear"))
        //    usingRight = true;
        //if (playerController.animator.GetBool("LeftShield")
        //    || playerController.animator.GetBool("LeftSword") 
        //    || playerController.animator.GetBool("LeftBoomerang")
        //    || playerController.animator.GetBool("LeftCrossbow")
        //    || playerController.animator.GetBool("LeftSpear"))
        //    usingLeft = true;

        if (usingRight == usingLeft)
            return Hand.NONE;
        else if (usingRight)
            return Hand.RIGHT;
        else // Using Left
            return Hand.LEFT;
    }

    /*******************
     * PickUpWeapon : Pick up a weapon and add it to hand.
     * @author : William de Beer
     * @param : (DroppedWeapon) The weapon to be picked up, (Hand) Hand to be given to.
     */
    public void PickUpWeapon(DroppedWeapon _weapon, Hand _hand)
    {
        if ((m_leftWeaponData && m_leftWeaponData.isTwoHanded) || (m_rightWeaponData && m_rightWeaponData.isTwoHanded))
        {
            DropWeapon(Hand.LEFT, _weapon.transform.position);
            ApplyWeaponData(Hand.LEFT);

            DropWeapon(Hand.RIGHT, _weapon.transform.position);
            ApplyWeaponData(Hand.RIGHT);
        }
        else
        {
            // Drop old weapon
            DropWeapon(_hand, _weapon.transform.position);
            ApplyWeaponData(_hand);
        }

        switch (_hand)
        {
            case Hand.LEFT:
                if (_weapon.m_weaponData.isTwoHanded)
                {
                    DropWeapon(Hand.RIGHT, transform.position);
                }

                // Set new weapon
                m_leftWeaponData = _weapon.m_weaponData;

                break;
            case Hand.RIGHT:
                if (_weapon.m_weaponData.isTwoHanded)
                {
                    DropWeapon(Hand.LEFT, transform.position);
                }

                // Set new weapon
                m_rightWeaponData = _weapon.m_weaponData;

                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                return;
        }
        ApplyWeaponData(Hand.LEFT);
        ApplyWeaponData(Hand.RIGHT);

        Destroy(_weapon.gameObject);
    }

    public void DropWeapon(Hand _hand, Vector3 _pos)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                if (m_leftWeaponData != null)
                {
                    if (m_leftWeaponData.abilityData != null && playerController.playerAbilities.m_leftAbility != null)
                        m_leftWeaponData.abilityData.lastCooldown = playerController.playerAbilities.m_leftAbility.m_cooldownTimer;
                    DroppedWeapon.CreateDroppedWeapon(_pos, m_leftWeaponData);
                    playerController.playerStats.RemoveEffect(m_leftWeaponData.itemEffect); // Remove any passive effect the weapon had

                    m_leftWeaponData = null;
                }
                break;
            case Hand.RIGHT:
                if (m_rightWeaponData != null)
                {
                    if (m_rightWeaponData.abilityData != null && playerController.playerAbilities.m_rightAbility != null)
                        m_rightWeaponData.abilityData.lastCooldown = playerController.playerAbilities.m_rightAbility.m_cooldownTimer;
                    DroppedWeapon.CreateDroppedWeapon(_pos, m_rightWeaponData);
                    playerController.playerStats.RemoveEffect(m_rightWeaponData.itemEffect); // Remove any passive effect the weapon had

                    m_rightWeaponData = null;
                }
                break;
            default:
                break;
        }
    }

    public void ApplyWeaponData(Hand _hand)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                // Delete old weapon from player
                Destroy(m_leftWeapon);

                if (m_leftWeaponData != null)
                {
                    m_leftWeapon = MakeNewWeaponComponent(m_leftWeaponData.weaponType);
                    m_leftWeapon.m_weaponObject = Instantiate(m_leftWeaponData.weaponModelPrefab, m_leftHandTransform);
                    m_leftWeapon.m_weaponData = m_leftWeaponData;
                    m_leftWeapon.SetHand(Hand.LEFT);

                    if (m_leftWeaponIcon != null)
                        m_leftWeaponIcon.SetIconSprite(m_leftWeaponData.weaponIcon);
                    else
                        Debug.LogWarning("Weapon icon not set");

                    playerController.playerAbilities.SetAbility(m_leftWeaponData.abilityData, Hand.LEFT);
                }
                else
                {
                    if (m_leftWeaponIcon != null)
                        m_leftWeaponIcon.SetIconSprite(null);
                    playerController.playerAbilities.SetAbility(null, Hand.LEFT);
                }

                playerController.m_statsMenu.UpdateWeaponInfo(Hand.LEFT, m_leftWeaponData);
                break;
            case Hand.RIGHT:
                // Delete old weapon from player
                Destroy(m_rightWeapon);

                if (m_rightWeaponData != null)
                {
                    m_rightWeapon = MakeNewWeaponComponent(m_rightWeaponData.weaponType);
                    m_rightWeapon.m_weaponObject = Instantiate(m_rightWeaponData.weaponModelPrefab, m_rightHandTransform);
                    m_rightWeapon.m_weaponData = m_rightWeaponData;
                    m_rightWeapon.SetHand(Hand.RIGHT);

                    if (m_rightWeaponIcon != null)
                        m_rightWeaponIcon.SetIconSprite(m_rightWeaponData.weaponIcon);
                    else
                        Debug.LogWarning("Weapon icon not set");

                    playerController.playerAbilities.SetAbility(m_rightWeaponData.abilityData, Hand.RIGHT);
                }
                else
                {
                    if (m_rightWeaponIcon != null)
                        m_rightWeaponIcon.SetIconSprite(null);
                    playerController.playerAbilities.SetAbility(null, Hand.RIGHT);
                }

                playerController.m_statsMenu.UpdateWeaponInfo(Hand.RIGHT, m_rightWeaponData);
                break;
        }

        m_leftWeaponIcon.SetDisabledState(m_rightWeaponData && m_rightWeaponData.isTwoHanded);
        m_rightWeaponIcon.SetDisabledState(m_leftWeaponData && m_leftWeaponData.isTwoHanded);

        playerController.playerAudioAgent.EquipWeapon();
    }

    private WeaponBase MakeNewWeaponComponent(Weapon _weapon)
    {
        switch (_weapon)
        {
            case Weapon.SWORD:
                return gameObject.AddComponent<Weapon_Sword>();
            case Weapon.SHIELD:
                return gameObject.AddComponent<Weapon_Shield>();
            case Weapon.BOOMERANG:
                return gameObject.AddComponent<Weapon_Boomerang>();
            case Weapon.CROSSBOW:
                return gameObject.AddComponent<Weapon_Crossbow>();
            case Weapon.SPEAR:
                return gameObject.AddComponent<Weapon_Spear>();
            case Weapon.BRICK:
                return gameObject.AddComponent<Weapon_Brick>();
            case Weapon.AXE:
                return gameObject.AddComponent<Weapon_Axe>();
            case Weapon.STAFF:
                return gameObject.AddComponent<Weapon_Staff>();
            case Weapon.BOW:
                return gameObject.AddComponent<Weapon_Bow>();
            case Weapon.GREATSWORD:
                return gameObject.AddComponent<Weapon_Sword>();
            default:
                return null;
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

        if (m_leftWeaponData != null && m_leftWeaponData.abilityData != null && playerController.playerAbilities.m_leftAbility != null)
            m_leftWeaponData.abilityData.lastCooldown = playerController.playerAbilities.m_leftAbility.m_cooldownTimer;

        if (m_rightWeaponData != null && m_rightWeaponData.abilityData != null && playerController.playerAbilities.m_rightAbility != null)
            m_rightWeaponData.abilityData.lastCooldown = playerController.playerAbilities.m_rightAbility.m_cooldownTimer;

        // Store old left hand weapon for future use
        WeaponData _leftHandStore = m_leftWeaponData;

        // Set new weapons
        m_leftWeaponData = m_rightWeaponData;
        m_rightWeaponData = _leftHandStore;

        ApplyWeaponData(Hand.LEFT);
        ApplyWeaponData(Hand.RIGHT);
    }

    /*******************
     * DamageTarget : Apply damage effects to Actor.
     * @author : William de Beer
     * @param : (GameObject) Target of attack, (float) Damage to deal
     */
    public void DamageTarget(GameObject _target, float _damage, float _knockbackForce = 5.0f, Vector3 _damageSource = default(Vector3))
    {
        playerController.playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.HIT_DEALT, _target.gameObject, _damage);
        playerController.playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.HIT_DEALT, _target.gameObject, _damage);

        Actor actor = _target.GetComponentInParent<Actor>();
        if (actor != null)
        {
            actor.DealDamage(_damage, CombatSystem.DamageType.Physical, CombatSystem.Faction.Player, transform.position);
        }

        Vector3 damageSource = (_damageSource == null ? transform.position : _damageSource);

        Destructible destructible = _target.GetComponentInParent<Destructible>();
        if (destructible != null)
        {
            destructible.ExplodeObject(transform.position, _knockbackForce * 8.0f, 20.0f);
        }

        if (LayerMask.LayerToName(_target.layer) == "Rubble")
        {
            Rigidbody rigidbody = _target.GetComponent<Rigidbody>();
            if (rigidbody)
                rigidbody.AddExplosionForce(_knockbackForce * 8.0f, transform.position, 20.0f);
        }
    }

    public void ShowWeapons(bool _show)
    {
        if (m_leftWeapon)
            m_leftWeapon.m_weaponObject.SetActive(_show);

        if (m_rightWeapon)
            m_rightWeapon.m_weaponObject.SetActive(_show);

        //if (m_leftWeaponObject != null)
        //    m_leftWeaponObject.SetActive(_show);

        //if (m_rightWeaponObject != null)
        //    m_rightWeaponObject.SetActive(_show);
    }

    private void OnDrawGizmosSelected()
    {
        if (playerController != null)
        {
            if (m_rightWeaponData != null)
            {
                Gizmos.color = Color.magenta;
                Gizmos.DrawWireSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_rightWeaponData.hitCenterOffset, m_rightWeaponData.hitSize);
            }
            if (m_leftWeaponData != null)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawWireSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_leftWeaponData.hitCenterOffset, m_leftWeaponData.hitSize);
            }
        }

        Gizmos.color = Color.yellow;
        Gizmos.DrawLine(m_leftHandTransform.position - m_leftHandTransform.forward * 0.3f, m_leftHandTransform.position + m_leftHandTransform.forward);
        Gizmos.DrawLine(m_rightHandTransform.position - m_rightHandTransform.forward * 0.3f, m_rightHandTransform.position + m_rightHandTransform.forward);
    }
    public void ToggleBlock(bool _active)
    {
        m_isBlocking = _active;
    }

    /*******************
     * CatchProjectile : Returns projectile to hand
     * @author : William de Beer
     * @param : (Hand) 
     */
    public void CatchProjectile(Hand _hand)
    {
        // Set activation booleans
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftWeapon.SetInUse(false);
                m_leftWeapon.m_weaponObject.SetActive(true);
                break;
            case Hand.RIGHT:
                m_rightWeapon.SetInUse(false);
                m_rightWeapon.m_weaponObject.SetActive(true);
                break;
            default:
                Debug.Log("If you got here, I don't know what to tell you. You must have a third hand or something");
                break;
        }
    }

    public void SetWeaponData(Hand _hand, WeaponData _data)
    {
        switch (_hand)
        {
            case Hand.LEFT:
                m_leftWeaponData = _data;
                ApplyWeaponData(Hand.LEFT);
                break;
            case Hand.RIGHT:
                m_rightWeaponData = _data;
                ApplyWeaponData(Hand.RIGHT);
                break;
            default:
                return;
        }
    }
}

///*******************
// * WeaponAttack : Create sphere attack detection and damages enemies in it.
// * @author : William de Beer
// * @param : (WeaponData) 
// */
//private void WeaponAttack(WeaponData _data, Vector3 _source)
//{
//    List<GameObject> hitList = new List<GameObject>();
//    Collider[] colliders = Physics.OverlapSphere(Vector3.up * m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * _data.hitCenterOffset, _data.hitSize, m_attackTargets);
//    foreach (var collider in colliders)
//    {
//        if (hitList.Contains(collider.gameObject))
//            continue;

//        DamageTarget(collider.gameObject, _data.m_damage);
//        Actor actor = collider.GetComponentInParent<Actor>();
//        if (actor != null && !hitList.Contains(collider.gameObject))
//        {
//            Debug.Log("Hit " + collider.name + " with " + _data.weaponType + " for " + _data.m_damage);
//            actor.KnockbackActor((actor.transform.position - _source).normalized * _data.m_knockback);
//        }
//        hitList.Add(collider.gameObject);
//    }
//    if (hitList.Count != 0)
//        playerController.playerAudioAgent.PlayWeaponHit(_data.weaponType); // Audio
//}


//#region ShieldBlock
//public void BeginBlock(Hand _hand)
//{
//    if (_hand == Hand.LEFT)
//    {
//        if (m_leftHeldObjectInstance != null)
//        {
//            Destroy(m_leftHeldObjectInstance);
//        }

//        m_leftHeldObjectInstance = Instantiate(m_shieldBlockPrefab, playerController.playerMovement.playerModel.transform);
//    }
//    else
//    {
//        if (m_rightHeldObjectInstance != null)
//        {
//            Destroy(m_rightHeldObjectInstance);
//        }

//        m_rightHeldObjectInstance = Instantiate(m_shieldBlockPrefab, playerController.playerMovement.playerModel.transform);
//    }
//    m_isBlocking = true;
//}
//public void StopBlock(Hand _hand)
//{
//    if (_hand == Hand.LEFT)
//    {
//        if (m_leftHeldObjectInstance != null)
//        {
//            Destroy(m_leftHeldObjectInstance);
//            m_leftHeldObjectInstance = null;
//            m_isBlocking = false;
//        }
//    }
//    else
//    {
//        if (m_rightHeldObjectInstance != null)
//        {
//            Destroy(m_rightHeldObjectInstance);
//            m_rightHeldObjectInstance = null;
//            m_isBlocking = false;
//        }
//    }
//}
//#endregion
///*******************
// * ThrowBoomerang : Launches projectile from specified hand.
// * @author : William de Beer
// * @param : (Vector3) Point which projectile spawns, (WeaponData), (Hand),
// */
//private void ThrowBoomerang(Vector3 _pos, WeaponData _data, Hand _hand)
//{
//    // Create projectile
//    GameObject projectile = Instantiate(m_boomerangeProjectilePrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
//    projectile.GetComponent<BoomerangProjectile>().SetReturnInfo(this, _data, _hand); // Set the information of the user to return to

//    // Set activation booleans
//    switch (_hand)
//    {
//        case Hand.LEFT:
//            m_leftWeaponObject.SetActive(false);
//            m_leftWeaponInUse = true;
//            break;
//        case Hand.RIGHT:
//            m_rightWeaponObject.SetActive(false);
//            m_rightWeaponInUse = true;
//            break;
//        default:
//            break;
//    }
//}

//public void ApplyWeaponData(Hand _hand)
//{
//    switch (_hand)
//    {
//        case Hand.LEFT:
//            // Delete old weapon from player
//            Destroy(m_leftWeaponObject);
//            if (m_leftWeaponData != null)
//            {
//                m_leftWeapon = MakeNewWeaponComponent(m_leftWeaponData.weaponType);
//                m_leftWeapon.m_weaponObject =  = Instantiate(m_leftWeaponData.weaponModelPrefab, m_leftHandTransform);

//                m_leftWeaponObject

//                    if (m_leftWeaponIcon != null)
//                    m_leftWeaponIcon.SetIconSprite(m_leftWeaponData.weaponIcon);
//                else
//                    Debug.LogWarning("Weapon icon not set");

//                playerController.playerAbilities.SetAbility(m_leftWeaponData.abilityData, Hand.LEFT);
//            }
//            else
//            {
//                if (m_leftWeaponIcon != null)
//                    m_leftWeaponIcon.SetIconSprite(null);
//                playerController.playerAbilities.SetAbility(null, Hand.LEFT);
//            }
//            break;
//        case Hand.RIGHT:
//            // Delete old weapon from player
//            Destroy(m_rightWeaponObject);
//            if (m_rightWeaponData != null)
//            {
//                m_rightWeaponObject = Instantiate(m_rightWeaponData.weaponModelPrefab, m_rightHandTransform);

//                if (m_rightWeaponIcon != null)
//                    m_rightWeaponIcon.SetIconSprite(m_rightWeaponData.weaponIcon);
//                else
//                    Debug.LogWarning("Weapon icon not set");

//                playerController.playerAbilities.SetAbility(m_rightWeaponData.abilityData, Hand.RIGHT);
//            }
//            else
//            {
//                if (m_rightWeaponIcon != null)
//                    m_rightWeaponIcon.SetIconSprite(null);
//                playerController.playerAbilities.SetAbility(null, Hand.RIGHT);
//            }
//            break;
//    }
//    playerController.playerAudioAgent.EquipWeapon();
//}
