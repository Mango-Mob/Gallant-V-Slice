using ActorSystem.AI;
using System;
using UnityEngine;
using PlayerSystem;

/****************
 * BurnStatus : A status effect that burns the victim and slowly damages them over time. 
 *              Strength = Damage per second
 * @author : Michael Jordan
 * @file : SlowStatus.cs
 * @year : 2021
 */
public class BurnStatus : StatusEffect
{
    public BurnStatus(float str, float dur) : base(str, dur) { }

    public float timer = 0;
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
    public override StatusEffect Clone()
    {
        return new BurnStatus(m_strength, m_duration);
    }
    public override void StartActor(Actor _actor, Transform headLoc)
    {
        //Show vfx
        m_vfxInWorld = GameObject.Instantiate(m_vfxDisplayPrefab, _actor.m_selfTargetTransform);
    }

    public override void StartPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
    }

    public override void UpdateOnActor(Actor _actor, float dt)
    {
        _actor.DealDamageSilent(m_strength * dt, CombatSystem.DamageType.Ability);
        
        m_duration -= dt;

        timer += dt;
        if (timer > 1)
        {
            HUDManager.Instance.GetDamageDisplay().DisplayDamage(_actor.transform, CombatSystem.DamageType.Ability, m_strength);
            timer -= 1;
        }
    }

    public override void UpdateOnPlayer(Player_Controller _player, float dt)
    {
        m_duration -= dt;
    }

    public override void EndActor(Actor _actor)
    {
        HUDManager.Instance.GetDamageDisplay().DisplayDamage(_actor.transform, CombatSystem.DamageType.Ability, m_strength * timer);
    }

    public override void EndPlayer(Player_Controller _player)
    {
        throw new NotImplementedException();
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
