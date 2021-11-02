using System;
using UnityEngine;

/****************
 * BurnStatus : A status effect that burns the victim and slowly damages them over time. 
 *              Strength = Damage per second
 * @author : Michael Jordan
 * @file : SlowStatus.cs
 * @year : 2021
 */
public class BurnStatus : StatusEffect
{
    public BurnStatus(float str, float dur) : base(str, dur) { m_displayColor = Color.red; }

    public override bool ReactTo(StatusEffect other)
    {
        if (other.GetType() == typeof(BurnStatus))
        {
            m_strength = Mathf.Max(m_strength, (other as BurnStatus).m_strength);
            m_duration = Mathf.Max(m_duration, (other as BurnStatus).m_duration);
            m_startDuration = Mathf.Max(m_startDuration, m_duration);
            return true;
        }
        return false;
    }

    public override void StartActor(Actor _actor)
    {
        //Show vfx
    }

    public override void StartPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    public override void UpdateOnActor(Actor _actor, float dt)
    {
        _actor.DealDamage(m_strength * dt);
        m_duration -= dt;
    }

    public override void UpdateOnPlayer(Player_Controller _player, float dt)
    {
        m_duration -= dt;
        throw new NotImplementedException();
    }

    public override void EndActor(Actor _actor)
    {
        //Delete vfx
    }

    public override void EndPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    protected override void LoadDisplayImage()
    {
        m_displayImage = Resources.Load<Sprite>("UI/BurnStatusIcon");
    }

    protected override void LoadDisplayVFX()
    {
        //Debug.LogError("Burn status effect doesn't currently load the Display vfx.");
    }
}
