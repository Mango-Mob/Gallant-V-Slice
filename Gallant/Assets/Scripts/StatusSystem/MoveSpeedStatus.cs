using ActorSystem.AI;
using System;
using UnityEngine;

/****************
 * BurnStatus : A status effect that burns the victim and slowly damages them over time. 
 *              Strength = Damage per second
 * @author : Michael Jordan
 * @file : SlowStatus.cs
 * @year : 2021
 */
public class MoveSpeedStatus : StatusEffect
{
    public MoveSpeedStatus(float str, float dur) : base(str, dur) { }

    public override bool ReactTo(StatusEffect other)
    {
        if (other.GetType() == typeof(MoveSpeedStatus))
        {
            m_strength = Mathf.Max(m_strength, (other as MoveSpeedStatus).m_strength);
            m_duration = Mathf.Max(m_duration, (other as MoveSpeedStatus).m_duration);
            m_startDuration = Mathf.Max(m_startDuration, m_duration);
            return true;
        }
        return false;
    }

    public override void StartActor(Actor _actor, Transform headLoc)
    {
        //Show vfx
        m_vfxInWorld = GameObject.Instantiate(m_vfxDisplayPrefab, _actor.m_selfTargetTransform);
    }

    public override void StartPlayer(Player_Controller _player)
    {

    }

    public override void UpdateOnActor(Actor _actor, float dt)
    {
        m_duration -= dt;
        throw new NotImplementedException();
    }

    public override void UpdateOnPlayer(Player_Controller _player, float dt)
    {
        _player.playerSkills.m_movementSpeedStatusBonus = 1.0f + m_strength;
        m_duration -= dt;
    }

    public override void EndActor(Actor _actor)
    {
        throw new NotImplementedException();
    }

    public override void EndPlayer(Player_Controller _player)
    {
        _player.playerSkills.m_movementSpeedStatusBonus = 1.0f;
    }

    protected override void LoadDisplayImage()
    {
        m_displayImage = Resources.Load<Sprite>("UI/Burn");
    }

    protected override void LoadDisplayVFX()
    {
        m_vfxDisplayPrefab = Resources.Load<GameObject>("VFX/Burn");
    }
}
