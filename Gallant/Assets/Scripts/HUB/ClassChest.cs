using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClassChest : MonoBehaviour
{
    public ClassData m_classData;
    public SpriteRenderer m_spriteRenderer;

    // Start is called before the first frame update
    void Start()
    {
        m_spriteRenderer.sprite = m_classData.m_classIcon;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter(Collider other)
    {
        if (PlayerPrefs.GetInt("RunActive") != 1 && m_classData)
        {
            if (other.GetComponent<Player_Controller>())
            {
                other.GetComponent<Player_Controller>().SelectClass(m_classData);
            }
        }
    }
}
