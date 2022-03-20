using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player_AudioAgent : MultiAudioAgent
{
    #region Weapon

    public void PlayShieldBlock()
    {
        base.PlayOnce("ShieldBlock", false, Random.Range(0.95f, 1.05f));
    }
    public void PlayWeaponSwing(Weapon _weapon, int _sound = 1)
    {
        switch (_weapon)
        {
            case Weapon.CROSSBOW:
                base.PlayOnce("BowLaunch", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.BOW:
                base.PlayOnce("BowLoad", false, Random.Range(0.95f, 1.05f));
                break;
            default:
                base.PlayOnce("WeaponSwing", false, Random.Range(0.95f, 1.05f));
                break;
        }
    }
    public void PlayWeaponHit(Weapon _weapon, int _sound = 1)
    {
        switch (_weapon)
        {
            case Weapon.SWORD:
                base.PlayOnce("SwordCut", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.SHIELD:
                base.PlayOnce("ShieldSlam", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.BRICK:
                base.PlayOnce("BoomerangImpact", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.BOOMERANG:
                base.PlayOnce("BoomerangImpact", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.STAFF:
                base.PlayOnce("BowImpact", false, Random.Range(0.95f, 1.05f));
                break;
            case Weapon.BOW:
                switch (_sound)
                {
                    case 1:
                        base.PlayOnce("BowLaunch", false, Random.Range(0.95f, 1.05f));
                        break;
                    case 2:
                        base.PlayOnce("BowImpact", false, Random.Range(0.95f, 1.05f));
                        break;
                    default:
                        break;
                }
                break;
            case Weapon.CROSSBOW:
                base.PlayOnce("BowImpact", false, Random.Range(0.95f, 1.05f));
                break;
            default:
                base.PlayOnce("SwordCut", false, Random.Range(0.95f, 1.05f));
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

    public void PlayBasicFootstep(int state)
    {
        switch (state)
        {
            default:
            case 0://Wood
                base.PlayOnce("PlayerFootstepBasic", false, Random.Range(0.95f, 1.05f));
                break;
            case 1://Stone
                base.PlayOnce("PlayerFootstepStone", false, Random.Range(0.95f, 1.05f));
                break;
            case 2: //Dirt
                base.PlayOnce("PlayerFootstepDirt", false, Random.Range(0.95f, 1.05f));
                break;
            case 3://Water
                base.PlayOnce("PlayerFootstepWater", false, Random.Range(0.95f, 1.05f));
                break;
        }
    }
}