using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBase : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }
    private GameObject m_weaponObject;
    private GameObject m_weaponData;

    private bool m_isInUse = false;

    public GameObject m_objectPrefab;

    public Hand m_hand;

    // Start is called before the first frame update
    protected void Awake()
    {
        playerController = GetComponent<Player_Controller>();
    }

    // Update is called once per frame
    protected void Update()
    {
        
    }

    public void TriggerWeapon()
    {
        WeaponFunctionality();
    }

    public abstract void WeaponFunctionality();
}
