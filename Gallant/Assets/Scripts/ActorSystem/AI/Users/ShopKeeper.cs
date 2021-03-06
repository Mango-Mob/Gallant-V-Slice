using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace ActorSystem.AI.Users
{
    public class ShopKeeper : Actor
    {
        [Header("~ ShopKeeper ~")]
        public TextAsset m_dialog;
        public float m_disableDistance;
        public RewardManager.RewardType m_myShopType;
        public float m_idealDistance = 1.5f;
        private bool m_hasGivenReward = false;

        private Interactable m_myInteractLogic;

        private GameObject m_interactDisplay;
        private UI_Text m_keyboardInput;
        private UI_Image m_gamepadInput;
        private GameObject m_player;
        private bool m_showUI { 
            get {
                if (m_player != null)
                    return Vector3.Distance(transform.position, m_player.transform.position) <= m_idealDistance;
                
                return false; 
            }  
        }

        protected override void Awake()
        {
            base.Awake();
            m_myInteractLogic = GetComponentInChildren<Interactable>();
        }

        protected override void Start()
        {
            base.Start();
            m_player = GameManager.Instance.m_player;
            m_keyboardInput = m_myBrain.m_ui.GetElement<UI_Text>("Keyboard");
            m_gamepadInput = m_myBrain.m_ui.GetElement<UI_Image>("Gamepad");
            m_interactDisplay = m_keyboardInput.transform.parent.gameObject;
            m_myBrain.m_ui.GetElement<UI_Bar>("Health").gameObject.SetActive(false);
        }

        protected override void Update()
        {
            m_interactDisplay.SetActive(m_showUI);
            m_keyboardInput.gameObject.SetActive(m_showUI && !InputManager.Instance.isInGamepadMode);
            m_gamepadInput.gameObject.SetActive(m_showUI && InputManager.Instance.isInGamepadMode);

            //m_myBrain.m_myOutline.SetEnabled(!m_hasGivenReward);
            if (m_myBrain.enabled && Vector3.Distance(transform.position, m_player.transform.position) > m_disableDistance)
            {
                m_myBrain.SetEnabled(false);
            }
            else if (!m_myBrain.enabled && Vector3.Distance(transform.position, m_player.transform.position) < m_disableDistance)
            {
                m_myBrain.SetEnabled(true);
            }

            if (m_showUI)
            {
                UpdateDisplay();
            }
            m_myInteractLogic.m_isReady = m_showUI;
            m_myBrain.m_myOutline.enabled = !m_hasGivenReward;
        }

        public void Interact()
        {
            //DialogManager.Instance.LoadDialog(m_dialog);
            if (!m_hasGivenReward)
            {
                //DialogManager.Instance.m_interact.Add(new UnityEngine.Events.UnityEvent());
                //DialogManager.Instance.m_interact[0].AddListener(Reward);

                RewardManager.Instance.Show(Mathf.FloorToInt(GameManager.currentLevel), m_myShopType);
                m_hasGivenReward = true;
            }

            this.SetTargetOrientaion(GameManager.Instance.m_player.transform.position);
            //GetComponentInChildren<Interactable>().m_isReady = false;
            //DialogManager.Instance.Show();
        }

        private void Reward()
        {
            DialogManager.Instance.Hide();
            RewardManager.Instance.Show(Mathf.FloorToInt(GameManager.currentLevel), m_myShopType);
            DialogManager.Instance.m_interact = null;
            m_hasGivenReward = true;
        }

        private void UpdateDisplay()
        {
            InputManager.Bind[] binds = InputManager.Instance.GetBinds("Interact");
            bool foundKey = false;
            bool foundButton = false;
            for (int i = 0; i < binds.Length; i++)
            {
                switch (InputManager.Bind.GetTypeID(binds[i].enumType))
                {
                    case 0:
                        {
                            if (!foundKey)
                            {
                                foundKey = true;
                                m_keyboardInput.GetComponentInChildren<Text>().text = InputManager.Instance.GetKeyString((KeyType)binds[i].value);
                            }
                            break;
                        }
                    case 1:
                        {
                            if (!foundKey)
                            {
                                foundKey = true;
                                m_keyboardInput.GetComponentInChildren<Text>().text = InputManager.Instance.GetMouseButtonString((MouseButton)binds[i].value);
                            }
                            break;
                        }
                    case 2:
                        {
                            if (!foundButton)
                            {
                                foundButton = true;
                                m_gamepadInput.m_mySprite = InputManager.Instance.GetGamepadSprite((ButtonType)binds[i].value);
                            }
                            break;
                        }
                    default:
                    case 3:
                        break;
                }
            }
        }
    }
}
