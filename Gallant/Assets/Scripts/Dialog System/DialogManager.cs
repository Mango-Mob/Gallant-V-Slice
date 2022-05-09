using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public enum DialogResult
{
    PROGRESS,
    TRANSFER,
    INTERACT,
    END, 
}

public class DialogManager : Singleton<DialogManager>
{
    private struct Options
    {
        public DialogResult onClickResult;
        public int otherData;
    }

    public List<UnityEvent> m_interact;
    public UnityEvent m_onDialogFinish;
    public GameObject m_defaultSelected;

    private Player_Controller m_player; 

    [SerializeField] private GameObject m_window;
    [SerializeField] private Text m_characterName;

    [SerializeField] private Text m_dialog;
    [SerializeField] private Button[] m_options;
    private DialogOption[] m_optionResults;
    [SerializeField] private Image m_characterBody;
    [SerializeField] private Image m_characterFace;

    [SerializeField] private Image[] gamepadButtons;
    private CharacterData m_activeCharacter;
    private DialogFile m_file;

    public TextAsset m_testFile;
    private int m_currentScene = 0;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        m_optionResults = new DialogOption[m_options.Length];
        m_player = FindObjectOfType<Player_Controller>();
        Hide();

        for (int i = 0; i < 4; i++)
        {
            m_interact.Add(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(m_window.activeInHierarchy)
        {
            for (int i = 0; i < 3; i++)
            {
                gamepadButtons[i].enabled = InputManager.Instance.isInGamepadMode;
                if (InputManager.Instance.IsKeyDown(KeyType.NUM_ONE + i) || InputManager.Instance.IsKeyDown(KeyType.ALP_ONE + i) || InputManager.Instance.IsGamepadButtonDown(ButtonType.UP + i, 0))
                {
                    if (m_options[i].interactable)
                        m_options[i].onClick?.Invoke();
                }
            }
        }
    }

    public void Show()
    {
        m_window.SetActive(true);
        m_player.m_isDisabledInput = true;
    }
    public void Hide()
    {
        m_window.SetActive(false);
        m_player.m_isDisabledInput = false;
    }

    public void ProcessOption(int i)
    {
        if(i < m_optionResults.Length)
        {
            if (m_optionResults[i] == null)
                return;

            switch (m_optionResults[i].result)
            {
                case DialogResult.PROGRESS:
                    m_currentScene = m_optionResults[i].nextDialog;
                    LoadScene(m_optionResults[i].nextDialog);
                    break;
                case DialogResult.TRANSFER:
                    m_currentScene = m_optionResults[i].nextDialog - 1;
                    LoadScene(m_optionResults[i].nextDialog - 1);
                    break;
                case DialogResult.INTERACT:
                    m_currentScene = m_optionResults[i].nextDialog;
                    LoadScene(m_optionResults[i].nextDialog);
                    m_interact[Mathf.Max(0, m_optionResults[i].interact)].Invoke();
                    break;
                case DialogResult.END:
                    m_onDialogFinish?.Invoke();
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
            SetCharacter(Resources.Load<CharacterData>(m_file.m_characterFile));
            m_currentScene = 0;
            LoadScene(m_currentScene);
        }

        for (int i = 0; i < 4; i++)
        {
            m_interact[i] = null;
        }
    }

    public void SetCharacter(CharacterData data)
    {
        m_activeCharacter = data;
        if (data == null)
        {
            m_characterBody.gameObject.SetActive(false);
            m_characterFace.gameObject.SetActive(false);
            m_characterName.transform.parent.gameObject.SetActive(false);
        }
        else
        {
            m_characterBody.gameObject.SetActive(true);
            m_characterFace.gameObject.SetActive(true);
            m_characterName.transform.parent.gameObject.SetActive(true);
            m_characterBody.sprite = m_activeCharacter.m_characterBody[0];
            m_characterFace.sprite = m_activeCharacter.m_characterFace[0];
            m_characterName.text = m_activeCharacter.m_names[0];
        }
    }
    public void SetDialogText(string text)
    {
        m_dialog.text = text;
    }
    public void SetButtonOption(int position, string buttonText, UnityAction interact)
    {
        if (position > 4)
            return;
        
        m_options[position].GetComponentInChildren<Text>().text = buttonText;

        if (interact != null)
        {
            m_optionResults[position] = new DialogOption(m_currentScene,
                    "dynamic",
                    "INTERACT",
                    position.ToString()
                    );

            m_options[position].interactable = true;

            m_interact[position] = new UnityEvent();
            m_interact[position].AddListener(interact);
        }
        else
        {
            m_options[position].interactable = false;
        }
    }

    private void LoadScene(int index)
    {
        if (m_file != null)
        {
            if (m_file.m_list.Count <= index)
            {
                m_onDialogFinish?.Invoke();
                Hide();
                return;
            }

            int nameId = m_file.m_list[index].nameID;
            int bodyId = m_file.m_list[index].bodyID;
            int faceID = m_file.m_list[index].faceID;

            m_characterName.text = m_activeCharacter.m_names[Mathf.Min(nameId, m_activeCharacter.m_names.Length - 1)];
            m_characterBody.sprite = m_activeCharacter.m_characterBody[Mathf.Min(bodyId, m_activeCharacter.m_characterBody.Length - 1)];
            m_characterFace.sprite = m_activeCharacter.m_characterFace[Mathf.Min(faceID, m_activeCharacter.m_characterFace.Length - 1)];

            SetDialogText(m_file.m_list[index].m_dialog);

            for (int i = 0; i < m_options.Length; i++)
            {
                if (i < m_file.m_list[index].results.Count)
                {
                    m_optionResults[i] = new DialogOption(m_currentScene,
                    m_file.m_list[index].results[i].resultText,
                    m_file.m_list[index].results[i].resultType,
                    m_file.m_list[index].results[i].other
                    );

                    m_options[i].GetComponentInChildren<Text>().text = m_file.m_list[index].results[i].resultText;
                    if (m_file.m_list[index].results[i].resultType == "INTERACT" && m_interact != null)
                    {
                        m_options[i].interactable = true;
                    }
                    else if (m_file.m_list[index].results[i].resultType != "INTERACT")
                    {
                        m_options[i].interactable = true;
                    }
                    else
                    {
                        m_options[i].interactable = false;
                    }
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
