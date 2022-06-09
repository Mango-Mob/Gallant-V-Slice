using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon_Spear : WeaponBase
{
    GameObject m_objectPrefab2;
    new private void Awake()
    {
        m_objectPrefab = Resources.Load<GameObject>("VFX/WeaponSwings/Spear Thrust 1");
        m_objectPrefab2 = Resources.Load<GameObject>("VFX/WeaponSwings/Spear Thrust 2");

        m_objectAltPrefab = Resources.Load<GameObject>("VFX/WeaponSwings/Spear Spin");
        base.Awake();
    }

    // Start is called before the first frame update
    new private void Start()
    {
        base.Start();
    }

    // Update is called once per frame
    new private void Update()
    {
        base.Update();
    }
    public override void WeaponFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        LongMeleeAttack(m_weaponData, transform.position + Vector3.up * playerController.playerAttack.m_swingHeight);

        Transform weaponTransform = m_weaponObject.transform.GetChild(0).transform;
        Debug.Log(weaponTransform.gameObject.name);
        switch (playerController.animator.GetInteger("ComboCount"))
        {
            case 1:
                GameObject VFX = SpawnVFX(m_objectPrefab, weaponTransform.position, weaponTransform.rotation);
                //VFX.transform.localScale *= (m_weaponData.hitCenterOffset + m_weaponData.hitSize) * 1.5f;
                VFX.transform.SetParent(m_weaponObject.transform);

                //VFX.transform.localScale = new Vector3(VFX.transform.localScale.x * -1.0f,
                //    VFX.transform.localScale.y,
                //    VFX.transform.localScale.z);
                //VFX.transform.Rotate(new Vector3(0.0f, 30.0f, 0.0f));
                VFX.transform.position += VFX.transform.up * 0.5f;
                //VFX.transform.position += playerController.playerMovement.playerModel.transform.right * 0.15f;

                break;
            case 2:
                GameObject VFX2 = SpawnVFX(m_objectPrefab2, weaponTransform.position, weaponTransform.rotation);
                //VFX2.transform.localScale *= (m_weaponData.hitCenterOffset + m_weaponData.hitSize) * 1.5f;
                VFX2.transform.SetParent(m_weaponObject.transform);

                VFX2.transform.position += VFX2.transform.up;
                //VFX.transform.Rotate(new Vector3(0.0f, -30.0f, 0.0f));
                break;
        }


    }
    public override void WeaponRelease() { }
    public override void WeaponAltFunctionality()
    {
        playerController.playerAudioAgent.PlayWeaponSwing(m_weaponData.weaponType);
        MeleeAttack(m_weaponData, transform.position + Vector3.up * playerController.playerAttack.m_swingHeight, Hand.LEFT);
        GameObject VFX = SpawnVFX(m_objectAltPrefab, transform.position + transform.up, playerController.playerMovement.playerModel.transform.rotation);
        VFX.transform.localScale *= m_weaponData.altHitSize * 0.4f;
        VFX.transform.SetParent(transform);
    }
    public override void WeaponAltRelease() { }
    private void OnDrawGizmos()
    {
        Vector3 capsulePos = Vector3.up * playerController.playerAttack.m_swingHeight + transform.position + playerController.playerMovement.playerModel.transform.forward * m_weaponData.hitCenterOffset;
        Gizmos.DrawWireSphere(capsulePos, m_weaponData.hitSize);
        Gizmos.DrawWireSphere(capsulePos + playerController.playerMovement.playerModel.transform.forward * m_weaponData.hitSize, m_weaponData.hitSize);
    }
}
