using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NavigationManager : SingletonPersistent<NavigationManager>
{
    public GameObject m_node;
    public NavigationNode m_rootNode;
    public NavigationNode m_endNode;

    public SceneData[] m_sceneData;

    public int index;

    public float cameraSpeed = 10f;
    public float width;
    private float height;

    private List<NavigationNode> m_activeNodes = new List<NavigationNode>();
    private Camera m_myCamera;

    protected override void Awake()
    {
        base.Awake();
        m_myCamera = GetComponentInChildren<Camera>();
        m_myCamera.enabled = false;
        index = 0;
    }

    public void Start()
    {
        Generate(3, 2, 5);
        UpdateMap();
        ConstructScene();
    }

    public void Update()
    {
        if (InputManager.Instance.IsKeyDown(KeyType.J))
        {
            SetVisibility(!m_myCamera.enabled);
        }

        if(m_myCamera.enabled)
        {
            float scrollDelta = InputManager.Instance.GetMouseScrollDelta();
            if (scrollDelta != 0)
            {
                float y = m_myCamera.transform.localPosition.y;
                y = Mathf.Clamp(y + scrollDelta * cameraSpeed * Time.deltaTime, 0, height);
                m_myCamera.transform.localPosition = new Vector3(0, y, -10);
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        ConstructScene();
    }

    public void SetVisibility(bool status)
    {
        m_myCamera.enabled = status;
        if (m_myCamera.enabled)
        {
            m_myCamera.transform.localPosition = new Vector3(0, 0, -10);
        }
        HUDManager.Instance.gameObject.SetActive(!m_myCamera.enabled);
    }

    public void UpdateMap()
    {
        foreach (var node in m_activeNodes)
        {
            node.GetComponent<Button>().interactable = false;
        }
        foreach (var nextNodes in m_activeNodes[index].otherNodes)
        {
            nextNodes.GetComponent<Button>().interactable = true;
        }
        
    }

    public void ConstructScene()
    {
        if(m_activeNodes.Count > 0 && index < m_activeNodes.Count)
            Instantiate(m_activeNodes[index].data.prefabToLoad, Vector3.zero, Quaternion.identity);
    }

    public void Generate(uint sectDiv, uint minPerSect, uint maxPerSect)
    {
        Clear();
        height = (m_endNode.transform as RectTransform).localPosition.y - (m_rootNode.transform as RectTransform).transform.localPosition.y;

        float heightStep = height / (sectDiv + 1);

        m_activeNodes.Add(m_rootNode);
        List<NavigationNode> prevNodes = new List<NavigationNode>(m_activeNodes);

        //For each mid section
        for (int i = 1; i < sectDiv + 1; i++)
        {
            List<NavigationNode> nextNodes = new List<NavigationNode>();
            int nodesToCreate = 0;
            if((Random.Range(0, 1000) > 1000 * 0.5f && prevNodes.Count != maxPerSect) || prevNodes.Count <= minPerSect)
            {
                nodesToCreate = Random.Range(prevNodes.Count, (int)maxPerSect+1);
            }
            else
            {
                nodesToCreate = Random.Range((int)minPerSect, prevNodes.Count);
            }
            float widthStep = width / nodesToCreate;
            for (int j = 0; j < nodesToCreate; j++)
            {
                //Random quantity;
                GameObject newNodeObj = GameObject.Instantiate(m_node, transform);
                (newNodeObj.transform as RectTransform).localPosition = new Vector3(widthStep * (j+1) - (width/ (nodesToCreate * 0.5f)), heightStep * i, 0);
                NavigationNode newNavNode = newNodeObj.GetComponent<NavigationNode>();
                newNavNode.m_myIndex = m_activeNodes.Count;
                newNavNode.data = m_sceneData[Random.Range(0, m_sceneData.Length)];
                nextNodes.Add(newNavNode);
                m_activeNodes.Add(newNavNode);
                foreach (var prev in prevNodes)
                {
                    prev.AddLink(newNavNode);
                }
            }
            prevNodes = nextNodes;
        }
        foreach (var prev in prevNodes)
        {
            prev.AddLink(m_endNode);
        }
        m_endNode.m_myIndex = m_activeNodes.Count;
        m_activeNodes.Add(m_endNode);
    }
    public void Clear()
    {
        for (int i = m_activeNodes.Count - 1; i >= 0; i--)
        {
            if(m_activeNodes[i] != m_endNode || m_activeNodes[i] != m_rootNode)
            {
                Destroy(m_activeNodes[i].gameObject);
            }
            m_activeNodes.RemoveAt(i);
        }
        m_activeNodes.Clear();
    }
    public void OnDrawGizmos()
    {
        float height = (m_endNode.transform as RectTransform).position.y - (m_rootNode.transform as RectTransform).transform.position.y;
        Gizmos.DrawWireCube((transform as RectTransform).position + new Vector3(0, height / 2f, 0), new Vector3(width * transform.localScale.x, height, 0));
    }
}
