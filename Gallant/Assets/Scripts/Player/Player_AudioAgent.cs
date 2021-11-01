using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AudioAgent : MultiAudioAgent
{
    public void PlayStep()
    {
        base.PlayOnce("Step", false, Random.Range(0.85f, 1.25f));
    }
    public void PlayRoll()
    {
        base.PlayOnce("PlayerRoll", false, Random.Range(0.95f, 1.05f));
    }
    public void PlayAdrenalineGain()
    {
        base.PlayOnce("PlayerAdrenalineGain", false, 1.0f);
    }
    public void PlayUseAdrenaline()
    {
        base.PlayOnce("UseAdrenaline", false, 1.0f);
    }
    public void PlaySwordHit()
    {
        base.PlayOnce("SwordSwing", false, Random.Range(0.95f, 1.05f));
    }
    public void PlayWeaponSwing()
    {
        base.PlayOnce("AttackSwing", false, Random.Range(0.95f, 1.05f));
    }
    public void PlayCast()
    {
        base.PlayOnce("FireballCast", false, Random.Range(0.95f, 1.05f));
    }
    public void PlayDeath()
    {
        base.PlayOnce("PlayerDeath", false, Random.Range(0.95f, 1.05f));
    }
}