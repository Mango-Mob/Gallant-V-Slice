using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CurrencyDrop : MonoBehaviour
{
    [SerializeField] private int m_heldValue = 1;
    [SerializeField] private float m_pickupRange = 0.75f;
    [SerializeField] private float m_pullRange = 2.0f;
    [SerializeField] private float m_forceMultiplier = 2.0f;

    private Player_Controller m_targetPlayer;
    [SerializeField] private Animator m_animator;

    private float m_targetScale;
    private bool m_spawning = true;

    static bool m_isLayerCollisionConfigured = false;

    static public void CreateCurrencyDropGroup(uint _count, Vector3 _position, float _valuePerOrb = 0.3f)
    {
        GameObject orbPrefab = Resources.Load<GameObject>("CurrencyDrop");
        if (orbPrefab == null)
            Debug.LogError("Could not find currency orb prefab");

        for (int i = 0; i < _count; i++)
        {
            Vector3 offset;
            do
            {
                offset = new Vector3(Random.Range(-1.0f, 1.0f), 0, Random.Range(-1.0f, 1.0f));
            } while (offset.x == 0 && offset.z == 0);

            offset = offset.normalized * Random.Range(0.0f, 1.0f);

            Instantiate(orbPrefab, _position + offset, Quaternion.Euler(0, 0, 0));
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
        //Invoke("StartDeathAnimation", 10.0f);
        //DestroyObject();
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
                    //m_targetPlayer.playerResources.ChangeAdrenaline(1);
                    PlayerPrefs.SetInt("Player Balance", PlayerPrefs.GetInt("Player Balance") + m_heldValue);
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

    public void DestroyObject()
    {
        Destroy(gameObject);
    }

    IEnumerator Spawning()
    {
        yield return new WaitForSecondsRealtime(1.5f);
        m_spawning = false;
        GetComponent<Rigidbody>().isKinematic = false;
    }
}
