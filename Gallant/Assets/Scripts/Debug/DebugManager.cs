using ActorSystem.AI;
using ActorSystem.AI.Components;
//using SOHNE.Accessibility.Colorblindness;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

namespace Exceed.Debug
{
    public class DebugManager : SingletonPersistent<DebugManager>
    {
        public LayerMask m_raycastLayers;

        public static bool showRoomLocations = false;
        public bool m_isMinimised = true;

        [SerializeField] private GameObject m_contentParent;
        [SerializeField] private Image m_backgroundImage;

        [SerializeField] private GameObject[] m_panelArray;
        [SerializeField] private Button[] m_buttonArray;

        [Header("Actor Content")]
        [SerializeField] private Text m_ActorCountTxt;
        [SerializeField] private Button m_killAllBtn;
        [SerializeField] private Text m_selelectedName;
        [SerializeField] private Button[] m_toggleSelectedButtons;
        [SerializeField] private Button m_killOneBtn;
        [SerializeField] private Debug_ActorDetail m_detailActorView;
        
        [SerializeField] private GameObject m_detailRoomView;
        [SerializeField] private Image m_toggleButtonImage;
        [SerializeField] private GameObject m_actorFieldPrefab;
        [SerializeField] private GameObject m_actorFieldParent;
        private List<Debug_ActorField> m_actorFieldList;

        //Room View
        [SerializeField] private Text m_roomWaves;
        [SerializeField] private Text m_roomCost;

        [Header("Scene Content")]
        [SerializeField] private Dropdown m_sceneList;
        [SerializeField] private Dropdown m_colorBlindList;
        [SerializeField] private Button[] m_toggleCamButtons;
        [SerializeField] private SimpleCameraController m_freeCamera;
        [SerializeField] private Slider m_timeSlider;
        [SerializeField] private Toggle m_timeCheck;
        [SerializeField] private Toggle m_HudCheck;

        [Header("Other Content")]
        [SerializeField] private Toggle m_showRooms;

        private Player_Controller m_player;
        private Camera m_mainCamera;

        private GameObject m_selected;
        private void Awake()
        {
            m_player = FindObjectOfType<Player_Controller>();
            m_actorFieldList = new List<Debug_ActorField>();
        }

        // Start is called before the first frame update
        void Start()
        {
            m_showRooms.isOn = showRoomLocations;
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
            m_selelectedName.text = (m_selected != null) ? $"\"{m_selected.name}\"" : "null";

            m_toggleSelectedButtons[0].interactable = false;
            m_toggleSelectedButtons[1].interactable = false;
            m_killOneBtn.interactable = false;
            OnLevelLoad();
        }

        private void OnLevelLoad()
        {
            m_freeCamera.gameObject.SetActive(false);
            m_toggleCamButtons[0].interactable = (m_player != null);
            m_toggleCamButtons[1].interactable = false;

            if(m_actorFieldList.Count > 0)
            {
                foreach (var item in m_actorFieldList)
                {
                    Destroy(item.gameObject);
                }
                m_actorFieldList.Clear();
            }

            foreach (var item in ActorManager.Instance.m_reserved.Keys)
            {
                var field = Instantiate(m_actorFieldPrefab, m_actorFieldParent.transform).GetComponent<Debug_ActorField>();
                field.m_actorName = item;
                field.gameObject.SetActive(true);
                m_actorFieldList.Add(field);
            }

            m_mainCamera = (m_player != null) ? m_player.GetComponentInChildren<Camera>() : Camera.main;
            m_timeSlider.value = 1.0f;
            Time.timeScale = 1.0f;
            m_freeCamera.SetTarget(m_mainCamera.transform);
        }

        private void OnLevelWasLoaded(int level)
        {
            m_player = FindObjectOfType<Player_Controller>();
            OnLevelLoad();
        }

        // Update is called once per frame
        void Update()
        {
            UpdateRoomDetails();
            GetComponent<Animator>().SetBool("IsMinimised", m_isMinimised);
            if (InputManager.Instance.IsKeyDown(KeyType.TILDE))
            {
                GetComponent<Animator>().SetTrigger("StateUpdate");
            }
            if (m_contentParent.activeInHierarchy)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
                Time.timeScale = m_timeSlider.value;
                m_freeCamera.enabled = false;

                if (InputManager.Instance.IsMouseButtonDown(MouseButton.LEFT))
                {
                    SelectedUpdate();
                }
            }
            else if (m_freeCamera.gameObject.activeInHierarchy)
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
                m_freeCamera.enabled = true;
            }

            if (!m_timeCheck.isOn)
            {
                m_timeSlider.value = 1.0f;
            }
            m_timeSlider.interactable = m_timeCheck.isOn;

            int count = ActorManager.Instance.m_subscribed.Count;
            m_ActorCountTxt.text = count.ToString();
            m_killAllBtn.interactable = count > 0;

            if (HUDManager.Instance != null)
                HUDManager.Instance.gameObject.SetActive(!m_HudCheck.isOn);

            DebugManager.showRoomLocations = m_showRooms.isOn;
        }

        private void SelectedUpdate()
        {
            if (!IsMouseOnWindow())
            {
                Ray ray;

                if (m_freeCamera.gameObject.activeInHierarchy)
                    ray = m_freeCamera.GetComponent<Camera>().ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());
                else
                    ray = m_mainCamera.ScreenPointToRay(InputManager.Instance.GetMousePositionInScreen());

                RaycastHit hit;
                if (Physics.Raycast(ray, out hit, m_raycastLayers))
                {
                    if (m_selected != hit.collider.gameObject)
                    {
                        m_selected = hit.collider.gameObject;
                        EvaluateSelected();
                    }
                }

                m_selelectedName.text = (m_selected != null) ? $"\"{m_selected.name}\"" : "null";

                m_toggleSelectedButtons[0].interactable = !m_selected.activeInHierarchy;
                m_toggleSelectedButtons[1].interactable = m_selected.activeInHierarchy;
                m_killOneBtn.interactable = m_selected.GetComponentInParent<Actor>() != null;
            }
        }

        public void EvaluateSelected()
        {
            m_detailActorView.gameObject.SetActive(false);
            m_detailRoomView.SetActive(false);

            if (m_selected.GetComponent<Actor>() != null)
            {
                m_detailActorView.gameObject.SetActive(true);
                m_detailActorView.SetReference(m_selected.GetComponent<Actor>());
            }
            else if (m_selected.GetComponent<Room>() != null)
            {
                m_detailRoomView.SetActive(true);
                return;
            }
            else
            {
                return;
            }
        }

        private bool IsMouseOnWindow()
        {
            Vector2 localPoint = m_backgroundImage.rectTransform.InverseTransformPoint(InputManager.Instance.GetMousePositionInScreen());
            if (m_backgroundImage.rectTransform.rect.Contains(localPoint))
            {
                return true;
            }
            return false;
        }

        public void LoadScene()
        {
            SceneManager.LoadScene(m_sceneList.value);
        }

        public void ShowPanel(int index)
        {
            m_isMinimised = false;

            if (m_panelArray.Length > 0 && m_buttonArray.Length > 0)
            {
                for (int i = 0; i < m_panelArray.Length; i++)
                {
                    if (i == index)
                    {
                        m_panelArray[i].SetActive(true);
                        m_buttonArray[i].GetComponentInChildren<Text>().color = new Color(250, 244, 0);
                        continue;
                    }
                    else
                    {
                        m_panelArray[i].SetActive(false);
                        m_buttonArray[i].GetComponentInChildren<Text>().color = new Color(255, 255, 255);
                    }
                }
            }
        }

        public void ToggleFreeCamera(bool status)
        {
            GameManager.Instance.m_activeCamera = (status) ? m_freeCamera.GetComponent<Camera>() : m_mainCamera;
            m_freeCamera.gameObject.SetActive(status);
            m_player.m_isDisabledInput = status;

            m_toggleCamButtons[0].interactable = !status;
            m_toggleCamButtons[1].interactable = status;
        }

        public void ColorBlindChange()
        {
            //GetComponent<Colorblindness>().Change(m_colorBlindList.value);
        }

        public void KillAllEnemies()
        {
            ActorManager.Instance.KillAll();
        }
        public void KillSelected()
        {
            ActorManager.Instance.Kill(m_selected.GetComponentInParent<ActorSystem.AI.Actor>());
            m_selected = null;
            m_toggleSelectedButtons[0].interactable = false;
            m_toggleSelectedButtons[1].interactable = false;
            m_killOneBtn.interactable = false;
        }

        public void ToggleSelected(bool status)
        {
            m_selected.SetActive(status);
            m_toggleSelectedButtons[0].interactable = !status;
            m_toggleSelectedButtons[1].interactable = status;
        }

        public void Minimise()
        {
            m_isMinimised = true;
        }

        #region DisplayUpdate

        private void UpdateRoomDetails()
        {
            if (m_detailRoomView.activeInHierarchy)
            {
                var room = m_selected.GetComponent<Room>();

                if (room.m_mySpawnner.m_waves != null)
                    m_roomWaves.text = room.m_mySpawnner.m_waves.Count.ToString();
                m_roomCost.text = room.m_mySpawnner.m_value.ToString();

                m_toggleButtonImage.color = (room.m_mySpawnner.enabled) ? new Color(0, 176, 0) : new Color(197, 0, 0);
            }
        }

        #endregion

        #region ButtonFunctions
        public void ResetRoom()
        {
            var room = m_selected.GetComponent<Room>();
            room.m_mySpawnner.Restart();
        }

        public void ToggleRoom()
        {
            var room = m_selected.GetComponent<Room>();
            room.m_mySpawnner.enabled = !room.m_mySpawnner.enabled;
        }

        public void ForceRoom()
        {
            var room = m_selected.GetComponent<Room>();
            room.m_mySpawnner.ForceWave();
        }
        #endregion
    }
}
