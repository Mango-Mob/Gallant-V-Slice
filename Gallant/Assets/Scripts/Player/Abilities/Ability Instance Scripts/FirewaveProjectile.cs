using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirewaveProjectile : MonoBehaviour
{
    public float m_speed = 10.0f;
    public float m_damage = 20.0f;
    public float m_lifeTime = 1.0f;
    private float m_lifeTimer = 0.0f;
    public AbilityData m_data;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position += transform.forward * m_speed * Time.fixedDeltaTime;
        m_lifeTimer += Time.fixedDeltaTime;
        if (m_lifeTimer > m_lifeTime)
            Destroy(gameObject);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Attackable"))
        {
            Debug.Log("Hit " + other.name + " with " + m_damage + " for " + m_damage);

            Actor actor = other.GetComponent<Actor>();
            if (actor != null)
            {
                actor.DealDamage(m_damage);
            }
        }
    }
}
