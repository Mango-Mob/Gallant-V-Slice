using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor_Material : MonoBehaviour
{
    public Material m_hitMaterial;
    public bool m_isDisolving { get; private set; } = false;

    private Renderer m_myMesh;
    private Material m_myMaterial;

    private float m_timer = 0.0f;
    
    private void Awake()
    {
        m_myMesh = GetComponent<Renderer>();
        m_myMaterial = m_myMesh.material;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_isDisolving && m_myMesh != null)
        {
            m_timer += Time.deltaTime;
            float maxTime = 4.5f;
            float disolveVal = 1.0f - m_timer / maxTime;

            m_myMaterial.SetFloat("Fade", disolveVal);

            if (m_timer > maxTime)
            {
                m_myMaterial.SetFloat("Fade", 0.0f);
                m_isDisolving = false;
            }
        }
    }
    private void LateUpdate()
    {

    }

    public void StartDisolve()
    {
        if(m_myMesh.material.name.Contains("Disolve"))
        {
            m_isDisolving = true;
        }
        else
        {
            Debug.LogWarning("Disolve called when texture isn't a disolve shader.");
        }
    }

    public void ShowHit()
    {
        StartCoroutine(ShowHitRoutine(0.02f));
    }

    private IEnumerator ShowHitRoutine(float time)
    {
        m_myMesh.material = m_hitMaterial;

        yield return new WaitForSecondsRealtime(time);

        m_myMesh.material = m_myMaterial;

        yield return null;
    }
}
