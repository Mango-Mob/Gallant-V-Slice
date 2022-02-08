using Actor.AI;
using System;
using UnityEngine;

/****************
 * WeakenStatus : A status effect that weakens the victim and slowly restores their resistance back to the initial value. 
 *              Strength = Damage per second
 * @author : Michael Jordan
 * @file : SlowStatus.cs
 * @year : 2021
 */
public class WeakenStatus : StatusEffect
{
    public float m_maxResistance;
    public float m_currResistance;

    public WeakenStatus(float str, float dur) : base(str, dur) { m_displayColor = Color.white; }

    public override bool ReactTo(StatusEffect other)
    {
        if (other.GetType() == typeof(WeakenStatus))
        {
            m_strength = Mathf.Max(m_strength, (other as WeakenStatus).m_strength);
            m_duration = Mathf.Max(m_duration, (other as WeakenStatus).m_duration);
            m_startDuration = Mathf.Max(m_startDuration, m_duration);
            return true;
        }
        return false;
    }

    public override void StartActor(Enemy _actor)
    {
        m_maxResistance = _actor.m_myData.phyResist;
        m_currResistance = m_maxResistance * m_strength;
        //Show vfx
    }

    public override void StartPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    public override void UpdateOnActor(Enemy _actor, float dt)
    {
        float resist = Mathf.Lerp(m_maxResistance, m_currResistance, m_duration / m_startDuration);
        _actor.SetResistance(resist);
        m_duration -= dt;
    }

    public override void UpdateOnPlayer(Player_Controller _player, float dt)
    {
        m_duration -= dt;
        throw new NotImplementedException();
    }

    public override void EndActor(Enemy _actor)
    {
        //Delete vfx
        _actor.SetResistance(m_maxResistance);
    }

    public override void EndPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    protected override void LoadDisplayImage()
    {
        m_displayImage = Resources.Load<Sprite>("UI/Weakened");
    }

    protected override void LoadDisplayVFX()
    {
        //Debug.LogError("Burn status effect doesn't currently load the Display vfx.");
    }
}
