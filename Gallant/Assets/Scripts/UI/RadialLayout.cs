using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
[ExecuteInEditMode]
public class RadialLayout : MonoBehaviour
{
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
