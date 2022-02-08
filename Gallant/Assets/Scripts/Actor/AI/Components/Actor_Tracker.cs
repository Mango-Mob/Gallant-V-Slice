using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Actor.AI.Components
{
    /****************
     * Actor_Tracker : An actor component used to track and display the damage dealt to this actor.
     * @author : Michael Jordan
     * @file : Actor_Tracker.cs
     * @year : 2021
     */
    public class Actor_Tracker : MonoBehaviour
{
    [Header("Settings")]
    public bool m_enableAutoHealing = false;
    public float m_peakRefreshTime = 5f;

    //Where to display the text;
    private Text m_display;

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

    //Stats to record:
    private float m_resistanceStat = 0;
    private float m_damageStat = 0;
    private DamageRecord m_peakDPS;
    private DamageRecord m_peakHit;
    private float m_lastHitStat = 0;
    public Queue<DamageRecord> m_damageRecords = new Queue<DamageRecord>();

    //Called upon creation of this class
    private void Awake()
    {
        m_display = GetComponent<Text>();
        m_peakDPS = new DamageRecord(0);
    }

    // Update is called once per frame
    void Update()
    {
        while(m_damageRecords.Count > 0 && m_damageRecords.Peek().HasElapsed(1.0f))
        {
            DamageRecord expired = m_damageRecords.Dequeue();
            m_damageStat -= expired.m_amount;
        }

        if (m_peakDPS.m_amount < m_damageStat)
        {
            m_peakDPS.m_amount = m_damageStat;
            m_peakDPS.Refresh();
        }

        if(m_peakDPS.HasElapsed(m_peakRefreshTime) && m_peakDPS.m_amount != 0)
        {
            m_peakDPS.m_amount = 0;
            m_peakDPS.Refresh();
        }

        if (m_peakHit.HasElapsed(m_peakRefreshTime) && m_peakHit.m_amount != 0)
        {
            m_peakHit.m_amount = 0;
            m_peakHit.Refresh();
        }

        string display = "";
        display += $"Resist: {m_resistanceStat} \n";
        display += $"Last Hit: {m_lastHitStat} ({m_peakHit.m_amount})\n";
        display += $"DPS: {m_damageStat}/s ({m_peakDPS.m_amount}/s) \n";

        if(m_display != null)
            m_display.text = display;
    }

    /*******************
     * RecordDamage : Records a damage value onto this tracker.
     * @author : Michael Jordan
     * @param : (float) damage to record.
     */
    public void RecordDamage(float damage)
    {
        m_lastHitStat = damage;
        if(m_peakHit.m_amount < damage)
        {
            m_peakHit.m_amount = damage;
            m_peakHit.Refresh();
        }
        m_damageStat += damage;
        m_damageRecords.Enqueue(new DamageRecord(damage));
    }

    /*******************
     * RecordResistance : Records the resistance value to display onto this tracker.
     * @author : Michael Jordan
     * @param : (float) resistance to record.
     */
    public void RecordResistance(float resistance)
    {
        m_resistanceStat = resistance;
    }
}
}
