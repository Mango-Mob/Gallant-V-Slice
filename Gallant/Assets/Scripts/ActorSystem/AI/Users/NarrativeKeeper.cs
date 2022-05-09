﻿using System;
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

        public Interactable m_interactDisplay;
        public bool isWaiting = false;
        public float interactRange = 1.5f;

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

        protected override void Awake()
        {
            base.Awake();
            if(NarrativeManager.Instance.m_deadNPCs[m_myData.ActorName])
            {
                gameObject.SetActive(false);
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

        protected override void Start()
        {
            base.Start();
            m_player = GameManager.Instance.m_player;
            visits = NarrativeManager.Instance.m_visitNPC[m_myData.ActorName];
            EvaluateDialogOptions(visits);
        }

        protected override void Update()
        {
            m_interactDisplay.m_isReady = m_showUI && !isWaiting && m_potentialDialogs.Count > 0;
            m_myBrain.SetEnabled(!isWaiting);
            base.Update();
        }

        public void TalkTo()
        {
            this.SetTargetOrientaion(m_player.transform.position);
            isWaiting = true;

            if(!hasInteractedWith)
            {
                hasInteractedWith = true;
                NarrativeManager.Instance.m_visitNPC[m_myData.ActorName] = visits + 1;
                PlayerPrefs.SetInt($"{m_myData.ActorName}Visits", visits + 1);
            }

            int select = UnityEngine.Random.Range(0, m_potentialDialogs.Count);

            DialogManager.Instance.LoadDialog(m_potentialDialogs[select].dialog);
            DialogManager.Instance.m_onDialogFinish = new UnityEvent();
            DialogManager.Instance.m_onDialogFinish.AddListener(EndTalk);
            DialogManager.Instance.Show();

            NarrativeManager.Instance.AddSeenDialog(m_potentialDialogs[select].dialog);

            this.SetTargetOrientaion(GameManager.Instance.m_player.transform.position);
            GetComponentInChildren<Interactable>().m_isReady = false;

            m_potentialDialogs.RemoveAt(select);
        }

        public void EndTalk()
        {
            isWaiting = false;
        }

        public override bool DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal = 0, Vector3? _damageLoc = null)
        {
            if (base.DealDamage(_damage, _type, piercingVal, _damageLoc))
            {
                NarrativeManager.Instance.m_deadNPCs[m_myData.ActorName] = true;
                return true;
            }
            return false;
        }
        public override bool DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            if(base.DealDamageSilent(_damage, _type))
            {
                NarrativeManager.Instance.m_deadNPCs[m_myData.ActorName] = true;
                return true;
            }
            return false;
        }
    }
}