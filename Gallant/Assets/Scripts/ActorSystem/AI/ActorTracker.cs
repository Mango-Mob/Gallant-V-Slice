using ActorSystem.AI.Components;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace ActorSystem.AI
{
    /****************
     * Actor_Tracker : An actor component used to track and display the damage dealt to this actor.
     * @author : Michael Jordan
     * @file : Actor_Tracker.cs
     * @year : 2021
     */
    public class ActorTracker : UI_Panel
    {
        [Header("Settings")]
        public float m_peakRefreshTime = 5f;

        /****************
         * DamageRecord : A struct which records the system time when a damage was received.
         * @author : Michael Jordan
         */
        public struct DamageRecord
        {
            public DamageRecord(float dmg) { m_amount = dmg; m_whenHit = DateTime.Now; }

            public float m_amount;
            private DateTime m_whenHit;

            /*******************
             * HasElapsed : Checks the total seconds elapsed since the creation of this DamageRecord.
             * @author : Michael Jordan
             * @param : (float) Seconds to compare with.
             * @return : (bool) true if the total seconds has elapsed the provided value.
             */
            public bool HasElapsed(float seconds) { return (DateTime.Now - m_whenHit).TotalSeconds >= seconds; }

            /*********************
             * Refresh : Resets time when this DamageRecord was created.
             * @author: Michael Jordan
             */
            public void Refresh() { m_whenHit = DateTime.Now; }
        }

        public Actor_Brain m_brainToTrack;

        private float m_lastFrameHealth;
        private UI_Text m_healthDisplay;
        private UI_Text m_phyDisplay;
        private UI_Text m_abilDisplay;
        private UI_Text m_damageDisplay;

        protected float m_health { get { return m_brainToTrack.m_currHealth; } }
        protected float m_phyResist { get { return m_brainToTrack.m_currPhyResist; } }
        protected float m_abiResist { get { return m_brainToTrack.m_currAbilResist; } }

        //Stats to record:
        private float m_damageStat = 0;
        private DamageRecord m_peakDPS;
        private DamageRecord m_peakHit;
        private float m_lastHitStat = 0;
        public Queue<DamageRecord> m_damageRecords = new Queue<DamageRecord>();

        private void Awake()
        {
            m_healthDisplay = GetElement<UI_Text>("HealthText");
            m_phyDisplay = GetElement<UI_Text>("PhysicalText");
            m_abilDisplay = GetElement<UI_Text>("AbilityText");
            m_damageDisplay = GetElement<UI_Text>("DamageText");

            m_peakDPS = new DamageRecord(0);
        }
        //Called upon creation of this class
        private void Start()
        {
            m_lastFrameHealth = m_health;
        }

        // Update is called once per frame
        void Update()
        {
            while (m_damageRecords.Count > 0 && m_damageRecords.Peek().HasElapsed(1.0f))
            {
                DamageRecord expired = m_damageRecords.Dequeue();
                m_damageStat -= expired.m_amount;
            }

            if (m_peakDPS.m_amount < m_damageStat)
            {
                m_peakDPS.m_amount = m_damageStat;
                m_peakDPS.Refresh();
            }

            if (m_peakDPS.HasElapsed(m_peakRefreshTime) && m_peakDPS.m_amount != 0)
            {
                m_peakDPS.m_amount = 0;
                m_peakDPS.Refresh();
            }

            if (m_peakHit.HasElapsed(m_peakRefreshTime) && m_peakHit.m_amount != 0)
            {
                m_peakHit.m_amount = 0;
                m_peakHit.Refresh();
            }

            m_healthDisplay.m_myText = m_health.ToString();
            m_phyDisplay.m_myText = m_phyResist.ToString();
            m_abilDisplay.m_myText = m_abiResist.ToString();

            m_damageDisplay.m_myText = $"{m_lastHitStat} ({m_damageStat.ToString("0.0")}/s)";
        }

        private void LateUpdate()
        {
            if (m_lastFrameHealth != m_health)
            {
                if(m_lastFrameHealth > m_health)
                {
                    RecordDamage(m_lastFrameHealth - m_health);
                }
                m_lastFrameHealth = m_health;
            }
        }
        /*******************
         * RecordDamage : Records a damage value onto this tracker.
         * @author : Michael Jordan
         * @param : (float) damage to record.
         */
        public void RecordDamage(float damage)
        {
            m_lastHitStat = damage;
            if (m_peakHit.m_amount < damage)
            {
                m_peakHit.m_amount = damage;
                m_peakHit.Refresh();
            }
            m_damageStat += damage;
            m_damageRecords.Enqueue(new DamageRecord(damage));
        }
    }
}
