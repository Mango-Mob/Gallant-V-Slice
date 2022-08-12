using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayerSystem;

public class Destruction : MonoBehaviour
{
    private Rigidbody[] m_myChildren;
    private Camera m_camera;
    private float m_cameraDestroyDist = 0.05f;

    // Start is called before the first frame update
    public void Awake()
    {
        m_myChildren = GetComponentsInChildren<Rigidbody>();
        m_camera = FindObjectOfType<Player_Controller>().playerCamera;
    }
    public void Update()
    {
        bool rubbleExists = false;
        foreach (var rubble in m_myChildren)
        {
            if (rubble == null)
                continue;

            rubbleExists = true;

            Vector3 screenPoint = m_camera.WorldToViewportPoint(rubble.transform.position);
            bool offScreen = screenPoint.x <= -m_cameraDestroyDist || screenPoint.x >= 1.0 + m_cameraDestroyDist 
                || screenPoint.y <= -m_cameraDestroyDist || screenPoint.y >= 1.0 + m_cameraDestroyDist;

            if (rubble.transform.position.y < -50.0f || offScreen)
            {
                Destroy(rubble.gameObject);
            }
        }

        if (!rubbleExists)
            Destroy(gameObject);
    }


    public void ApplyExplosionForce(Vector3 forceLoc, float forceVal, float maxDist)
    {
        foreach (var bodies in m_myChildren)
        {
            bodies.AddExplosionForce(forceVal, forceLoc, maxDist);
        }
    }
    
}
