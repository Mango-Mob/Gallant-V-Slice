using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MeshFader : MonoBehaviour
{
    private Camera m_mainCamera;

    private List<Collider> m_collidersSaved = new List<Collider>();
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


        List<RaycastHit> hits = new List<RaycastHit>(Physics.SphereCastAll(m_mainCamera.transform.position, 0.5f, m_mainCamera.transform.forward, 150f, 1 << LayerMask.NameToLayer("Environment")));
        List<Collider> toRemove = new List<Collider>(m_collidersSaved);
        m_collidersSaved.Clear();
        foreach (var hit in hits)
        {
            if (player.distance > hit.distance && hit.collider.GetComponent<MeshFade>() != null)
            {
                //Found valid
                m_collidersSaved.Add(hit.collider);
                if(toRemove.Contains(hit.collider))
                {
                    toRemove.Remove(hit.collider);
                }
                else
                {
                    //Start Fade Out
                    hit.collider.GetComponent<MeshFade>().FadeOut();
                }
            }
        }

        foreach (var old in toRemove)
        {
            //Start Fade In
            old.GetComponent<MeshFade>().FadeIn();
        }
    }
}
