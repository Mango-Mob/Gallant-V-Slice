using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldToCanvas : MonoBehaviour
{
    public Transform m_anchorTransform;
    private Canvas m_canvas;
    // Start is called before the first frame update
    void Start()
    {
        m_canvas = GetComponentInParent<Canvas>();
    }

    // Update is called once per frame
    void Update()
    {
        if(m_canvas.enabled)
            ForceUpdate();
    }
    private void OnEnable()
    {
        ForceUpdate();
    }

    public void ForceUpdate()
    {
        if (m_anchorTransform != null)
        {
            Vector3 pos = GameManager.Instance.m_activeCamera.WorldToScreenPoint(m_anchorTransform.transform.position);
            (transform as RectTransform).position = pos;
        }
    }
}
