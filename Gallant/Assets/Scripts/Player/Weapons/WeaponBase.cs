using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
    public GameObject m_weaponObject;
    public WeaponData m_weaponData;

    public bool m_isInUse { private set; get; } = false;

    public GameObject m_objectPrefab;
    public GameObject m_objectAltPrefab;
    public GameObject m_heldInstance;

    public Hand m_hand;

    private GameObject m_hitVFXPrefab;
    public bool m_attackReady = false;

    // Start is called before the first frame update
    protected void Awake()
    {
        playerController = GetComponent<Player_Controller>();
    }

    protected void Start()
    {
    }
    // Update is called once per frame
    protected void Update()
    {
        
    }
    private void OnDestroy()
    {
        Destroy(m_weaponObject);
    }
    public void TriggerWeapon(bool _active)
    {
        if (_active)
        {
            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
            WeaponFunctionality();
        }
        else
            WeaponRelease();
    }
    public void TriggerWeaponAlt(bool _active)
    {
        if (_active)
        {
            playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
            WeaponAltFunctionality();
        }
        else
            WeaponAltRelease();
    }

    public abstract void WeaponFunctionality();
    public abstract void WeaponRelease();
    public abstract void WeaponAltFunctionality();
    public abstract void WeaponAltRelease();

    public void SetHand(Hand _hand) { m_hand = _hand; }
    public void SetInUse(bool _inUse) { m_isInUse = _inUse; }

    public virtual string GetWeaponName()
    {
        if (m_weaponData.overrideAnimation =="")
            return m_weaponData.weaponType.ToString()[0] + m_weaponData.weaponType.ToString().Substring(1).ToLower();
        else
        {
            return m_weaponData.overrideAnimation;
        }
    }

    private List<GameObject> DamageColliders(WeaponData _data, Vector3 _source, Collider[] colliders, StatusEffect _status = null)
    {
        List<GameObject> hitList = new List<GameObject>();
        foreach (var collider in colliders)
        {
            if (hitList.Contains(collider.gameObject))
                continue;

            bool isRubble = collider.gameObject.layer == LayerMask.NameToLayer("Rubble");

            playerController.playerAttack.DamageTarget(collider.gameObject, _data.m_damage * (m_hand == Hand.LEFT ? _data.m_altDamageMult : 1.0f), _data.m_knockback * (m_hand == Hand.LEFT ? _data.m_altKnockbackMult : 1.0f), _data.m_piercing);
            Actor actor = collider.GetComponentInParent<Actor>();

            if (actor != null && !hitList.Contains(collider.gameObject) && collider.gameObject.layer != LayerMask.NameToLayer("Rubble"))
            {
                Debug.Log("Hit " + collider.name + " with " + _data.weaponType + " for " + _data.m_damage * (m_hand == Hand.LEFT ? _data.m_altDamageMult : 1.0f));
                actor.KnockbackActor((actor.transform.position - _source).normalized * _data.m_knockback * (m_hand == Hand.LEFT ? _data.m_altKnockbackMult : 1.0f));

                StatusEffectContainer statusContainer = collider.GetComponentInParent<StatusEffectContainer>();
                if (statusContainer != null && _status != null)
                {
                    statusContainer.AddStatusEffect(_status);
                }
            }
            hitList.Add(collider.gameObject);

            if (!isRubble)
                playerController.playerAttack.CreateVFX(collider, _source);
        }
        if (hitList.Count != 0)
            playerController.playerAudioAgent.PlayWeaponHit(_data.weaponType); // Audio

        return hitList;
    }

    /*******************
     * MeleeAttack : Create sphere attack detection and damages enemies in it.
     * @author : William de Beer
     * @param : (WeaponData) 
     */
    protected void MeleeAttack(WeaponData _data, Vector3 _source, StatusEffect _status = null)
    {
        Collider[] colliders = Physics.OverlapSphere(Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * (m_hand == Hand.LEFT ? _data.altHitCenterOffset : _data.hitCenterOffset),
            m_hand == Hand.LEFT ? _data.altHitSize : _data.hitSize, playerController.playerAttack.m_attackTargets);

        DamageColliders(_data, _source, colliders, _status);
    }

    protected void LongMeleeAttack(WeaponData _data, Vector3 _source, StatusEffect _status = null)
    {
        Vector3 capsulePos = Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * (m_hand == Hand.LEFT ? _data.altHitCenterOffset : _data.hitCenterOffset);
        Collider[] colliders = Physics.OverlapCapsule(capsulePos, capsulePos + playerController.playerMovement.playerModel.transform.forward * _data.hitSize,
            m_hand == Hand.LEFT ? _data.altHitSize : _data.hitSize, playerController.playerAttack.m_attackTargets);

        
        DamageColliders(_data, _source, colliders, _status);
    }
    protected void GroundSlam(WeaponData _data, Vector3 _source, StatusEffect _status = null)
    {
        Debug.Log(m_hand);

        Collider[] colliders = Physics.OverlapSphere(Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * (m_hand == Hand.LEFT ? _data.altHitCenterOffset : _data.hitCenterOffset),
            m_hand == Hand.LEFT ? _data.altHitSize : _data.hitSize, playerController.playerAttack.m_attackTargets);

        DamageColliders(_data, _source, colliders, _status);
    }

    protected void BeginBlock()
    {
        playerController.playerAttack.ToggleBlock(true);
    }

    /*******************
     * ThrowWeapon : Launches weapon from specified hand.
     * @author : William de Beer
     * @param : (Vector3) Point which projectile spawns, (WeaponData), (Hand),
     */
    protected GameObject ThrowWeapon(Vector3 _pos, WeaponData _data, Hand _hand)
    {
        // Create projectile
        GameObject projectile = Instantiate(_hand == Hand.LEFT ? m_objectAltPrefab : m_objectPrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        projectile.GetComponent<BasePlayerProjectile>().SetReturnInfo(playerController.playerAttack, _data, _hand); // Set the information of the user to return to

        m_weaponObject.SetActive(false);
        m_isInUse = true;

        return projectile;
    }
    protected GameObject ConeAttack(Vector3 _pos, WeaponData _data, Hand _hand, float _angle)
    {
        Collider[] colliders = playerController.GetCollidersInfrontOfPlayer(_angle, _data.altHitSize, true).ToArray();
        DamageColliders(_data, _pos, colliders);

        GameObject vfx = Instantiate(_hand == Hand.LEFT ? m_objectAltPrefab : m_objectPrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        ParticleSystem particleSystem = vfx.GetComponentInChildren<ParticleSystem>();
        ParticleSystem.MainModule mainModule = particleSystem.main;
        mainModule.startLifetime = ((_hand == Hand.LEFT ? _data.altHitSize : _data.hitSize)) / mainModule.startSpeed.constant;
        particleSystem.Play();

        return vfx;
    }

    protected GameObject ShootProjectile(Vector3 _pos, WeaponData _data, Hand _hand, float _charge = 1.0f, bool _canCharge = false)
    {
        GameObject projectile = Instantiate(_hand == Hand.LEFT ? m_objectAltPrefab : m_objectPrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        projectile.GetComponent<BasePlayerProjectile>().SetReturnInfo(playerController.playerAttack, _data, _hand, _charge, _canCharge);

        return projectile;
    }

    protected void SpawnProjectileInTransform(Vector3 _pos, WeaponData _data, Hand _hand)
    {
        GameObject projectile = Instantiate(_hand == Hand.LEFT ? m_objectAltPrefab : m_objectPrefab, transform);
        projectile.transform.position += Vector3.up * playerController.playerAttack.m_swingHeight + _pos;
        projectile.transform.rotation = Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up);
        projectile.GetComponent<BasePlayerProjectile>().SetReturnInfo(playerController.playerAttack, _data, _hand);

        m_weaponObject.SetActive(false);
        m_isInUse = true;
    }
}
