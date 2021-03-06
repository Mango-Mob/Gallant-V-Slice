using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace ActorSystem.AI.Users
{
    public class NarrativeKeeper : Actor
    {
        [Serializable]
        public struct DialogNarrative
        {
            public int minVisits;
            public int maxVisits;
            public TextAsset dialog;
        }

        public List<DialogNarrative> m_potentialDialogs;
        public ScriptableObject m_reward;
        
        public Interactable m_interactDisplay;
        public bool isWaiting = false;
        public float interactRange = 1.5f;
        public bool isTestMode = true;

        private bool rewardGiven = false;
        private bool hasInteractedWith = false;
        private int visits = 0;
        private GameObject m_player;
        private bool m_showUI
        {
            get
            {
                if (m_player != null)
                    return Vector3.Distance(transform.position, m_player.transform.position) <= interactRange;

                return false;
            }
        }

        private void EvaluateDialogOptions(int visits)
        {
            for (int i = m_potentialDialogs.Count - 1; i >= 0; i--)
            {
                if(!(visits >= m_potentialDialogs[i].minVisits && visits <= m_potentialDialogs[i].maxVisits))
                {
                    m_potentialDialogs.RemoveAt(i);
                }
                else
                {
                    if(NarrativeManager.Instance.HasPlayerSeen(m_potentialDialogs[i].dialog))
                    {
                        m_potentialDialogs.RemoveAt(i);
                    }
                }
            }
        }

        protected override void Awake()
        {
            base.Awake();
        }

        protected override void Start()
        {
            base.Start();
            if (NarrativeManager.Instance.m_deadNPCs[m_myData.ActorName])
            {
                gameObject.SetActive(false);
            }
            m_player = GameManager.Instance.m_player;
            visits = NarrativeManager.Instance.m_visitNPC[m_myData.ActorName];

            if(!isTestMode)
                EvaluateDialogOptions(visits);
        }

        protected override void Update()
        {
            if (m_myBrain.IsDead)
            {
                m_myBrain.m_myOutline.enabled = false;
                return;
            }

            if(m_myBrain.m_target != null && m_myBrain.m_legs != null)
                this.SetTargetOrientaion(m_myBrain.m_legs.m_targetPosition);

            m_myBrain.m_myOutline.enabled = m_potentialDialogs.Count > 0;
            m_interactDisplay.m_isReady = m_showUI && !isWaiting && m_potentialDialogs.Count > 0;
            m_myBrain.SetEnabled(!isWaiting);

            if(rewardGiven)
            {
                if(!RewardManager.Instance.IsVisible)
                {
                    rewardGiven = false;
                    isWaiting = false;
                }
            }
            base.Update();
        }

        public void TalkTo()
        {
            if (m_potentialDialogs.Count == 0)
                return;

            this.SetTargetOrientaion(m_player.transform.position);
            isWaiting = true;

            if(!hasInteractedWith && !isTestMode)
            {
                hasInteractedWith = true;
                NarrativeManager.Instance.UpdateVisit(m_myData.ActorName, visits + 1);
            }

            int select = 0; 

            DialogManager.Instance.LoadDialog(m_potentialDialogs[select].dialog);
            DialogManager.Instance.m_onDialogFinish = new UnityEvent();
            DialogManager.Instance.m_onDialogFinish.AddListener(EndTalk);
            DialogManager.Instance.Show();

            DialogManager.Instance.m_interact[0] = new UnityEvent();
            DialogManager.Instance.m_interact[0].AddListener(Reward);
            DialogManager.Instance.m_interact[1] = new UnityEvent();
            DialogManager.Instance.m_interact[1].AddListener(RewardReturn);

            NarrativeManager.Instance.AddSeenDialog(m_potentialDialogs[select].dialog);

            this.SetTargetOrientaion(GameManager.Instance.m_player.transform.position);
            GetComponentInChildren<Interactable>().m_isReady = false;

            m_potentialDialogs.RemoveAt(select);
        }

        private void Reward()
        {
            DialogManager.Instance.Hide();
            if (m_reward == null)
                RewardManager.Instance.Show(Mathf.FloorToInt(GameManager.currentLevel), RewardManager.RewardType.STANDARD, true);
            else
                RewardManager.Instance.ShowSolo(m_reward, null, true);

            DialogManager.Instance.m_interact = null;
            rewardGiven = true;
        }
        private void RewardReturn()
        {
            DialogManager.Instance.Hide();
            if (m_reward == null)
                RewardManager.Instance.Show(Mathf.FloorToInt(GameManager.currentLevel), RewardManager.RewardType.STANDARD, false);
            else
                RewardManager.Instance.ShowSolo(m_reward, null, false);

            DialogManager.Instance.m_interact = null;
            rewardGiven = true;
        }

        public void EndTalk()
        {
            isWaiting = false;
        }

        public override bool DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal = 0, Vector3? _damageLoc = null)
        {
            if (!m_myBrain.IsDead && !m_myData.invincible)
            {
                m_potentialDialogs.Clear();
                if (base.DealDamage(_damage, _type, piercingVal, _damageLoc))
                {
                    NarrativeManager.Instance.m_deadNPCs[m_myData.ActorName] = true;
                    SetState(new State_Dead(this));
                    return true;
                }
                SetState(new State_FleeFromTarget(this));
            }
            
            return false;
        }
        public override bool DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            if(!m_myBrain.IsDead && !m_myData.invincible)
            {
                if (base.DealDamageSilent(_damage, _type))
                {
                    NarrativeManager.Instance.m_deadNPCs[m_myData.ActorName] = true;
                    SetState(new State_Dead(this));
                    return true;
                }
                SetState(new State_FleeFromTarget(this));
            }
            return false;
        }
    }
}
