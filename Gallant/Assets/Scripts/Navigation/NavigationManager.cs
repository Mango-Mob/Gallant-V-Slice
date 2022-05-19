using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class NavigationManager : SingletonPersistent<NavigationManager>
{
    public GameObject m_linePrefab;
    public NavigationNode m_rootNode;
    public NavigationNode m_endNode;
    public GameObject m_mapObj;

    public int index = -1;
    public Vector2 iconNoise;

    public float cameraSpeed = 10f;

    public LevelData m_generatedLevel { get; private set; } = null;

    [SerializeField] private Button m_mouseInput;
    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;

    public bool IsVisible { get { return m_myCamera.enabled && m_myCanvas.enabled; } }
    private List<NavigationNode> m_activeNodes = new List<NavigationNode>();
    private Camera m_myCamera;
    private Canvas m_myCanvas;

    public Image m_playerIcon;

    private SoloAudioAgent m_audio;
    private bool m_canQuit = true;
    protected override void Awake()
    {
        base.Awake();
        m_audio = GetComponent<SoloAudioAgent>();
        m_myCamera = GetComponentInChildren<Camera>();
        m_myCamera.gameObject.SetActive(false);
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
        if (InputManager.Instance == null)
            return;

        if (IsVisible && m_canQuit && (InputManager.Instance.IsKeyDown(KeyType.ESC) || InputManager.Instance.IsKeyDown(KeyType.Q) || InputManager.Instance.IsGamepadButtonDown(ButtonType.EAST, 0)))
        {
            SetVisibility(false);
        }

        if(IsVisible)
        {
            if (InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject == null)
            {
                EventSystem.current.SetSelectedGameObject(m_rootNode.m_myConnections[0].other.gameObject);
            }
            else if (!InputManager.Instance.isInGamepadMode && EventSystem.current.currentSelectedGameObject != null)
            {
                EventSystem.current.SetSelectedGameObject(null);
            }
        }

        m_mouseInput.gameObject.SetActive(m_canQuit && !InputManager.Instance.isInGamepadMode);
        m_keyboardInput.gameObject.SetActive(m_canQuit && !InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(m_canQuit && InputManager.Instance.isInGamepadMode);

        if (m_myCamera.enabled)
        {
            float scrollDelta = 0;
            if (InputManager.Instance.isInGamepadMode)
            {
                scrollDelta = InputManager.Instance.GetGamepadStick(StickType.RIGHT, 0).y * 15f;
            }
            else if (InputManager.Instance.IsMouseButtonPressed(MouseButton.LEFT))
            {
                scrollDelta = -InputManager.Instance.GetMouseDelta().y * 10f;
            }else
            {
                scrollDelta = InputManager.Instance.GetMouseScrollDelta() * 5f;
            }
           
            if (scrollDelta != 0 && m_generatedLevel != null)
            {
                float y = m_myCamera.transform.localPosition.y;
                y = Mathf.Clamp(y + scrollDelta * cameraSpeed * Time.deltaTime, 0, m_generatedLevel.m_height);
                m_myCamera.transform.localPosition = new Vector3(0, y, -10);
            }
        }
    }

    private void OnLevelWasLoaded(int level)
    {
        ConstructScene();
        Instance.SetVisibility(false);
    }

    public void SetVisibility(bool status, bool canQuit = true)
    {
        m_myCamera.gameObject.SetActive(status);
        m_myCanvas.enabled = status;
        m_canQuit = canQuit;

        if (m_myCamera.enabled)
        {
            if(index > 0 && m_activeNodes.Count > 0)
                m_myCamera.transform.localPosition = new Vector3(0, m_activeNodes[index].transform.localPosition.y, -10);
            else
                m_myCamera.transform.localPosition = new Vector3(0, 0, -10);
        }
        if (HUDManager.Instance != null)
            HUDManager.Instance.gameObject?.SetActive(!m_myCamera.gameObject.activeInHierarchy);

        if(GameManager.Instance.m_player != null)
            GameManager.Instance.m_player.GetComponent<Player_Controller>().m_isDisabledInput = status;

        if(status)
            m_activeNodes[index].ActivateMyConnections();
        else
            EventSystem.current.SetSelectedGameObject(null);
    }

    public void MovePlayerIconTo(Vector3 position, System.Action loadToScene)
    {
        m_audio.Play();
        StartCoroutine(MovePlayerIcon(position, loadToScene));
    }

    private IEnumerator MovePlayerIcon(Vector3 position, System.Action loadToScene)
    {
        m_canQuit = false;
        Vector3 start = m_playerIcon.transform.position;
        float speed = 15f;
        float distance = Vector3.Distance(start, position);
        float dt = speed / distance;
        float t = 0;
        while(t < 1.0f)
        {
            t += dt * Time.deltaTime;
            m_playerIcon.transform.position = Vector3.Lerp(start, position, t);
            yield return new WaitForEndOfFrame();
        }
        m_canQuit = true;
        loadToScene.Invoke();
        yield return null;
    }

    public void UpdateMap(int newIndex)
    {
        if(index >= 0)
            m_activeNodes[index].DeactivateMyConnections();
        index = newIndex;
    }

    public int GetFloor()
    {
        return m_activeNodes[index].m_myDepth;
    }

    public void ConstructScene()
    {
        if(m_activeNodes.Count > 0 && index < m_activeNodes.Count && index >= 0 )
        {
            if (SceneManager.GetActiveScene().name == m_activeNodes[index].m_myData.sceneToLoad && m_activeNodes[index].m_myData.prefabToLoad != null)
            {
                m_activeNodes[index].ActivateMyConnections();
                Instantiate(m_activeNodes[index].m_myData.prefabToLoad, Vector3.zero, Quaternion.identity);
                if(m_activeNodes[index].m_myData.prefabPropsToLoad != null && m_activeNodes[index].m_myData.prefabPropsToLoad.Count > 0)
                {
                    int select = Random.Range(0, m_activeNodes[index].m_myData.prefabPropsToLoad.Count);
                    Instantiate(m_activeNodes[index].m_myData.prefabPropsToLoad[select], Vector3.zero, Quaternion.identity);
                }
            }
        }
    }

    public void Generate(LevelData data)
    {
        Clear(false);

        m_generatedLevel = data;

        NarrativeManager.Instance.Refresh();
        float heightStep = m_generatedLevel.m_height / (data.m_levelFloors.Count);
        index = 0;

        if (m_rootNode == null)
            SetRoot(data.m_root);

        m_activeNodes.Add(m_rootNode);

        m_playerIcon.transform.position = m_activeNodes[0].transform.position;

        m_rootNode.MarkCompleted();
        List<NavigationNode> prevNodes = new List<NavigationNode>(m_activeNodes);
        GameObject newNodeObj;
        //For each mid section
        for (int i = 0; i < data.m_levelFloors.Count - 1; i++)
        {
            List<NavigationNode> nextNodes = new List<NavigationNode>();
            int nodesToCreate = 0;
            float probOfIncrease = data.Evaluate(i);
            if((Random.Range(0, 10000) < 10000 * probOfIncrease && prevNodes.Count != data.m_maxRoomCount) || prevNodes.Count <= data.m_minRoomCount)
            {
                //Increase 
                int min = Mathf.Min(prevNodes.Count + 1, (int)data.m_maxRoomCount);
                int max = Mathf.Min(prevNodes.Count + 2, (int)data.m_maxRoomCount);
                nodesToCreate = Random.Range(min, max);
            }
            else
            {
                //Decrease
                int min = Mathf.Max(prevNodes.Count - 2, (int)data.m_minRoomCount);
                int max = Mathf.Max(prevNodes.Count - 1, (int)data.m_minRoomCount);
                nodesToCreate = Random.Range(min, max + 1);
            }
            float widthStep = m_generatedLevel.m_width / nodesToCreate;
            for (int j = 0; j < nodesToCreate; j++)
            {
                //Random quantity;
                newNodeObj = NavigationNode.CreateNode(Extentions.GetFromList<SceneData>(data.m_levelFloors[i].potentialScenes), m_mapObj.transform);
                float xPos = widthStep * (j+0.5f) - (m_generatedLevel.m_width / 2) + Random.Range(-iconNoise.x, iconNoise.x);
                (newNodeObj.transform as RectTransform).localPosition = new Vector3(xPos, heightStep * (i + 1) + Random.Range(-iconNoise.y, iconNoise.y), 0);
                NavigationNode newNavNode = newNodeObj.GetComponent<NavigationNode>();
                newNavNode.m_myIndex = m_activeNodes.Count;
                newNavNode.m_myDepth = i;
                nextNodes.Add(newNavNode);
                m_activeNodes.Add(newNavNode);

                foreach (var prev in prevNodes)
                {
                    prev.AddLink(newNavNode, m_linePrefab);
                }
            }
            prevNodes = nextNodes;
        }

        //Create end node
        newNodeObj = NavigationNode.CreateNode(Extentions.GetFromList<SceneData>(data.m_levelFloors[(data.m_levelFloors.Count - 1)].potentialScenes), transform);
        m_endNode = newNodeObj.GetComponent<NavigationNode>();
        m_endNode.m_myIndex = m_activeNodes.Count;
        m_endNode.m_myDepth = data.m_levelFloors.Count - 1;
        m_endNode.transform.localPosition = new Vector3(0, m_generatedLevel.m_height, 0);
        
        m_activeNodes.Add(m_endNode);
        foreach (var prev in prevNodes)
        {
            prev.AddLink(m_endNode, m_linePrefab);
        }
        
        Reposition();
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
        m_endNode.transform.localPosition = new Vector3(0, m_generatedLevel.m_height, 0);
    }

    public void Reposition()
    {
        float radius = 5;
        for (int i = 0; i < m_activeNodes.Count; i++)
        {
            NavigationNode a = m_activeNodes[i];
            for (int j = 0; j < m_activeNodes.Count; j++)
            {
                NavigationNode b = m_activeNodes[j];
                if (i == j)
                    break;

                if (Extentions.CircleVsCircle(a.transform.position, b.transform.position, radius, radius))
                {
                    Vector3 forwardVect = (b.transform.position - a.transform.position) * 0.5f;

                    a.transform.position -= forwardVect.normalized * (forwardVect.magnitude);
                    b.transform.position += forwardVect.normalized * (forwardVect.magnitude);
                }
            }
        }
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

    public void DisableAll()
    {
        m_activeNodes[index].DeactivateMyConnections();
    }

    public void SetRoot(SceneData data)
    {
        GameObject nodeObj = NavigationNode.CreateNode(data, m_mapObj.transform);
        nodeObj.transform.localPosition = new Vector3(0, 0, 0);
        NavigationNode nodeNav = nodeObj.GetComponent<NavigationNode>();
        nodeNav.m_myIndex = m_activeNodes.Count;
        nodeNav.m_myDepth = -1;

        if (m_rootNode != null)
            Destroy(m_endNode.gameObject);

        m_rootNode = nodeNav;
    }

    public void SetEnd(SceneData data)
    {
        GameObject nodeObj = NavigationNode.CreateNode(data, m_mapObj.transform);
        nodeObj.transform.localPosition = new Vector3(0, m_generatedLevel.m_height, 0);
        NavigationNode nodeNav = nodeObj.GetComponent<NavigationNode>();
        nodeNav.m_myIndex = m_activeNodes.Count;

        if (m_endNode != null)
            Destroy(m_endNode.gameObject);

        m_endNode = nodeNav;
    }

    public void Clear(bool clearAll = false)
    {
        m_generatedLevel = null;
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
        if(m_generatedLevel != null)
            Gizmos.DrawWireCube((transform as RectTransform).position + new Vector3(0, m_generatedLevel.m_height / 2f, 0), new Vector3(m_generatedLevel.m_width * transform.localScale.x, m_generatedLevel.m_height, 0));
    }
}
