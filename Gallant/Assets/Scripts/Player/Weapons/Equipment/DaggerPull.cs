using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerPull : BasePlayerProjectile
{
    private Collider m_hitTarget;
    private Vector3 m_relativeHitPosition;

    // Start is called before the first frame update
    void Awake()
    {
        animator = GetComponent<Animator>();
    }
    new private void Start()
    {
        base.Start();
        ApplyWeaponModel();

        m_weaponModel.transform.localPosition += Vector3.forward * 0.5f;
        m_weaponModel.transform.localRotation = Quaternion.Euler(-90.0f, 0.0f, 0.0f);
        m_projectileSpeed = 0;
    }

    private void Update()
    {

    }
    protected override void EnvironmentCollision(Collider _other)
    {

    }
    private void EnemyCollision(Collider _other)
    {

    }
    private void OnTriggerEnter(Collider other)
    {
        if (hitList.Contains(other.gameObject))
            return;

        LayerMask layerMask = m_projectileUser.playerController.playerAttack.m_attackTargets;
        if (layerMask == (layerMask | (1 << other.gameObject.layer)))
        {
            EnemyCollision(other);
        }
        if (m_canCollideWithEnvironment && other.gameObject.layer == LayerMask.NameToLayer("Environment"))
        {
            EnvironmentCollision(other);
        }
    }
}
