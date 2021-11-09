using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DialogDisplay : MonoBehaviour
{
    public UnityEvent m_interact;
    [SerializeField] private GameObject m_window;
    [SerializeField] private Text m_characterName;
    [SerializeField] private Text m_dialog;
    [SerializeField] private Button[] m_options;
    [SerializeField] private Image m_characterBody;
    [SerializeField] private Image m_characterFace;

    private DialogResult[] m_optionResult;
    private CharacterData m_activeCharacter;
    private DialogFile m_file;

    public TextAsset m_testFile;
    private int m_currentScene = 0;
    // Start is called before the first frame update
    void Awake()
    {
        m_optionResult = new DialogResult[m_options.Length];
        LoadDialog(m_testFile);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Show()
    {
        m_window.SetActive(true);
    }
    public void Hide()
    {
        m_window.SetActive(false);
    }

    public void ProcessOption(int i)
    {
        if(i < m_file.m_list[m_currentScene].results.Count)
        {
            DialogOption result = new DialogOption(m_currentScene,
                    m_file.m_list[m_currentScene].results[i].resultText,
                    m_file.m_list[m_currentScene].results[i].resultType,
                    m_file.m_list[m_currentScene].results[i].other
                    );
            switch (result.result)
            {
                case DialogResult.PROGRESS:
                    m_currentScene = result.nextDialog;
                    LoadScene(result.nextDialog);
                    break;
                case DialogResult.TRANSFER:
                    m_currentScene = result.nextDialog - 1;
                    LoadScene(result.nextDialog - 1);
                    break;
                case DialogResult.INTERACT:
                    m_interact.Invoke();
                    Hide();
                    break;
                case DialogResult.END:
                    Hide();
                    break;
                default:
                    break;
            }
        }
    }

    public void LoadDialog(TextAsset file)
    {
        m_file = JsonUtility.FromJson(file.text, typeof(DialogFile)) as DialogFile;
        if(m_file != null)
        {
            m_activeCharacter = Resources.Load<CharacterData>(m_file.m_characterFile);
            m_characterName.text = m_activeCharacter.m_name;
            m_currentScene = 0;
            LoadScene(m_currentScene);
        }
    }

    private void LoadScene(int index)
    {
        if (m_file != null)
        {
            int bodyId = m_file.m_list[index].bodyID;
            int faceID = m_file.m_list[index].faceID;

            Rect recBody = new Rect(0, 0, m_activeCharacter.m_characterBody[bodyId].width, m_activeCharacter.m_characterBody[bodyId].height);
            Rect recFace = new Rect(0, 0, m_activeCharacter.m_characterFace[bodyId].width, m_activeCharacter.m_characterFace[bodyId].height);

            m_characterBody.sprite = Sprite.Create(m_activeCharacter.m_characterBody[bodyId], recBody, new Vector2(0.5f, 0.5f));
            m_characterFace.sprite = Sprite.Create(m_activeCharacter.m_characterFace[faceID], recFace, new Vector2(0.5f, 0.5f));

            m_dialog.text = m_file.m_list[index].m_dialog;

            for (int i = 0; i < m_options.Length; i++)
            {
                if(i < m_file.m_list[index].results.Count)
                {
                    m_options[i].GetComponentInChildren<Text>().text = m_file.m_list[index].results[i].resultText;
                    m_options[i].interactable = true;
                }
                else
                {
                    m_options[i].GetComponentInChildren<Text>().text = "";
                    m_options[i].interactable = false;
                }
            }
        }
    }
}
