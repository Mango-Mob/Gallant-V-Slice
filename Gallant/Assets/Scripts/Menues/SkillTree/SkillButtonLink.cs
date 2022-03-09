using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillButtonLink : MonoBehaviour
{
    private Image m_lineImage;
    private LineRenderer m_lineRenderer;

    [SerializeField] private Color m_activeColor = new Color(1, 1, 1, 1);
    [SerializeField] private Color m_deactiveColor = new Color(0.5f, 0.5f, 0.5f, 1);

    // Start is called before the first frame update
    void Start()
    {
        m_lineImage = GetComponent<Image>();
        m_lineRenderer = GetComponent<LineRenderer>();
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
