using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Actor_Tracker : MonoBehaviour
{
    public bool m_enableAutoHealing = false;
    public float m_peakRefreshTime = 5f;

    private TextMeshPro m_display;
    private float m_resistanceStat = 0;
    private float m_damageStat = 0;
    private DamageRecord m_peakDPS;
    private DamageRecord m_peakHit;
    private float m_lastHitStat = 0;

    public struct DamageRecord
    {
        public DamageRecord(float dmg) { m_amount = dmg; m_whenHit = DateTime.Now; }
        
        public float m_amount;
        private DateTime m_whenHit;
        public bool HasElapsed(float seconds) { return (DateTime.Now - m_whenHit).TotalSeconds > seconds; }
        public void Refresh() { m_whenHit = DateTime.Now; }
    }

    public Queue<DamageRecord> m_damageRecords = new Queue<DamageRecord>();

    private void Awake()
    {
        m_display = GetComponent<TextMeshPro>();
        m_peakDPS = new DamageRecord(0);
    }

    // Start is called before the first frame update
    void Start()
    {
        
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

        m_display.text = display;
    }

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

    public void RecordResistance(float resistance)
    {
        m_resistanceStat = resistance;
    }
}
