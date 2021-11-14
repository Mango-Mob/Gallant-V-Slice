using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFader : MonoBehaviour
{
    private Camera m_mainCamera;

    void Awake()
    {
        m_mainCamera = this.GetComponent<Camera>();
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit player;
        Physics.Raycast(m_mainCamera.transform.position, m_mainCamera.transform.forward, out player, 150f,  1 << LayerMask.NameToLayer("Player"));

        RaycastHit[] hits = Physics.SphereCastAll(m_mainCamera.transform.position, 0.5f, m_mainCamera.transform.forward, 150f, 1 << LayerMask.NameToLayer("Environment"));
        foreach (var hit in hits)
        {
            if(player.distance > hit.distance)
            {
                hit.collider.GetComponent<MeshFade>()?.Fade();
            }
        }
    }
}
