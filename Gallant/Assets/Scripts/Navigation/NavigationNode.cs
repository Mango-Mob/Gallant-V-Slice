using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class NavigationNode : MonoBehaviour
{
    public SceneData m_myData;
    public int m_myIndex = 0;
    public int m_myDepth = 0;
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

    public void LoadMyScene()
    {
        NavigationManager.Instance.UpdateMap(m_myIndex);
        NavigationManager.Instance.SetVisibility(false);
        GameManager.Instance.m_player.GetComponent<Player_Controller>().StorePlayerInfo();
        LevelManager.Instance.LoadNewLevel(m_myData.sceneToLoad);
    }

    public static GameObject CreateNode(SceneData data, Transform parent)
    {
        GameObject nodeObj = new GameObject();
        nodeObj.name = $"NavNode ({data.name})";
        nodeObj.transform.SetParent(parent);
        nodeObj.transform.localPosition = Vector3.zero;
        nodeObj.transform.localRotation = Quaternion.identity;
        nodeObj.transform.localScale = Vector3.one;

        nodeObj.AddComponent(typeof(Button));
        nodeObj.GetComponent<Button>().targetGraphic = nodeObj.AddComponent(typeof(Image)) as Image;
        

        nodeObj.GetComponent<Image>().sprite = data.sceneIcon;

        nodeObj.AddComponent<NavigationNode>();
        nodeObj.GetComponent<NavigationNode>().m_myData = data;
        nodeObj.GetComponent<Button>().onClick.AddListener(nodeObj.GetComponent<NavigationNode>().LoadMyScene);
        nodeObj.GetComponent<Button>().interactable = false;
        return nodeObj;
    }

    
}