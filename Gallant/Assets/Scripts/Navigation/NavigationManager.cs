using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationManager : SingletonPersistent<NavigationManager>
{
    public GameObject m_linePrefab;
    public NavigationNode m_rootNode;
    public NavigationNode m_endNode;

    public SceneData[] m_sceneData;

    public int index = -1;
    public Vector2 iconNoise;

    public float cameraSpeed = 10f;
    public float width;
    public float height;

    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;

    public bool IsVisible { get { return m_myCamera.enabled && m_myCanvas.enabled; } }
    private List<NavigationNode> m_activeNodes = new List<NavigationNode>();
    private Camera m_myCamera;
    private Canvas m_myCanvas;

    protected override void Awake()
    {
        base.Awake();
        m_myCamera = GetComponentInChildren<Camera>();
        m_myCamera.enabled = false;
        m_myCanvas = GetComponent<Canvas>();
        m_myCanvas.enabled = false;
        index = -1;
    }

    public void Start()
    {
        //Generate(6, 2, 6);
        //UpdateMap(0);
        //ConstructScene();
    }

    public void Update()
    {
        if (IsVisible && (InputManager.Instance.IsKeyDown(KeyType.ESC) || InputManager.Instance.IsKeyDown(KeyType.Q) || InputManager.Instance.IsGamepadButtonDown(ButtonType.EAST, 0)))
        {
            SetVisibility(false);
        }

        m_keyboardInput.gameObject.SetActive(!InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(InputManager.Instance.isInGamepadMode);

        if (m_myCamera.enabled)
        {
            float scrollDelta = 0;
            if (InputManager.Instance.isInGamepadMode)
            {
                scrollDelta = InputManager.Instance.GetGamepadStick(StickType.RIGHT, 0).y * 10f;
            }
            else if (InputManager.Instance.IsMouseButtonPressed(MouseButton.LEFT))
            {
                scrollDelta = -InputManager.Instance.GetMouseDelta().y * 10f;
            }else
            {
                scrollDelta = InputManager.Instance.GetMouseScrollDelta() * 5f;
            }
           
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
        m_myCanvas.enabled = status;
        if (m_myCamera.enabled)
        {
            if(index > 0 && m_activeNodes.Count > 0)
                m_myCamera.transform.localPosition = new Vector3(0, m_activeNodes[index].transform.localPosition.y, -10);
            else
                m_myCamera.transform.localPosition = new Vector3(0, 0, -10);
        }
        HUDManager.Instance.gameObject.SetActive(!m_myCamera.enabled);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = status;
    }

    public void UpdateMap(int newIndex)
    {
        if(index >= 0)
            m_activeNodes[index].DeactivateMyConnections();
        if(newIndex >= 0)
            m_activeNodes[newIndex].ActivateMyConnections();
        index = newIndex;
    }
    public void ConstructScene()
    {
        if(m_activeNodes.Count > 0 && index < m_activeNodes.Count && index >= 0 )
        {
            if (SceneManager.GetActiveScene().name == m_activeNodes[index].m_myData.sceneToLoad && m_activeNodes[index].m_myData.prefabToLoad != null)
            {
                Instantiate(m_activeNodes[index].m_myData.prefabToLoad, Vector3.zero, Quaternion.identity);
            }
        }
    }

    public void Generate(uint sectDiv, uint minPerSect, uint maxPerSect)
    {
        Clear(false);
        float heightStep = height / (sectDiv + 1);

        if (m_rootNode == null)
            SetRoot(m_sceneData[Random.Range(0, m_sceneData.Length)]);

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
                GameObject newNodeObj = NavigationNode.CreateNode(m_sceneData[Random.Range(0, m_sceneData.Length)], transform);
                float xPos = widthStep * (j+0.5f) - (width/2) + Random.Range(-iconNoise.x, iconNoise.x);
                (newNodeObj.transform as RectTransform).localPosition = new Vector3(xPos, heightStep * i + Random.Range(-iconNoise.y, iconNoise.y), 0);
                NavigationNode newNavNode = newNodeObj.GetComponent<NavigationNode>();
                newNavNode.m_myIndex = m_activeNodes.Count;
                newNavNode.m_myDepth = i;
                newNavNode.m_myData = m_sceneData[Random.Range(0, m_sceneData.Length)];
                nextNodes.Add(newNavNode);
                m_activeNodes.Add(newNavNode);

                foreach (var prev in prevNodes)
                {
                    prev.AddLink(newNavNode, m_linePrefab);
                }
            }
            prevNodes = nextNodes;
        }
        m_endNode.m_myIndex = m_activeNodes.Count;
        m_endNode.transform.localPosition = new Vector3(0, height, 0);
        m_activeNodes.Add(m_endNode);
        foreach (var prev in prevNodes)
        {
            prev.AddLink(m_endNode, m_linePrefab);
        }
        CleanUp();
    }

    public void Generate(SceneData[] data, (int, int)[] connections)
    {
        Clear(true);
        foreach (var item in data)
        {
            GameObject nodeObj = NavigationNode.CreateNode(item, transform);
            nodeObj.transform.localPosition = item.navLocalPosition;
            NavigationNode nodeNav = nodeObj.GetComponent<NavigationNode>();
            nodeNav.m_myIndex = m_activeNodes.Count;
            m_activeNodes.Add(nodeNav);
        }

        for (int i = 0; i < connections.Length; i++)
        {
            m_activeNodes[connections[i].Item1].AddLink(m_activeNodes[connections[i].Item2], m_linePrefab);
        }
        m_rootNode = m_activeNodes[0];
        m_endNode = m_activeNodes[m_activeNodes.Count - 1];
        m_endNode.transform.localPosition = new Vector3(0, height, 0);
    }

    public void CleanUp()
    {
        foreach (var nodeA in m_activeNodes)
        {
            foreach (var nodeB in m_activeNodes)
            {
                //If node A and B are the same or don't have any connections
                if (nodeA == nodeB || nodeA.m_myConnections.Count == 0 || nodeB.m_myConnections.Count == 0)
                    continue;

                //If node A and B aren't on the same depth
                if (nodeA.m_myDepth != nodeB.m_myDepth)
                    continue;

                for (int i = nodeA.m_myConnections.Count - 1; i >= 0; i--)
                {
                    for (int j = nodeB.m_myConnections.Count - 1; j >= 0; j--)
                    {
                        //If connection A and B are going to the same target
                        if (nodeA.m_myConnections[i].other.m_myIndex == nodeB.m_myConnections[j].other.m_myIndex)
                            continue;

                        Vector2 result = new Vector2();
                        if(Extentions.LineIntersection(nodeA.m_myConnections[i].posA, nodeA.m_myConnections[i].posB, nodeB.m_myConnections[j].posA, nodeB.m_myConnections[j].posB, ref result))
                        {
                            if(nodeA.m_myConnections[i].mag >= nodeB.m_myConnections[j].mag)
                            {
                                Destroy(nodeA.m_myConnections[i].render.gameObject);
                                nodeA.m_myConnections.RemoveAt(i);
                                break;
                            }
                            else
                            {
                                Destroy(nodeB.m_myConnections[j].render.gameObject);
                                nodeB.m_myConnections.RemoveAt(j);
                                continue;
                            }
                        }
                    }
                }
            }
        }
    }

    public void SetRoot(SceneData data)
    {
        GameObject nodeObj = NavigationNode.CreateNode(data, transform);
        nodeObj.transform.localPosition = new Vector3(0, 0, 0);
        NavigationNode nodeNav = nodeObj.GetComponent<NavigationNode>();
        nodeNav.m_myIndex = m_activeNodes.Count;

        if (m_rootNode != null)
            Destroy(m_endNode.gameObject);

        m_rootNode = nodeNav;
    }

    public void SetEnd(SceneData data)
    {
        GameObject nodeObj = NavigationNode.CreateNode(data, transform);
        nodeObj.transform.localPosition = new Vector3(0, height, 0);
        NavigationNode nodeNav = nodeObj.GetComponent<NavigationNode>();
        nodeNav.m_myIndex = m_activeNodes.Count;

        if (m_endNode != null)
            Destroy(m_endNode.gameObject);

        m_endNode = nodeNav;
    }

    public void Clear(bool clearAll = false)
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

        if(clearAll)
        {
            if(m_rootNode != null)
            {
                Destroy(m_rootNode.gameObject);
                m_rootNode = null;
            }
            if(m_endNode != null)
            {
                Destroy(m_endNode.gameObject);
                m_endNode = null;
            }
        }
    }
    public void OnDrawGizmos()
    {
        Gizmos.DrawWireCube((transform as RectTransform).position + new Vector3(0, height / 2f, 0), new Vector3(width * transform.localScale.x, height, 0));
    }
}
