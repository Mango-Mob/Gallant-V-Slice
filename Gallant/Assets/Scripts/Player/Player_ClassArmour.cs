using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_ClassArmour : MonoBehaviour
{
    public Player_Controller playerController { private set; get; }

    [Header("Slots")]
    public GameObject m_headSlot;
    private MeshRenderer m_headMeshRenderer;
    private MeshFilter m_headMeshFilter;

    // Start is called before the first frame update
    private void Awake()
    {
        playerController = GetComponent<Player_Controller>();

        m_headMeshRenderer = m_headSlot.GetComponentInChildren<MeshRenderer>();
        m_headMeshFilter = m_headSlot.GetComponentInChildren<MeshFilter>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SetClassArmour(ClassData _class)
    {
        m_headMeshRenderer.material = _class.helmetMaterial;
        m_headMeshFilter.mesh = _class.helmetModel;
    }
}
