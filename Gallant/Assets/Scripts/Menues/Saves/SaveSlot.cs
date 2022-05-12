using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using System.IO;

public class SaveSlot : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    enum SaveSlotButtonState
    {
        NONE,
        EMPTY,
        BASIC,
        CONFIRMATION,
    }

    [Range(1, 4)] [SerializeField] private int m_slotNumber = 1;
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_currencyText;
    [SerializeField] private Image m_slotBackground;
    [SerializeField] private Sprite m_emptySlotSprite;

    [SerializeField] private GameObject m_buttonGroup;

    [Header("Selection Buttons")]
    [SerializeField] private GameObject[] m_basicButtons;
    [SerializeField] private GameObject[] m_emptySlotButtons;
    [SerializeField] private GameObject[] m_confirmButtons;

    private EventSystem eventSystem;
    private bool m_validSave = false;
    private bool m_isSelected = false;
    private SaveSlotButtonState m_currentState;

    // Start is called before the first frame update
    void Start()
    {
        eventSystem = FindObjectOfType<EventSystem>();
        AlterVisuals();
    }
    private void Update()
    {
        if (InputManager.Instance.isInGamepadMode)
        {
            bool checkSelected = (eventSystem.currentSelectedGameObject == gameObject);
            if (m_isSelected != checkSelected)
            {
                m_isSelected = checkSelected;
                Debug.Log($"Setting m_isSelected to {m_isSelected}.");
                SelectProcess();
            }

            switch (m_currentState)
            {
                case SaveSlotButtonState.EMPTY:
                    if (InputManager.Instance.IsGamepadButtonDown(ButtonType.SOUTH, 0)) // New Game
                    {
                        TryLoad();
                    }
                    break;
                case SaveSlotButtonState.BASIC:
                    if (InputManager.Instance.IsGamepadButtonDown(ButtonType.SOUTH, 0)) // Play
                    {
                        TryLoad();
                    }
                    if (InputManager.Instance.IsGamepadButtonDown(ButtonType.WEST, 0)) // Delete
                    {
                        SetButtonStates(SaveSlotButtonState.CONFIRMATION);
                    }
                    break;
                case SaveSlotButtonState.CONFIRMATION:
                    if (InputManager.Instance.IsGamepadButtonDown(ButtonType.SOUTH, 0)) // Confirm
                    {
                        DeleteSave();
                        SetButtonStates(SaveSlotButtonState.EMPTY);
                    }
                    if (InputManager.Instance.IsGamepadButtonDown(ButtonType.WEST, 0)) // Cancel
                    {
                        SetButtonStates(SaveSlotButtonState.BASIC);
                    }
                    break;
            } 
        }
    }
    public void OnPointerEnter(PointerEventData eventData)
    {
        m_isSelected = true;
        SelectProcess();
    }
    public void OnPointerExit(PointerEventData eventData)
    {
        m_isSelected = false;
        SelectProcess();
    }
    private void SelectProcess()
    {
        if (m_isSelected)
        {
            if (m_validSave)
                SetButtonStates(SaveSlotButtonState.BASIC);
            else
                SetButtonStates(SaveSlotButtonState.EMPTY);
        }
        else
        {
            SetButtonStates(SaveSlotButtonState.NONE);
        }
    }
    private void SetButtonStates(SaveSlotButtonState _state)
    {
        m_buttonGroup.SetActive(_state != SaveSlotButtonState.NONE);

        if (_state != SaveSlotButtonState.NONE)
        {
            foreach (var button in m_basicButtons)
            {
                button.SetActive(_state == SaveSlotButtonState.BASIC);
            }
            foreach (var button in m_emptySlotButtons)
            {
                button.SetActive(_state == SaveSlotButtonState.EMPTY);
            }
            foreach (var button in m_confirmButtons)
            {
                button.SetActive(_state == SaveSlotButtonState.CONFIRMATION);
            }
        }
        m_currentState = _state;
    }
    private void AlterVisuals()
    {
        GameManager.m_saveSlotInUse = m_slotNumber;
        GameManager.LoadSaveInfoFromFile();
        m_validSave = GameManager.m_saveInfo.m_validSave;

        if (m_validSave)
        {
            m_currencyText.transform.parent.gameObject.SetActive(true);
            m_currencyText.text = PlayerPrefs.GetInt($"Player Balance {m_slotNumber}").ToString();

            SetButtonStates(SaveSlotButtonState.BASIC);
        }
        else
        {
            m_titleText.text = "Empty Slot";
            m_currencyText.transform.parent.gameObject.SetActive(false);
            //m_slotBackground.sprite = m_emptySlotSprite;

            SetButtonStates(SaveSlotButtonState.EMPTY);
        }
        SetButtonStates(SaveSlotButtonState.NONE);
    }
    public void SetConfirmState() { SetButtonStates(SaveSlotButtonState.CONFIRMATION); }
    public void SetBasicState() { SetButtonStates(SaveSlotButtonState.BASIC); }
    public void SetEmptyState() { SetButtonStates(SaveSlotButtonState.EMPTY); }
    public void TryLoad()
    {
        GameManager.m_saveSlotInUse = m_slotNumber;
        if (!Directory.Exists(Application.persistentDataPath + $"/saveSlot{ GameManager.m_saveSlotInUse}/"))
            Directory.CreateDirectory(Application.persistentDataPath + $"/saveSlot{ GameManager.m_saveSlotInUse}/");

        if (m_validSave)
            LoadSave();
        else
            OverwriteSave();
    }
    public void DeleteSave(bool _alterVisuals = false)
    {
        GameManager.m_saveSlotInUse = m_slotNumber;

        GameManager.ResetPlayerInfo();
        if (GameManager.RetrieveValidSaveState())
            GameManager.ClearPlayerInfoFromFile();
        if (GameManager.m_saveInfo.m_validSave)
            GameManager.ClearSaveInfoFromFile();
        SkillTreeReader.instance.EmptyAllTrees();

        PlayerPrefs.SetInt("SwampLevel", 0);
        PlayerPrefs.SetInt("CastleLevel", 0);
        PlayerPrefs.SetInt("MagmaLevel", 0);

        var files = Directory.GetFiles(Application.persistentDataPath + $"/saveSlot{GameManager.m_saveSlotInUse}/");
        for (int i = 0; i < files.Length; i++)
        {
            File.Delete(files[i]);
        }
        Directory.Delete(Application.persistentDataPath + $"/saveSlot{GameManager.m_saveSlotInUse}/");

        PlayerPrefs.SetInt($"Player Balance {m_slotNumber}", 0);

        if (_alterVisuals)
        {
            AlterVisuals();
        }
    }
    public void LoadSave()
    {
        GameManager.m_saveSlotInUse = m_slotNumber;
        GameManager.LoadPlayerInfoFromFile();
        GameManager.LoadSaveInfoFromFile();

        GameManager.currentLevel = PlayerPrefs.GetFloat("Level", 0f);
        EndScreenMenu.Restart();
        LevelManager.Instance.LoadNewLevel("HubWorld");
    }
    public void OverwriteSave()
    {
        GameManager.m_saveSlotInUse = m_slotNumber;

        DeleteSave();

        EndScreenMenu.Restart();
        LevelManager.Instance.LoadNewLevel("Tutorial");
    }
}
