using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TagDetails : MonoBehaviour
{
    public int weight;

    public string m_tagTitle;
    public Color m_tagColor;
    public Color m_textColor;
    public uint m_fontSize = 20;

    public float m_width { get { return m_tagTitle.Length* m_fontSize; } }

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
            text.fontSize = (int) m_fontSize;

            if (text.gameObject != this.gameObject)
                text.color = m_textColor;
        }
    }

    public static int Compare(TagDetails a, TagDetails b)
    {
        if (a.weight == b.weight)
            return 0;

        return (b.weight - a.weight) / Mathf.Abs(b.weight - a.weight);
    }
}
