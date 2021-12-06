using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DebugMenu : MonoBehaviour
{
    private static DebugMenu _instance = null;

    [SerializeField] private GameObject[] m_panelArray;
    [SerializeField] private Button[] m_buttonArray;


    [Header("Scene Content")]
    [SerializeField] private Dropdown m_sceneList;

    private void Awake()
    {
        if(_instance != null && _instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    // Start is called before the first frame update
    void Start()
    {
        m_sceneList.ClearOptions();
        List<string> options = new List<string>();
        
        for (int i = 0; i < SceneManager.sceneCountInBuildSettings; i++)
        {
            string path = SceneUtility.GetScenePathByBuildIndex(i);
            int start = path.LastIndexOf('/');
            int end = path.LastIndexOf('.');
            options.Add(path.Substring(start + 1, end - start - 1));
        }
        m_sceneList.AddOptions(options);
    }

    private void OnApplicationQuit()
    {
        Destroy(_instance.gameObject);    
    }

    // Update is called once per frame
    void Update()
    {
        if(InputManager.instance.IsKeyDown(KeyType.TILDE))
        {
            GetComponent<Animator>().SetTrigger("StateUpdate");
        }
    }

    public void LoadScene()
    {
        SceneManager.LoadScene(m_sceneList.value);
    }

    public void ShowPanel(int index)
    {
        if (m_panelArray.Length > 0 && m_buttonArray.Length > 0)
        {
            for (int i = 0; i < m_panelArray.Length; i++)
            {
                if (i == index)
                {
                    m_panelArray[i].SetActive(true);
                    EventSystem.current.SetSelectedGameObject(m_buttonArray[Mathf.Min(i, m_buttonArray.Length - 1)].gameObject);
                    continue;
                }
                else
                {
                    m_panelArray[i].SetActive(false);
                }
            }
        }
    }
}
