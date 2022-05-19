using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ActorSystem.AI.Users
{
    public class LoreKeeper : Actor
    {
        [Header("~ LoreKeeper ~")]
        public TextAsset[] m_dialog;
        public int myTutorialPosition = 0;
        public float m_idealDistance = 1.5f;
        
        private Interactable m_myInteractLogic;

        private UI_Text m_keyboardInput;
        private UI_Image m_gamepadInput;
        private GameObject m_player;
        private bool isVisible = false;
        private bool isDone = false;
        private bool m_showUI { 
            get {
                if (m_player != null)
                    return Vector3.Distance(transform.position, m_player.transform.position) <= m_idealDistance && isVisible;
                
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

            m_myBrain.m_ui.GetElement<UI_Bar>("Health").gameObject.SetActive(false);
            
        }

        protected override void Update()
        {
            isVisible = TutorialManager.Instance.tutorialPosition == myTutorialPosition && !isDone && !GameManager.Instance.IsInCombat;
            m_myBrain.m_animator.SetBool("IsVisible", isVisible);
            m_myBrain?.m_myOutline.SetEnabled(isVisible);

            GetComponent<Collider>().enabled = isVisible;
            m_keyboardInput.transform.parent.gameObject.SetActive(m_showUI && !InputManager.Instance.isInGamepadMode);
            m_gamepadInput.gameObject.SetActive(m_showUI && InputManager.Instance.isInGamepadMode);
            if (m_showUI)
            {
                UpdateDisplay();
            }

            m_myInteractLogic.m_isReady = m_showUI && Vector3.Distance(transform.position, m_player.transform.position) <= 1.5f;

            if(isDone && m_myBrain.m_animator.IsCurrentStatePlaying(0, "Hidden"))
            {
                Destroy(gameObject);
            }
        }

        public void Interact()
        {
            DialogManager.Instance.LoadDialog(m_dialog[TutorialManager.Instance.targetDialog]);

            DialogManager.Instance.m_interact[0] = new UnityEvent();
            DialogManager.Instance.m_interact[0].AddListener(InteractFunc);
            DialogManager.Instance.m_onDialogFinish = new UnityEvent();
            DialogManager.Instance.m_onDialogFinish.AddListener(CompleteDialog);
            GetComponentInChildren<Interactable>().m_isReady = false;
            
            DialogManager.Instance.Show();
        }

        public void InteractFunc()
        {
            TutorialManager.Instance.InteractFunction();
        }

        public void CompleteDialog()
        {
            isDone = TutorialManager.Instance.AdvanceTutorial();
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
