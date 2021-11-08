﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AdrenalineDrop : MonoBehaviour
{
    [SerializeField] private float m_heldAdrenaline = 0.3f;
    [SerializeField] private float m_pickupRange = 0.75f;
    [SerializeField] private float m_pullRange = 2.0f;
    [SerializeField] private float m_forceMultiplier = 2.0f;

    private Player_Controller m_targetPlayer;

    private float m_targetScale;
    private bool m_spawning = true;

    static bool m_isLayerCollisionConfigured = false;

    static public void CreateAdrenalineDropGroup(uint _count, Vector3 _position, float _valuePerOrb = 0.3f)
    {
        GameObject orbPrefab = Resources.Load<GameObject>("AdrenalineDrop");
        if (orbPrefab == null)
            Debug.LogError("Could not find adrenaline orb prefab");

        for (int i = 0; i < _count; i++)
        {
            Instantiate(orbPrefab, _position, Quaternion.Euler(0, 0, 0));
        }
    }    

    // Start is called before the first frame update
    void Start()
    {
        if (!m_isLayerCollisionConfigured)
        {
            Physics.IgnoreLayerCollision(16, 8, true);
            Physics.IgnoreLayerCollision(16, 9, true);
            Physics.IgnoreLayerCollision(16, 10, true);
            Physics.IgnoreLayerCollision(16, 12, true);
            Physics.IgnoreLayerCollision(16, 13, true);
            Physics.IgnoreLayerCollision(16, 16, true);

            m_isLayerCollisionConfigured = true;
        }

        m_targetPlayer = FindObjectOfType<Player_Controller>();
        if (m_targetPlayer == null)
        {
            Debug.Log("Could not find player to move towards. Destroying Object.");
            Destroy(gameObject);
        }

        GetComponent<Rigidbody>().isKinematic = false;
        m_targetScale = transform.localScale.x;
        transform.localScale = Vector3.zero;

        StartCoroutine(Spawning());
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        if (!m_spawning)
        {
            float newScale;
            float distance = Vector3.Distance(m_targetPlayer.transform.position + transform.up, transform.position);
            if (distance < m_pullRange)
            {
                if (distance < m_pickupRange)
                {
                    m_targetPlayer.playerResources.ChangeAdrenaline(m_heldAdrenaline);
                    Destroy(gameObject);
                    return;
                }
                newScale = Mathf.Lerp(m_targetScale, 0.0f, 1.0f - (distance / m_pickupRange));
                GetComponent<Rigidbody>().AddForce((m_targetPlayer.transform.position - transform.position + transform.up).normalized * m_forceMultiplier * (1.0f - (distance / m_pullRange)));
                GetComponent<Rigidbody>().useGravity = false;
            }
            else
            {
                newScale = 1.0f;
                GetComponent<Rigidbody>().useGravity = true;
            }
            transform.localScale = new Vector3(1.0f, 1.0f, 1.0f) * newScale;
        }
        else
        {
            transform.localScale = Vector3.Lerp(transform.localScale, new Vector3(1.0f, 1.0f, 1.0f) * m_targetScale, 1 - Mathf.Pow(2.0f, -Time.fixedDeltaTime * 5.0f));
        }
    }

    IEnumerator Spawning()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        m_spawning = false;
        GetComponent<Rigidbody>().isKinematic = false;

    }
}