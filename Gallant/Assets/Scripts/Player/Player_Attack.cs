using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum Weapon
{
    SWORD,
    SHIELD,
    BOOMERANG,
    CROSSBOW,
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
        WeaponData thisData;
        string animatorTriggerName = "";

        switch (_hand)
        {
            case Hand.LEFT: // Left hand weapon
                if (m_leftWeaponInUse)
                    return;
                // Set weapon information
                thisData = m_leftWeaponData;
                animatorTriggerName += "Left";
                break;
            case Hand.RIGHT: // Right hand weapon
                if (m_rightWeaponInUse)
                    return;
                // Set weapon information
                thisData = m_rightWeaponData;
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
            case Weapon.CROSSBOW: // Use boomerang
                animatorTriggerName += "Crossbow";
                break;
            default:
                Debug.Log("Weapon not implemented:" + thisData.weaponType);
                break;
        }

        playerController.playerAudioAgent.PlayWeaponSwing();
        playerController.animator.SetBool(animatorTriggerName, true);
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

        if (playerController.animator.GetBool("RightShield")
            || playerController.animator.GetBool("RightSword")
            || playerController.animator.GetBool("RightBoomerang")
            || playerController.animator.GetBool("RightCrossbow"))
            usingRight = true;
        if (playerController.animator.GetBool("LeftShield")
            || playerController.animator.GetBool("LeftSword") 
            || playerController.animator.GetBool("LeftBoomerang")
            || playerController.animator.GetBool("LeftCrossbow"))
            usingLeft = true;

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
        switch (_hand)
        {
            case Hand.LEFT:
                // Drop old weapon
                if (m_leftWeaponData != null)
                {
                    if (m_leftWeaponData.abilityData != null && playerController.playerAbilities.m_leftAbility != null)
                        m_leftWeaponData.abilityData.lastCooldown = playerController.playerAbilities.m_leftAbility.m_cooldownTimer;
                    DroppedWeapon.CreateDroppedWeapon(_weapon.transform.position, m_leftWeaponData);
                    playerController.playerStats.RemoveEffect(m_leftWeaponData.itemEffect); // Remove any passive effect the weapon had
                }
                // Set new weapon
                m_leftWeaponData = _weapon.m_weaponData;
                ApplyWeaponData(Hand.LEFT);

                break;
            case Hand.RIGHT:
                // Drop old weapon
                if (m_rightWeaponData != null)
                {
                    if (m_rightWeaponData.abilityData != null && playerController.playerAbilities.m_rightAbility != null)
                        m_rightWeaponData.abilityData.lastCooldown = playerController.playerAbilities.m_rightAbility.m_cooldownTimer;
                    DroppedWeapon.CreateDroppedWeapon(_weapon.transform.position, m_rightWeaponData);
                    playerController.playerStats.RemoveEffect(m_rightWeaponData.itemEffect); // Remove any passive effect the weapon had
                }
                // Set new weapon
                m_rightWeaponData = _weapon.m_weaponData;
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
                break;
        }
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

        // Left hand weapon
        // Set new weapon
        m_leftWeaponData = m_rightWeaponData;
        ApplyWeaponData(Hand.LEFT);

        // Right hand weapon
        // Set new weapon
        m_rightWeaponData = _leftHandStore;
        ApplyWeaponData(Hand.RIGHT);
    }

    /*******************
     * DamageTarget : Apply damage effects to Actor.
     * @author : William de Beer
     * @param : (GameObject) Target of attack, (float) Damage to deal
     */
    public void DamageTarget(GameObject _target, float _damage)
    {
        playerController.playerAbilities.PassiveProcess(Hand.LEFT, PassiveType.HIT_DEALT, _target.gameObject, _damage);
        playerController.playerAbilities.PassiveProcess(Hand.RIGHT, PassiveType.HIT_DEALT, _target.gameObject, _damage);

        Actor actor = _target.GetComponentInParent<Actor>();
        if (actor != null)
        {
            actor.DealDamage(_damage, CombatSystem.DamageType.Physical, CombatSystem.Faction.Player, transform.position);
        }
    }

    public void ShowWeapons(bool _show)
    {
        if (m_leftWeapon)
            m_leftWeapon.m_weaponObject.SetActive(_show);

        if (m_leftWeapon)
            m_leftWeapon.m_weaponObject.SetActive(_show);

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
