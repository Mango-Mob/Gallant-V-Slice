using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.IO;

public class SaveSlot : MonoBehaviour
{
    [Range(1, 4)] [SerializeField] private int m_slotNumber = 1;
    [SerializeField] private TextMeshProUGUI m_currencyText;
    [SerializeField] private Image m_slotBackground;

    private bool m_validSave = false;

    // Start is called before the first frame update
    void Start()
    {
        GameManager.m_saveSlotInUse = m_slotNumber;
        GameManager.LoadSaveInfoFromFile();
        m_validSave = GameManager.m_saveInfo.m_validSave;

        if (m_validSave)
            m_currencyText.text = PlayerPrefs.GetInt($"Player Balance {m_slotNumber}").ToString();
        else
            m_currencyText.enabled = false;
    }

    public void TryLoad()
    {
        if (!Directory.Exists(Application.persistentDataPath + $"/saveSlot{ GameManager.m_saveSlotInUse}/"))
            Directory.CreateDirectory(Application.persistentDataPath + $"/saveSlot{ GameManager.m_saveSlotInUse}/");


        if (m_validSave)
            LoadSave();
        else
            OverwriteSave();
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

        GameManager.ResetPlayerInfo();
        if (GameManager.RetrieveValidSaveState())
            GameManager.ClearPlayerInfoFromFile();
        if (GameManager.m_saveInfo.m_validSave)
            GameManager.ClearSaveInfoFromFile();
        SkillTreeReader.instance.EmptyAllTrees();

        PlayerPrefs.SetInt("SwampLevel", 0);
        PlayerPrefs.SetInt("CastleLevel", 0);
        PlayerPrefs.SetInt("MagmaLevel", 0);
        EndScreenMenu.Restart();
        LevelManager.Instance.LoadNewLevel("Tutorial");
    }
}
