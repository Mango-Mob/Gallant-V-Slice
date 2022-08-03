using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DaggerAttackVFX : BasePlayerProjectile
{
    private Animator animator;
    private Vector3 forward;

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

        forward = transform.forward;
    }

    private void Update()
    {
        transform.forward = forward;
    }
    protected override void EnvironmentCollision(Collider _other)
    {

    }

    public void SetAnimationSpeed(float _speed)
    {
        animator.speed = _speed;
    }
}
