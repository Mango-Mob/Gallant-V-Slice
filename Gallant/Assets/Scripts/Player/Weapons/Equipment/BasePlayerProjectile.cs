using ActorSystem.AI;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BasePlayerProjectile : MonoBehaviour
{
    public GameObject[] m_effects;

    // The model transform of the projectile to animate it
    public Transform m_modelTransform;
    public GameObject m_weaponModel;

    protected Player_Attack m_projectileUser; // The user of the projectile
    public float m_projectileSpeed = 10.0f;
    public WeaponData m_weaponData;


    protected Hand m_hand; // Hand to return to
    protected Transform m_handTransform;

    protected float m_lifeTimer = 0.0f;
    protected float m_throwDuration = 1.0f;
    protected float m_charge = 1.0f;

    protected bool m_returning = false;

    protected List<GameObject> hitList = new List<GameObject>();

    private bool m_spawning = true;
    private float m_scaleLerp = 0.0f;
    public float m_thrownScale = 1.0f;
    private Vector3 m_startScale;

    // Start is called before the first frame update
    protected void Start()
    {
        // Set hand transform to be returned to
        m_handTransform = (m_hand == Hand.LEFT ? m_projectileUser.m_leftHandTransform : m_projectileUser.m_rightHandTransform);
        GetComponentInChildren<SphereCollider>().radius = m_hand == Hand.LEFT ? m_weaponData.altHitSize : m_weaponData.hitSize;
        m_startScale = transform.localScale;

        StartCoroutine(Spawning());
    }
    private void Update()
    {
        if (m_spawning)
        {
            m_scaleLerp += Time.deltaTime * 4.0f;
            transform.localScale = Vector3.Lerp(m_startScale, m_startScale * m_thrownScale, m_scaleLerp);
        }
    }
    protected void ApplyWeaponModel()
    {
        m_weaponModel = Instantiate(m_weaponData.weaponModelPrefab, m_modelTransform);

        GameObject model = m_weaponModel.transform.GetChild(0).gameObject;
        if (model != null)
        {
            model.transform.parent = m_modelTransform;
            Destroy(m_weaponModel);
            m_weaponModel = model;

            m_weaponModel.transform.localPosition = Vector3.zero;
            m_weaponModel.transform.localRotation = Quaternion.Euler(0, 0, 0);
        }

    }
    protected void ProjectileReturnUpdate()
    {
        if (m_handTransform == null)
        {
            Debug.LogWarning("Trying to call return function without hand to return to set.");
        }

        // Lifetime timer for return
        m_lifeTimer += Time.fixedDeltaTime;
        if (m_throwDuration > m_lifeTimer && !m_returning) // If projectile is moving away from player
        {
            transform.position += m_projectileSpeed * transform.forward * Time.fixedDeltaTime; // Move projectile
        }
        else // If projectile is moving back towards the player
        {
            if (!m_returning)
            {
                m_returning = true;
                hitList.Clear();
            }

            // Get direction towards player
            Vector3 direction = ((m_handTransform.position) - transform.position);
            direction.y = 0;
            direction.Normalize();

            transform.position += m_projectileSpeed * direction * Time.fixedDeltaTime; // Move projectile
                                                                                       //transform.rotation = Quaternion.LookRotation(direction, transform.up); // Face projectile towards player
            if (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), // Check distance between player and projectile
                new Vector3(m_handTransform.position.x, 0, m_handTransform.position.z)) < 0.2f * m_projectileSpeed / 20.0f)
            {
                // "Catch" the projectile when close enough to player.
                m_projectileUser.CatchProjectile(m_hand);

                foreach (var effect in m_effects)
                {
                    effect.transform.SetParent(null);
                    if (effect.GetComponent<VFXTimerScript>() != null)
                        effect.GetComponent<VFXTimerScript>().m_startedTimer = true;
                }

                Destroy(gameObject);
            }
        }
    }
    protected bool ProjectileCollide(Collider other)
    {
        Debug.Log("Hit " + other.name + " with " + m_weaponData.weaponType + " for " + m_weaponData.m_damage * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f));

        m_projectileUser.DamageTarget(other.gameObject, m_weaponData.m_damage * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altDamageMult : 1.0f), m_weaponData.m_knockback * m_charge * (m_hand == Hand.LEFT ? m_weaponData.m_altKnockbackMult : 1.0f), 0, CombatSystem.DamageType.Physical);

        m_projectileUser.playerController.playerAudioAgent.PlayWeaponHit(m_weaponData.weaponType, 2); // Audio

        m_projectileUser.CreateVFX(other, transform.position);

        Actor actor = other.GetComponentInParent<Actor>();
        if (actor != null)
        {
            if (actor.m_myBrain.IsDead)
                return false;

            hitList.Add(other.gameObject);
            actor.KnockbackActor((actor.transform.position - transform.position).normalized * m_weaponData.m_knockback * m_charge);
            return true;
        }
        return false;
    }
    public void Destruct()
    {
        m_projectileUser.CatchProjectile(m_hand);

        foreach (var effect in m_effects)
        {
            effect.transform.SetParent(null);
            if (effect.GetComponentInChildren<VFXTimerScript>() != null)
                effect.GetComponentInChildren<VFXTimerScript>().m_startedTimer = true;

            if (effect.GetComponentInChildren<ParticleSystem>() != null)
                effect.GetComponentInChildren<ParticleSystem>().Stop();
        }

        Destroy(gameObject);
    }

    /*******************
     * SetReturnInfo : Sets the information of the user who threw the boomerang and who it should be returned to.
     * @author : William de Beer
     * @param : (Player_Attack) The Player_Attack component of player, (WeaponData) The data of weapon, (Hand) The hand it originated from.
     * @return : (type) 
     */
    public void SetReturnInfo(Player_Attack _user, WeaponData _data, Hand _hand, float _charge = 1.0f, bool _canCharge = false)
    {
        m_projectileUser = _user;
        m_weaponData = _data;
        m_hand = _hand;

        m_charge = _charge;

        if (_canCharge)
            m_effects[0].SetActive(_canCharge && _charge >= 1.0f);
    }
    IEnumerator Spawning()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        m_spawning = false;
    }
}
