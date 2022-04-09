using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkillTreeObject : MonoBehaviour
{
    private Player_Controller playerController;

    private Interactable m_interactable;
    [SerializeField] private UI_Text m_keyboardInput;
    [SerializeField] private UI_Image m_gamepadInput;
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
        m_keyboardInput.transform.parent.gameObject.SetActive(m_interactable.m_isReady && !InputManager.Instance.isInGamepadMode);
        m_gamepadInput.gameObject.SetActive(m_interactable.m_isReady && InputManager.Instance.isInGamepadMode);
        m_useButton.SetActive(m_interactable.m_isReady && !m_skillTreeDisplayControl.activeSelf);
        if (m_skillTreeDisplayControl.activeSelf)
        {
            if (InputManager.Instance.IsBindDown("Skill_Back"))
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

        if (HUDManager.Instance != null)
            HUDManager.Instance.gameObject.SetActive(false);
    }

    public void CloseSkillTree()
    {
        m_skillTreeDisplayControl.SetActive(false);
        StartCoroutine(DelayControl());

        playerController.playerCamera.enabled = true;
        m_skillTreeCamera.enabled = false;

        playerController.playerSkills.EvaluateSkills();

        if (HUDManager.Instance != null)
            HUDManager.Instance.gameObject.SetActive(true);
    }

    IEnumerator DelayControl()
    {
        yield return new WaitForEndOfFrame();
        playerController.m_isDisabledInput = false;
    }
}
