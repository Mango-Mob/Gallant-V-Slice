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

        private bool m_hasGivenReward = false;

        private UI_Image m_keyboardInput;
        private UI_Image m_gamepadInput;
        private GameObject m_player;
        private bool m_showUI { 
            get {
                if (m_player != null)
                    return Vector3.Distance(transform.position, m_player.transform.position) <= m_myBrain.m_idealDistance;
                
                return false; 
            }  
        }

        protected override void Awake()
        {
            base.Awake();
            m_player = GameManager.Instance.m_player;
            m_keyboardInput = m_myBrain.m_ui.GetElement<UI_Image>("Keyboard");
            m_gamepadInput = m_myBrain.m_ui.GetElement<UI_Image>("Keyboard");
        }

        protected override void Update()
        {
            m_keyboardInput.gameObject.SetActive(m_showUI && !InputManager.Instance.isInGamepadMode);
            m_gamepadInput.gameObject.SetActive(m_showUI && InputManager.Instance.isInGamepadMode);

            if(m_myBrain.enabled && Vector3.Distance(transform.position, m_player.transform.position) > m_disableDistance)
            {
                m_myBrain.enabled = false;
            }
            else if (!m_myBrain.enabled && Vector3.Distance(transform.position, m_player.transform.position) < m_disableDistance)
            {
                m_myBrain.enabled = true;
            }
        }

        public void Interact()
        {
            DialogManager.Instance.LoadDialog(m_dialog);
            if (!m_hasGivenReward)
            {
                DialogManager.Instance.m_interact = new UnityEngine.Events.UnityEvent();
                DialogManager.Instance.m_interact.AddListener(Reward);
            }
            GetComponentInChildren<Interactable>().m_isReady = false;
            DialogManager.Instance.Show();
        }

        private void Reward()
        {
            RewardManager.Instance.Show(Mathf.FloorToInt(GameManager.currentLevel));
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
