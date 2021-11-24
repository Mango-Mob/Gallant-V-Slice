using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AudioAgent : MultiAudioAgent
{
    #region Weapon
    public void PlayWeaponSwing()
    {
        base.PlayOnce("WeaponSwing", false, Random.Range(0.95f, 1.05f));
    }

    public void PlayShieldBlock()
    {
        base.PlayOnce("ShieldBlock", false, Random.Range(0.95f, 1.05f));
    }
    public void PlayWeaponHit(Weapon _weapon)
    {
        switch (_weapon)
        {
            case Weapon.SWORD:
                base.PlayOnce("SwordCut", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.SHIELD:
                base.PlayOnce("ShieldSlam", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.BOOMERANG:
                base.PlayOnce("BoomerangImpact", false, Random.Range(0.95f, 1.05f));
                break;
            default:
                Debug.Log("No weapon audio attached to weapon type.");
                break;
        }
    }
    #endregion

    #region Abilities
    public void FirewaveLaunch()
    {
        base.PlayOnce("FireballCast", false, Random.Range(0.95f, 1.05f));
    }
    public void Iceroll()
    {
        base.PlayOnce("IceRoll", false, Random.Range(0.95f, 1.05f));
    }
    public void Lightning()
    {
        base.PlayOnce("Lightning", false, Random.Range(0.95f, 1.05f));
    }
    public void SandmissileImpact()
    {
        base.PlayOnce("SandImpact", false, Random.Range(0.95f, 1.05f));
    }
    public void BarrierLaunch()
    {
        base.PlayOnce("BarrierLaunch", false, Random.Range(0.95f, 1.05f));
    }
    public void ReflectiveShield()
    {
        base.PlayOnce("Thorns", false, Random.Range(0.95f, 1.05f));
    }
    #endregion

    #region Pickup
    public void PlayOrbPickup()
    {
        base.PlayOnce("OrbPickup", false, Random.Range(0.95f, 1.05f));
    }
    public void EquipWeapon()
    {
        base.PlayOnce("Pickup", false, Random.Range(0.95f, 1.05f));
    }
    #endregion

    #region Other
    public void PlayUseAdrenaline()
    {
        base.PlayOnce("UseAdrenaline", false, 1.0f);
    }
    public void PlayDeath()
    {
        base.PlayOnce("PlayerDeath", false, Random.Range(0.95f, 1.05f));
    }
    #endregion
}