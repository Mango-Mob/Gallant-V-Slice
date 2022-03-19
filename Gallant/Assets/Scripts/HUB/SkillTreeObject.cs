using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeObject : MonoBehaviour
{
    private Player_Controller playerController;

    private Interactable m_interactable;
    [SerializeField] private GameObject m_skillTreeDisplayControl;
    [SerializeField] private Camera m_skillTreeCamera;
    [SerializeField] private GameObject m_useButton;

    // Start is called before the first frame update
    void Start()
    {
        playerController = FindObjectOfType<Player_Controller>();
        m_skillTreeCamera.enabled = false;
        m_skillTreeDisplayControl.SetActive(false);
        m_interactable = GetComponentInChildren<Interactable>();
    }

    // Update is called once per frame
    void Update()
    {
        m_useButton.SetActive(m_interactable.m_isReady);
        if (m_skillTreeDisplayControl.activeSelf)
        {
            if (InputManager.Instance.IsBindDown("Roll"))
            {
                CloseSkillTree();
            }
        }
    }

    public void OpenSkillTree()
    {
        m_skillTreeDisplayControl.SetActive(true);
        playerController.m_isDisabledInput = true;

        playerController.playerCamera.enabled = false;
        m_skillTreeCamera.enabled = true;
    }

    public void CloseSkillTree()
    {
        m_skillTreeDisplayControl.SetActive(false);
        playerController.m_isDisabledInput = false;

        playerController.playerCamera.enabled = true;
        m_skillTreeCamera.enabled = false;
    }
}
