using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
/*
Radial Layout Group by Just a Pixel (Danny Goodayle) - http://www.justapixel.co.uk
Copyright (c) 2015

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in
all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
THE SOFTWARE.
*/
[ExecuteInEditMode]
public class RadialLayout : MonoBehaviour
{
    private int m_lastChildCount = 0;
    public float m_lerpSpeed = 10.0f;
    public float m_distance;
    [Range(0f, 360f)]
    public float m_minAngle, m_maxAngle, m_startAngle;
    private void Update()
    {
        CalculatePosition();
    }
    private void OnValidate()
    {
#if UNITY_EDITOR
        CalculatePosition();
#endif
    }

    void CalculatePosition()
    {
        if (transform.childCount == 0)
            return;

        m_lastChildCount = transform.childCount;

        float offsetAngle = ((m_maxAngle - m_minAngle)) / (transform.childCount - 1);
        float angle = m_startAngle;
        for (int i = 0; i < transform.childCount; i++)
        {
            RectTransform child = (RectTransform)transform.GetChild(i);
            if (child != null)
            {
                Vector3 vPos = Vector3.zero;
                if (transform.childCount == 1)
                {
                    float centeredAngle = m_startAngle + ((m_maxAngle - m_minAngle) * 0.5f);
                    vPos = new Vector3(Mathf.Cos(centeredAngle * Mathf.Deg2Rad), Mathf.Sin(centeredAngle * Mathf.Deg2Rad), 0);
                }
                else
                {
                    vPos = new Vector3(Mathf.Cos(angle * Mathf.Deg2Rad), Mathf.Sin(angle * Mathf.Deg2Rad), 0);
                }

                if (Application.isPlaying)
                {
                    child.localPosition = Vector3.Lerp(child.localPosition, vPos * m_distance, Time.deltaTime * m_lerpSpeed);
                }
                else
                {
                    child.localPosition = vPos * m_distance;
                }

                //Force objects to be center aligned, this can be changed however I'd suggest you keep all of the objects with the same anchor points.
                child.anchorMin = child.anchorMax = child.pivot = new Vector2(0.5f, 0.5f);
                angle += offsetAngle;
            }
        }
    }
}
