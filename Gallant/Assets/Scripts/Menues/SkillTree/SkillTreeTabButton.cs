using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeTabButton : MonoBehaviour
{
    private Button m_button;
    [SerializeField] private Image m_deselectedEffect;
    [SerializeField] private InkmanClass m_class;

    // Start is called before the first frame update
    void Start()
    {
        m_button = GetComponent<Button>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void Activate()
    {
        SkillTreeDisplayControl._instance.SelectTab(m_class);
    }
    public void ToggleSelected(bool _active)
    {
        m_deselectedEffect.enabled = !_active;
    }
}
