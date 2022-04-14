using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ColourShift : MonoBehaviour
{
    public GameObject m_Button;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (m_Button.activeInHierarchy)
        {
            var colors = GetComponent<Button>().colors;
            colors.normalColor = Color.white;
            GetComponent<Button>().colors = colors;
        }
    }
}
