using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TagDetails : MonoBehaviour
{
    public string m_tagTitle;
    public Color m_tagColor;
    public Color m_textColor;

    // Update is called once per frame
    void Update()
    {
        foreach (var image in GetComponentsInChildren<Image>())
        {
            image.color = m_tagColor;
        }
        foreach (var text in GetComponentsInChildren<Text>())
        {
            text.text = m_tagTitle;
            text.color = m_textColor;
        }
    }
}
