using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

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

    protected GameObject m_overrideHitVFXPrefab;
    protected GameObject m_overrideAltHitVFXPrefab;
    public bool m_attackReady = false;

    protected bool m_isVFXColored = false;
    protected bool m_isAltVFXColored = false;

    public ParticleSystem[] m_weaponTrailParticles;
    private bool m_trailActive = false;

    private bool m_attackedThisFrame = false;
    private bool m_releasedThisFrame = false;
    private bool m_altAttackedThisFrame = false;
    private bool m_altReleasedThisFrame = false;

    public bool isDashing = false;
    public List<Actor> m_dashHitList = new List<Actor>();

    // Start is called before the first frame update
    protected void Awake()
    {
        playerController = GetComponent<Player_Controller>();
    }

    protected void Start()
    {
        m_weaponTrailParticles = m_weaponObject.GetComponentsInChildren<ParticleSystem>();
        if (m_weaponData.abilityData != null)
        {
            foreach (var particleSystem in m_weaponTrailParticles)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                Color newColor = m_weaponData.abilityData.droppedEnergyColor;
                newColor.a = mainModule.startColor.color.a;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(newColor);
            }
        }
    }
    // Update is called once per frame
    protected void Update()
    {
        //if (!playerController.animator.GetBool((m_hand == Hand.LEFT) ? "UsingLeft" : "UsingRight"))
        if (!playerController.animator.GetBool("UsingLeft") && !playerController.animator.GetBool("UsingRight"))
        {
            SetTrailActive(false);
        }
        if (!playerController.animator.GetBool("UsingLeft") && isDashing) // Dash stop
        {
            isDashing = false;
            m_dashHitList.Clear();
        }

        m_attackedThisFrame = false;
        m_releasedThisFrame = false;
        m_altAttackedThisFrame = false;
        m_altReleasedThisFrame = false;
    }

    private void OnDestroy()
    {
        Destroy(m_weaponObject);
    }
    public void TriggerWeapon(bool _active)
    {
        if (_active)
        {
            if (m_attackedThisFrame)
                return;

            m_attackedThisFrame = true;
            WeaponFunctionality();
        }
        else
        {
            if (m_releasedThisFrame)
                return;

            m_releasedThisFrame = true;
            WeaponRelease();
        }
    }
    public void TriggerWeaponAlt(bool _active)
    {
        if (_active)
        {
            if (m_altAttackedThisFrame)
                return;

            m_altAttackedThisFrame = true;
            playerController.playerResources.ChangeStamina(-m_weaponData.m_altAttackStaminaCost);
            WeaponAltFunctionality();
        }
        else
        {
            if (m_altReleasedThisFrame)
                return;

            m_altReleasedThisFrame = true;
            WeaponAltRelease();
        }
    }

    public abstract void WeaponFunctionality();
    public abstract void WeaponRelease();
    public abstract void WeaponAltFunctionality();
    public abstract void WeaponAltRelease();

    public void SetHand(Hand _hand) { m_hand = _hand; }
    public void SetInUse(bool _inUse) { m_isInUse = _inUse; }
    public void SetTrailActive(bool _active)
    {
        if (m_trailActive == _active)
        {
            return;
        }

        m_trailActive = _active;
        foreach (var particleSystem in m_weaponTrailParticles)
        {
            if (_active)
            {
                particleSystem.Clear();
                particleSystem.Play();
            }
            else if (!_active)
            { 
                particleSystem.Stop();
            }
        }
    }
    protected GameObject SpawnVFX(GameObject _prefab, Vector3 _position, Quaternion _rotation)
    {
        GameObject VFX = Instantiate(_prefab, _position, _rotation);
        ParticleSystem[] particles = VFX.GetComponentsInChildren<ParticleSystem>();
        if (m_weaponData.abilityData != null)
        {
            Color newColor = m_weaponData.abilityData.droppedEnergyColor;
            foreach (var particleSystem in particles)
            {
                ParticleSystem.MainModule mainModule = particleSystem.main;
                newColor.a = mainModule.startColor.color.a;
                mainModule.startColor = new ParticleSystem.MinMaxGradient(newColor);
            }
        }
        return VFX;
    }
    public virtual string GetWeaponName()
    {
        if (m_weaponData.overrideAnimation =="" || m_hand == Hand.LEFT)
            return m_weaponData.weaponType.ToString()[0] + m_weaponData.weaponType.ToString().Substring(1).ToLower();
        else
        {
            return m_weaponData.overrideAnimation;
        }
    }

    private List<GameObject> DamageColliders(WeaponData _data, Vector3 _source, Collider[] colliders, Hand _usedHand = Hand.NONE, StatusEffect _status = null)
    {
        List<float> comboList = _data.m_comboDamageMult;
        float comboDamageMult = 1.0f;
        if (_usedHand == Hand.RIGHT && playerController.animator.GetInteger("ComboCount") < _data.m_comboDamageMult.Count)
            comboDamageMult = _data.m_comboDamageMult[playerController.animator.GetInteger("ComboCount")];

        playerController.animator.SetInteger("ComboCount", playerController.animator.GetInteger("ComboCount") + 1);
        List<GameObject> hitList = new List<GameObject>();
        foreach (var collider in colliders)
        {
            Actor actor = collider.GetComponentInParent<Actor>();
            if (hitList.Contains(collider.gameObject) || (actor != null && hitList.Contains(actor.gameObject)))
                continue;

            bool isRubble = collider.gameObject.layer == LayerMask.NameToLayer("Rubble");

            playerController.playerAttack.DamageTarget(collider.gameObject, comboDamageMult * _data.m_damage * (_usedHand == Hand.LEFT ? _data.m_altDamageMult : 1.0f), _data.m_impact * (_usedHand == Hand.LEFT ? _data.m_altImpactMult : 1.0f), _data.m_piercing, CombatSystem.DamageType.Physical, m_weaponData.abilityData != null ? m_weaponData.abilityData.m_tags : null);

            if (actor != null && !hitList.Contains(collider.gameObject) && collider.gameObject.layer != LayerMask.NameToLayer("Rubble"))
            {
                //Debug.Log("Hit " + collider.name + " with " + _data.weaponType + " for " + _data.m_damage * (m_hand == Hand.LEFT ? _data.m_altDamageMult : 1.0f));
                //actor.KnockbackActor((actor.transform.position - _source).normalized * _data.m_impact * (m_hand == Hand.LEFT ? _data.m_altImpactMult : 1.0f));

                StatusEffectContainer statusContainer = collider.GetComponentInParent<StatusEffectContainer>();
                if (statusContainer != null && _status != null)
                {
                    statusContainer.AddStatusEffect(_status.Clone());
                }
            }
            if (actor != null)
                hitList.Add(actor.gameObject);
            hitList.Add(collider.gameObject);

            if (!isRubble)
            {
                if (m_weaponData.abilityData && (_usedHand == Hand.LEFT && m_isAltVFXColored || _usedHand == Hand.RIGHT && m_isVFXColored))
                {
                    playerController.playerAttack.CreateVFX(collider, _source, m_weaponData.abilityData.droppedEnergyColor, _usedHand == Hand.LEFT ? m_overrideAltHitVFXPrefab : m_overrideHitVFXPrefab);
                }
                else
                {
                    playerController.playerAttack.CreateVFX(collider, _source, _usedHand == Hand.LEFT ? m_overrideAltHitVFXPrefab : m_overrideHitVFXPrefab);
                }
            }
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
    protected void MeleeAttack(WeaponData _data, Vector3 _source, Hand _overrideHand = Hand.NONE, StatusEffect _status = null)
    {
        Hand currentHand = m_hand;
        if (_overrideHand != Hand.NONE)
            currentHand = _overrideHand;

        Collider[] colliders = Physics.OverlapSphere(Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * (currentHand == Hand.LEFT ? _data.altHitCenterOffset : _data.hitCenterOffset),
            currentHand == Hand.LEFT ? _data.altHitSize : _data.hitSize, playerController.playerAttack.m_attackTargets);

        DamageColliders(_data, _source, colliders, currentHand, _status);
    }

    protected void LongMeleeAttack(WeaponData _data, Vector3 _source, Hand _overrideHand = Hand.NONE, StatusEffect _status = null)
    {
        Hand currentHand = m_hand;
        if (_overrideHand != Hand.NONE)
            currentHand = _overrideHand;

        Vector3 capsulePos = Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * (currentHand == Hand.LEFT ? _data.altHitCenterOffset : _data.hitCenterOffset);
        Collider[] colliders = Physics.OverlapCapsule(capsulePos, capsulePos + playerController.playerMovement.playerModel.transform.forward * (currentHand == Hand.LEFT ? _data.altHitSize : _data.hitSize),
            1.0f, playerController.playerAttack.m_attackTargets);

        
        DamageColliders(_data, _source, colliders, currentHand, _status);
    }
    protected void GroundSlam(WeaponData _data, Vector3 _source, Hand _overrideHand = Hand.NONE, StatusEffect _status = null)
    {
        Hand currentHand = m_hand;
        if (_overrideHand != Hand.NONE)
            currentHand = _overrideHand;

        Collider[] colliders = Physics.OverlapSphere(Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * (currentHand == Hand.LEFT ? _data.altHitCenterOffset : _data.hitCenterOffset),
            currentHand == Hand.LEFT ? _data.altHitSize : _data.hitSize, playerController.playerAttack.m_attackTargets);

        DamageColliders(_data, _source, colliders, currentHand, _status);
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
        projectile.GetComponent<BasePlayerProjectile>().m_overrideHitVFXColor = _hand == Hand.LEFT ? m_isAltVFXColored : m_isVFXColored;

        m_weaponObject.SetActive(false);
        m_isInUse = true;

        return projectile;
    }
    protected GameObject ConeAttack(Vector3 _pos, WeaponData _data, Hand _hand, float _angle)
    {
        Collider[] colliders = playerController.GetCollidersInfrontOfPlayer(_angle, _data.altHitSize, true).ToArray();
        DamageColliders(_data, _pos, colliders, _hand);

        //GameObject vfx = Instantiate(_hand == Hand.LEFT ? m_objectAltPrefab : m_objectPrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        //ParticleSystem particleSystem = vfx.GetComponentInChildren<ParticleSystem>();
        //ParticleSystem.MainModule mainModule = particleSystem.main;
        //mainModule.startLifetime = ((_hand == Hand.LEFT ? _data.altHitSize : _data.hitSize)) / mainModule.startSpeed.constant;
        //particleSystem.Play();

        return null;
    }

    protected GameObject ShootProjectile(Vector3 _pos, WeaponData _data, Hand _hand, float _charge = 1.0f, bool _canCharge = false)
    {
        GameObject projectile = Instantiate(_hand == Hand.LEFT ? m_objectAltPrefab : m_objectPrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        projectile.GetComponent<BasePlayerProjectile>().SetReturnInfo(playerController.playerAttack, _data, _hand, _charge, _canCharge);
        projectile.GetComponent<BasePlayerProjectile>().m_overrideHitVFXColor = _hand == Hand.LEFT ? m_isAltVFXColored : m_isVFXColored;

        return projectile;
    }

    protected GameObject SpawnProjectileInTransform(Vector3 _pos, WeaponData _data, Hand _hand)
    {
        GameObject projectile = Instantiate(_hand == Hand.LEFT ? m_objectAltPrefab : m_objectPrefab, transform);
        projectile.transform.position += Vector3.up * playerController.playerAttack.m_swingHeight + _pos;
        projectile.transform.rotation = Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up);
        projectile.GetComponent<BasePlayerProjectile>().SetReturnInfo(playerController.playerAttack, _data, _hand);
        projectile.GetComponent<BasePlayerProjectile>().m_overrideHitVFXColor = _hand == Hand.LEFT ? m_isAltVFXColored : m_isVFXColored;

        m_weaponObject.SetActive(false);
        m_isInUse = true;

        return projectile;
    }
}
