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
    public GameObject m_heldInstance;

    public Hand m_hand;

    // Start is called before the first frame update
    protected virtual void Awake()
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
    public void TriggerWeapon()
    {
        WeaponFunctionality();
    }

    public abstract void WeaponFunctionality();

    public void SetHand(Hand _hand) { m_hand = _hand; }
    public void SetInUse(bool _inUse) { m_isInUse = _inUse; }


    /*******************
     * MeleeAttack : Create sphere attack detection and damages enemies in it.
     * @author : William de Beer
     * @param : (WeaponData) 
     */
    protected void MeleeAttack(WeaponData _data, Vector3 _source)
    {
        List<GameObject> hitList = new List<GameObject>();
        Collider[] colliders = Physics.OverlapSphere(Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * _data.hitCenterOffset,
            _data.hitSize, playerController.playerAttack.m_attackTargets);
        foreach (var collider in colliders)
        {
            if (hitList.Contains(collider.gameObject))
                continue;

            playerController.playerAttack.DamageTarget(collider.gameObject, _data.m_damage);
            Actor actor = collider.GetComponentInParent<Actor>();
            if (actor != null && !hitList.Contains(collider.gameObject))
            {
                Debug.Log("Hit " + collider.name + " with " + _data.weaponType + " for " + _data.m_damage);
                actor.KnockbackActor((actor.transform.position - _source).normalized * _data.m_knockback);
            }
            hitList.Add(collider.gameObject);
        }
        if (hitList.Count != 0)
            playerController.playerAudioAgent.PlayWeaponHit(_data.weaponType); // Audio
    }
    protected void BeginBlock()
    {
        playerController.playerAttack.ToggleBlock(true);
    }

    /*******************
     * ThrowBoomerang : Launches projectile from specified hand.
     * @author : William de Beer
     * @param : (Vector3) Point which projectile spawns, (WeaponData), (Hand),
     */
    protected void ThrowBoomerang(Vector3 _pos, WeaponData _data, Hand _hand)
    {
        // Create projectile
        GameObject projectile = Instantiate(m_objectPrefab, _pos, Quaternion.LookRotation(playerController.playerMovement.playerModel.transform.forward, Vector3.up));
        projectile.GetComponent<BoomerangProjectile>().SetReturnInfo(playerController.playerAttack, _data, _hand); // Set the information of the user to return to

        m_weaponObject.SetActive(false);
        m_isInUse = true;
    }
}
