using ActorSystem.AI;
using System;
using UnityEngine;
/****************
 * SlowStatus : A status effect that slows the victim and slowly speeds them up while the duration ends. 
 *              Strength = percentage of slow.
 * @author : Michael Jordan
 * @file : SlowStatus.cs
 * @year : 2021
 */
public class SlowStatus : StatusEffect
{
    public SlowStatus(float str, float dur) : base(str, dur) { }

    public override bool ReactTo(StatusEffect other)
    {
        if (other.GetType() == typeof(SlowStatus))
        {
            if (m_strength == (other as SlowStatus).m_strength)
            {
                m_duration = Mathf.Max(m_duration, (other as SlowStatus).m_duration);
                m_startDuration = Mathf.Max(m_startDuration, m_duration);
            }
            else
            {
                m_strength = Mathf.Max(m_strength, (other as SlowStatus).m_strength);
                m_duration = (other as SlowStatus).m_duration;
                m_startDuration = m_duration;
            }
            return true;
        }
        return false;
    }
    public override StatusEffect Clone()
    {
        return new SlowStatus(m_strength, m_duration);
    }
    public override void StartActor(Actor _actor, Transform headLoc)
    {
        if(_actor.m_myBrain.m_legs != null && !_actor.m_myBrain.IsStunned)
            _actor.m_myBrain.m_legs.m_speedModifier = 1.0f - m_strength;

        foreach (var item in _actor.m_myBrain.m_materials)
        {
            item.StartFreeze(0.6f);
        }
    }

    public override void StartPlayer(Player_Controller _player)
    {
        
    }

    public override void UpdateOnActor(Actor _actor, float dt)
    {
        if (_actor.m_myBrain.m_legs != null && !_actor.m_myBrain.IsStunned)
        {
            float lerp = 1.0f - m_duration / m_startDuration;
            float strength = Mathf.Lerp(1.0f - m_strength, 1.0f, lerp);
            _actor.m_myBrain.m_legs.m_speedModifier = strength;

            foreach (var item in _actor.m_myBrain.m_indicators)
            {
                item.m_speed = Mathf.Min(strength, item.m_speed);
            }
        }

        m_duration -= dt;
    }

    public override void UpdateOnPlayer(Player_Controller _player, float dt)
    {
        m_duration -= dt;
        throw new NotImplementedException();
    }

    public override void EndActor(Actor _actor)
    {
        if (_actor.m_myBrain.m_legs != null)
            _actor.m_myBrain.m_legs.m_speedModifier = 1.0f;

        foreach (var material in _actor.m_myBrain.m_materials)
        {
            material.RefreshColor();
        }

        foreach (var item in _actor.m_myBrain.m_indicators)
        {
            item.m_speed = 1.0f;
        }
    }

    public override void EndPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    protected override void LoadDisplayImage()
    {
        m_displayImage = Resources.Load<Sprite>("UI/Slow");
    }

    protected override void LoadDisplayVFX()
    {
        //Debug.LogError("Slow status effect doesn't currently load the Display vfx.");
    }
}
