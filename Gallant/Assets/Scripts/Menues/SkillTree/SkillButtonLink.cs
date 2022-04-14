using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonLink : MonoBehaviour
{
    private Transform m_point1;
    private Transform m_point2;

    private Image m_lineImage;
    private LineRenderer m_lineRenderer;

    [SerializeField] private Color m_activeColor = new Color(1, 1, 1, 1);
    [SerializeField] private Color m_deactiveColor = new Color(0.5f, 0.5f, 0.5f, 1);

    // Start is called before the first frame update
    void Awake()
    {
        m_lineImage = GetComponent<Image>();
        m_lineRenderer = GetComponent<LineRenderer>();
    }
    public void UpdatePositions()
    {
        if (m_point1 != null)
            m_lineRenderer.SetPosition(0, m_point1.position + Vector3.forward * 20.0f);
        if (m_point2 != null)
            m_lineRenderer.SetPosition(1, m_point2.position + Vector3.forward * 20.0f);
    }
    public void SetPoints(Transform _point1, Transform _point2)
    {
        m_point1 = _point1;
        m_point2 = _point2;

        m_lineRenderer.SetPosition(0, m_point1.position + Vector3.forward * 20.0f);
        m_lineRenderer.SetPosition(1, m_point2.position + Vector3.forward * 20.0f);
    }

    public void ToggleActive(bool _active)
    {
        if (_active)
        {
            m_lineImage.color = m_activeColor;
        }
        else
        {
            m_lineImage.color = m_deactiveColor;
        }
    }

    public void ToggleAvailability(bool _active)
    {
        m_lineRenderer.enabled = _active;
    }
}
