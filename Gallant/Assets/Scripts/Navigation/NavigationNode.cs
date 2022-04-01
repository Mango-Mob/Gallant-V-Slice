using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NavigationNode : MonoBehaviour
{
    public SceneData data;
    public GameObject m_linePrefab;
    public int m_myIndex = 0;

    private List<LineRenderer> lineLinks = new List<LineRenderer>();

    public List<NavigationNode> otherNodes { get; private set; } = new List<NavigationNode>();

    public void AddLink(NavigationNode node)
    {
        Vector3 position1 = new Vector3(0, 0, 1);
        Vector3 position2 = (node.transform as RectTransform).localPosition - (transform as RectTransform).transform.localPosition;
        position2.z = 1;

        otherNodes.Add(node);
        GameObject lineObject = GameObject.Instantiate(m_linePrefab, transform);
        lineObject.SetActive(true);
        LineRenderer lineRend = lineObject.GetComponent<LineRenderer>();
        lineRend.SetPosition(0, position1);
        lineRend.SetPosition(1, position2);
        lineLinks.Add(lineRend);
    }

    public void LoadMyScene()
    {
        NavigationManager.Instance.index = m_myIndex;
        NavigationManager.Instance.SetVisibility(false);
        LevelManager.Instance.LoadNewLevel("SceneLoadTest");
        NavigationManager.Instance.UpdateMap();
    }
}
