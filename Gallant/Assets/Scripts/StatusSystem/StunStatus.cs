using ActorSystem.AI;
using System;
using UnityEngine;

public class StunStatus : StatusEffect
{
    public StunStatus(float dur) : base(dur, dur) { }

    public override void StartActor(Actor _actor)
    {
        //Show VFX
        _actor.m_myBrain.m_legs.m_speedModifier = 0.0f;
        _actor.m_myBrain.IsStunned = true;
    }

    public override void StartPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    public override void EndActor(Actor _actor)
    {
        _actor.m_myBrain.m_legs.m_speedModifier = 1.0f;
        _actor.m_myBrain.IsStunned = false;
    }

    public override void EndPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    public override bool ReactTo(StatusEffect other)
    {
        if (other.GetType() == typeof(StunStatus))
        {
            if(m_duration > (other as StunStatus).m_duration)
            {
                //don't bother
            }
            else
            {
                //Take the new stun
                m_duration = Mathf.Max(m_duration, (other as StunStatus).m_duration);
                m_startDuration = Mathf.Max(m_startDuration, m_duration);
            }
            return true;
        }
        return false;
    }

    public override void UpdateOnActor(Actor _actor, float dt)
    {
        _actor.m_myBrain.m_legs.m_speedModifier = 0.0f;
        m_duration -= dt;
    }

    public override void UpdateOnPlayer(Player_Controller _player, float dt)
    {
        m_duration -= dt;
        throw new NotImplementedException();
    }

    protected override void LoadDisplayImage()
    {
        m_displayImage = Resources.Load<Sprite>("UI/Stun");
    }

    protected override void LoadDisplayVFX()
    {
        m_vfxDisplayPrefab = Resources.Load<GameObject>("VFX/Stun");
    }
}
