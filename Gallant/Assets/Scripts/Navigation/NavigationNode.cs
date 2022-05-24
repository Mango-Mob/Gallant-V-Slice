using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationNode : MonoBehaviour
{
    public FloorData m_myFloor;
    public SceneData m_myData;
    public int m_myIndex = 0;
    public int m_myDepth = 0;

    private bool IsCompleted = false;
    private Image mainIconLoc;
    public struct Connection
    {
        public LineRenderer render;
        public NavigationNode other;

        public Vector2 posA;
        public Vector2 posB;
        public float mag { get { return Vector2.Distance(posA, posB); } }
    }
    public List<Connection> m_myConnections { get; private set; } = new List<Connection>();

    public void AddLink(NavigationNode node, GameObject linePrefab)
    {
        //Create Line renderer
        GameObject lineObject = GameObject.Instantiate(linePrefab, transform);
        LineRenderer lineRend = lineObject.GetComponent<LineRenderer>();
        lineObject.SetActive(true);

        //First position
        Vector3 position1 = new Vector3(0, 0, 1);
        lineRend.SetPosition(0, position1);

        //Second position
        Vector3 position2 = (node.transform as RectTransform).localPosition - (transform as RectTransform).transform.localPosition;
        position2.z = 1;
        lineRend.SetPosition(1, position2);

        Connection newConnection = new Connection();
        newConnection.other = node;
        newConnection.render = lineRend;
        newConnection.posA = (transform as RectTransform).localPosition + lineRend.GetPosition(0);
        newConnection.posB = (transform as RectTransform).localPosition + lineRend.GetPosition(1);
        m_myConnections.Add(newConnection);
    }

    public void Update()
    {
        for (int i = 0; i < m_myConnections.Count; i++)
        {
            Vector3 position2 = (m_myConnections[i].other.transform as RectTransform).localPosition - (transform as RectTransform).transform.localPosition;
            position2.z = 1;
            m_myConnections[i].render.SetPosition(1, position2);
        }
    }

    public void ActivateMyConnections()
    {
        foreach (var connection in m_myConnections)
        {
            connection.other.GetComponent<Button>().interactable = true;
        }

        ColorBlock colors = GetComponent<Button>().colors;
        colors.disabledColor = new Color(255, 193, 0, 0.8f);
        GetComponent<Button>().colors = colors;
        GetComponent<Button>().interactable = false;

        if(m_myConnections.Count > 0)
            EventSystem.current.SetSelectedGameObject(m_myConnections[0].other.gameObject);
    }

    public void DeactivateMyConnections()
    {
        foreach (var connection in m_myConnections)
        {
            connection.other.GetComponent<Button>().interactable = false;
        }
        ColorBlock colors = GetComponent<Button>().colors;
        colors.disabledColor = new Color(200, 200, 200, 0.5f);
        GetComponent<Button>().colors = colors;
        GetComponent<Button>().interactable = false;
    }

    public void Navigate()
    {
        NavigationManager.Instance.MovePlayerIconTo(transform.position, LoadMyScene);
        NavigationManager.Instance.DisableAll();
        GetComponent<Button>().interactable = false;
    }

    public void LoadMyScene()
    {
        NavigationManager.Instance.UpdateMap(m_myIndex);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().StorePlayerInfo();
        LevelManager.Instance.LoadNewLevel(m_myData.sceneToLoad);
    }

    public void MarkCompleted()
    {
        IsCompleted = true;
        mainIconLoc.sprite = m_myData.sceneCompleteIcon;
    }

    public static GameObject CreateNode(SceneData data, Transform parent)
    {
        GameObject nodeObj = new GameObject();
        GameObject nodeImage = new GameObject();
        GameObject nodeBackground = new GameObject();
        nodeBackground.transform.SetParent(nodeObj.transform);
        nodeImage.transform.SetParent(nodeObj.transform);

        nodeObj.name = $"NavNode ({data.name})";
        nodeObj.AddComponent<RectTransform>();
        nodeObj.transform.SetParent(parent);
        nodeObj.transform.localPosition = Vector3.zero;
        nodeObj.transform.localRotation = Quaternion.identity;
        nodeObj.transform.localScale = Vector3.one;

        nodeObj.AddComponent(typeof(Button));

        ColorBlock temp = nodeObj.GetComponent<Button>().colors;
        temp.selectedColor = Color.cyan;
        temp.disabledColor = Color.gray;
        nodeObj.GetComponent<Button>().colors = temp;

        NavigationNode nav = nodeObj.AddComponent<NavigationNode>();
        nav.m_myFloor = null;
        nav.m_myData = data;

        nodeBackground.AddComponent<Image>().sprite = nav.m_myData.iconBack;

        nodeObj.GetComponent<Button>().onClick.AddListener(nodeObj.GetComponent<NavigationNode>().Navigate);
        nodeObj.GetComponent<Button>().interactable = false;
        nav.mainIconLoc = nodeImage.AddComponent<Image>();
        nodeObj.GetComponent<Button>().targetGraphic = nav.mainIconLoc;
        nodeImage.GetComponent<Image>().sprite = nav.m_myData.sceneIcon;

        return nodeObj;
    }

    public static GameObject CreateNode(FloorData data, Transform parent)
    {
        GameObject nodeObj = new GameObject();
        GameObject nodeImage = new GameObject();
        GameObject nodeBackground = new GameObject();
        nodeBackground.transform.SetParent(nodeObj.transform);
        nodeImage.transform.SetParent(nodeObj.transform);

        nodeObj.name = $"NavNode ({data.name})";
        nodeObj.AddComponent<RectTransform>();
        nodeObj.transform.SetParent(parent);
        nodeObj.transform.localPosition = Vector3.zero;
        nodeObj.transform.localRotation = Quaternion.identity;
        nodeObj.transform.localScale = Vector3.one;

        nodeObj.AddComponent(typeof(Button));

        ColorBlock temp = nodeObj.GetComponent<Button>().colors;
        temp.selectedColor = Color.cyan;
        temp.disabledColor = Color.gray;
        nodeObj.GetComponent<Button>().colors = temp;

        NavigationNode nav = nodeObj.AddComponent<NavigationNode>();
        nav.m_myFloor = data;
        nav.m_myData = data.GetScene();

        nodeBackground.AddComponent<Image>().sprite = nav.m_myData.iconBack;

        nodeObj.GetComponent<Button>().onClick.AddListener(nodeObj.GetComponent<NavigationNode>().Navigate);
        nodeObj.GetComponent<Button>().interactable = false;
        nav.mainIconLoc = nodeImage.AddComponent<Image>();
        nodeObj.GetComponent<Button>().targetGraphic = nav.mainIconLoc;
        nodeImage.GetComponent<Image>().sprite = nav.m_myData.sceneIcon;

        return nodeObj;
    }

    public void OnDrawGizmos()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawLine(transform.position, transform.position + transform.right * 5);
    }

}
