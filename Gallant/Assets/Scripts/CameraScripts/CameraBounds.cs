using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraBounds : MonoBehaviour
{
    private Collider m_collider;

    // Start is called before the first frame update
    void Awake()
    {
        m_collider = GetComponent<Collider>();
    }

    public Vector3 RecalculateCameraLocation(Vector3 _pos)
    {
        return m_collider.ClosestPoint(_pos);

        bool containsCamera = false;

        Collider[] colliders = Physics.OverlapSphere(_pos, 1.0f);
        foreach (var collider in colliders)
        {
            if (collider == m_collider)
            {
                containsCamera = true;
                break;
            }
        }

        Debug.Log(containsCamera);

        if (!containsCamera)
        {
        }
        return _pos;
    }
}
