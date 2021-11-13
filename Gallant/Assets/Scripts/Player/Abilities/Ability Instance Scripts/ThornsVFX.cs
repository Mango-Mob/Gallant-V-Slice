using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThornsVFX : MonoBehaviour
{
    public GameObject target;
    [SerializeField] private float m_fadeRate = 2.0f;
    [SerializeField] private GameObject m_barrier;

    private Material m_mat;
    private Vector4 m_matAlphaVector;

    // Start is called before the first frame update
    void Start()
    {
        m_mat = m_barrier.GetComponentInChildren<Renderer>().material;
        string[] props = m_mat.GetTexturePropertyNames();
        foreach (var p in props)
        {
            Debug.Log(p);
        }
        //m_matAlphaVector = m_mat.GetVector("_AlphaMult");
    }

    // Update is called once per frame
    void Update()
    {
        if (target != null)
            transform.forward = (target.transform.position + transform.up * 0.5f - transform.position).normalized;

        m_barrier.transform.localScale += new Vector3(1, 1, 1) * Time.deltaTime;

        //m_matAlphaVector.x = Mathf.Max(0.0f, m_matAlphaVector.x - Time.deltaTime * m_fadeRate);
        //if (m_mat != null)
        //    m_mat.SetVector("_AslphaMult", m_matAlphaVector);

        //m_mat.SetColor("_Emission", Color.red);
        //Color color = m_mat.GetColor("_Emission");

        m_mat.SetFloat("AlphaMult", Mathf.Max(0.0f, m_mat.GetFloat("AlphaMult") - Time.deltaTime * m_fadeRate));

    }
}
