using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]

public class SerializedWeapon
{
    public Weapon weaponType; // The type of weapon the object is.
    public int m_level = 0;
    public int m_damage = 10;
    public float m_speed = 1;
    public float m_knockback = 1;
    public float m_projectileSpeed = 0;
    public string m_weaponModel;

    public static SerializedWeapon SerializeWeapon(WeaponData _data)
    {
        SerializedWeapon weapon = new SerializedWeapon();
        if (_data == null)
        {
            Debug.Log("No weapon to store");
            weapon.m_level = -2;
            return weapon;
        }

        weapon.weaponType = _data.weaponType;

        weapon.m_level = _data.m_level;
        weapon.m_damage = _data.m_damage;
        weapon.m_speed = _data.m_speed;
        weapon.m_knockback = _data.m_knockback;
        weapon.m_projectileSpeed = _data.m_projectileSpeed;
        weapon.m_weaponModel = _data.weaponModelPrefab.name;

        return weapon;
    }

    public static WeaponData DeserializeWeapon(SerializedWeapon _weapon)
    {
        if (_weapon.m_level == -2)
            return null;

        WeaponData data = WeaponData.GenerateSpecificWeapon(_weapon.m_level, _weapon.weaponType, Ability.NONE, 1);

        data.m_damage = _weapon.m_damage;
        data.m_speed = _weapon.m_speed;
        data.m_knockback = _weapon.m_knockback;
        data.m_projectileSpeed = _weapon.m_projectileSpeed;
        data.weaponModelPrefab = Resources.Load<GameObject>("Weapons/Held Weapons/" + _weapon.m_weaponModel);
        
        return data;
    }
}


/****************
 * WeaponData: Scriptable object containing data for weapon
 * @author : William de Beer
 * @file : WeaponData.cs
 * @year : 2021
 */
[CreateAssetMenu(fileName = "weaponData", menuName = "Game Data/Weapon Data", order = 1)]
[System.Serializable]
public class WeaponData : ScriptableObject
{
    public string weaponName;
    public Weapon weaponType; // The type of weapon the object is.
    public GameObject weaponModelPrefab; // Prefab of weapon model to be used when player is holding it

    public bool isTwoHanded = false;
    public Sprite weaponIcon;
    public float hitCenterOffset = 0.75f;
    public float hitSize = 1.0f;
    public AbilityData abilityData;
    public ItemEffect itemEffect; // Only for weapons with passives.

    public string overrideAnimation = "";

    [Header("Base Weapon Stats")]
    public int m_level = 0;
    public int m_damage = 10; 
    public float m_speed = 1;
    public float m_knockback = 1;
    public float m_projectileSpeed = 0;
    public float m_attackMoveSpeed = 0.5f;

    [Header("Dropped Weapon Data")]
    public float m_dropScaleMultiplier = 1.0f;

    private static int m_minDamagePerLevel = 1;
    private static int m_maxDamagePerLevel = 2;

    private static float m_minSpeedPerLevel = 0.05f;
    private static float m_maxSpeedPerLevel = 0.2f;

    public static WeaponData GenerateWeapon(int _level)
    {
        WeaponData data = CreateInstance<WeaponData>();

        // Random weapon type.
        Weapon newWeaponType = (Weapon)Random.Range(0, System.Enum.GetValues(typeof(Weapon)).Length);
        ApplyWeaponData(data, newWeaponType);

        data.abilityData = null;

        // Damage / Speed are randomly assigned (between a range that increases based on the level value).
        data.SetDamage(data.m_damage + (int)(Random.Range(m_minDamagePerLevel, m_maxDamagePerLevel) * (_level - 1.0f)));
        data.SetSpeed(data.m_speed + (Random.Range(m_minSpeedPerLevel, m_maxSpeedPerLevel) * (_level - 1.0f)));

        data.SetLevel(_level);

        //// Damage / Speed are randomly assigned (between a range that increases based on the level value).
        //data.m_damage += (int)(data.m_damage * Random.Range(0.1f, 0.2f) * (_level - 1.0f));
        //data.m_speed += (data.m_speed * Random.Range(0.05f, 0.1f) * (_level - 1.0f));

        //data.m_level = _level;

        // Random ability and power level is assigned.
        Ability newAbilityType;
        int iter = 0;
        do
        {
            newAbilityType = (Ability)UnityEngine.Random.Range(0, System.Enum.GetValues(typeof(Ability)).Length - 1);
        
            int curve = UnityEngine.Random.Range(0, 3) + UnityEngine.Random.Range(0, 3) - 2;
            int result = Mathf.Max(_level + curve, 0);

            // Power level
            int powerLevel = 0;
            if (result < 3)
                powerLevel = 1;
            else if (result < 6)
                powerLevel = 2;
            else
                powerLevel = 3;

            // Ability
            ApplyAbilityData(data, newAbilityType, powerLevel);
            iter++; 
            if (iter > 20)
            {
                ApplyAbilityData(data, Ability.ARCANE_BOLT, powerLevel);
                break;
            }
        } while (newWeaponType == Weapon.STAFF && data.abilityData.isPassive);
        

        // Create weapon name
        if (data.abilityData != null)
        { 
            data.weaponName = data.weaponName + " of " + data.abilityData.weaponTitle;
            data.abilityData.lastCooldown = 0.0f;
        }

        //Return new weapon.
        return data;
    }

    public static WeaponData GenerateSpecificWeapon(int _weaponLevel, Weapon _weaponType, Ability _abilityType, int _powerLevel)
    {
        WeaponData data = CreateInstance<WeaponData>();

        ApplyWeaponData(data, _weaponType);
        if (data == null)
        {
            Debug.LogWarning("Could not create weapon due to inavlid weapon type randomised.");
            return null;
        }

        data.abilityData = null;

        //// Damage / Speed are randomly assigned (between a range that increases based on the level value).
        //data.m_damage += (int)(data.m_damage * Random.Range(0.05f, 0.1f) * (_weaponLevel - 1.0f));
        //data.m_speed += (data.m_speed * Random.Range(0.05f, 0.1f) * (_weaponLevel - 1.0f));

        //data.m_level = _powerLevel;

        // Damage / Speed are randomly assigned (between a range that increases based on the level value).
        data.SetDamage(data.m_damage + (int)(Random.Range(m_minDamagePerLevel, m_maxDamagePerLevel) * (_weaponLevel - 1.0f)));
        data.SetSpeed(data.m_speed + (Random.Range(m_minSpeedPerLevel, m_maxSpeedPerLevel) * (_weaponLevel - 1.0f)));

        data.SetLevel(_weaponLevel);

        ApplyAbilityData(data, _abilityType, _powerLevel);

        // Create weapon name
        if (data.abilityData != null)
        {
            data.weaponName = data.weaponName + " of " + data.abilityData.weaponTitle;
            data.abilityData.lastCooldown = 0.0f;
        }

        //Return new weapon.
        return data;
    }
    private static void ApplyWeaponData(WeaponData _data, Weapon _weaponType)
    {
        // Weapon type.
        switch (_weaponType)
        {
            case Weapon.SWORD:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/swordData"));
                break;
            case Weapon.SHIELD:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/shieldData"));
                break;
            case Weapon.BOOMERANG:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/boomerangData"));
                break;
            case Weapon.CROSSBOW:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/crossbowData"));
                break;
            case Weapon.SPEAR:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/spearData"));
                break;
            case Weapon.BRICK:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/brickData"));
                break;
            case Weapon.AXE:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/axeData"));
                break;
            case Weapon.STAFF:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/staffData"));
                break;
            case Weapon.GREATSWORD:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/greatswordData"));
                break;
            case Weapon.BOW:
                _data.Clone(Resources.Load<WeaponData>("Data/BaseWeapons/bowData"));
                break;
        }
    }
    public static void ApplyAbilityData(WeaponData _data, Ability _abilityType, int _powerLevel)
    {
        // Ability
        switch (_abilityType)
        {
            case Ability.NONE:
                _data.abilityData = null;
                break;
            case Ability.FIREWAVE:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/firewave" + _powerLevel.ToString());
                break;
            case Ability.SAND_MISSILE:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/sandmissile" + _powerLevel.ToString());
                break;
            case Ability.LIGHTNING_BOLT:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/lightning" + _powerLevel.ToString());
                break;
            case Ability.ICE_ROLL:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/frostevade" + _powerLevel.ToString());
                break;
            case Ability.HP_BUFF:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/barrier" + _powerLevel.ToString());
                break;
            case Ability.THORNS:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/thorns" + _powerLevel.ToString());
                break;
            case Ability.ARCANE_BOLT:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/arcanebolt" + _powerLevel.ToString());
                break;
            case Ability.FLAME_ROLL:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/flameevade" + _powerLevel.ToString());
                break;
            case Ability.ROLL_BASH:
                _data.abilityData = Resources.Load<AbilityData>("Data/Abilities/rollBash" + _powerLevel.ToString());
                break;
            default:
                Debug.LogWarning("Could not add ability due to inavlid ability type randomised.");
                break;
        }
    }

    public static WeaponData UpgradeWeaponLevel(WeaponData _data)
    {
        WeaponData data = CreateInstance<WeaponData>();
        data.Clone(_data);

        data.SetDamage(data.m_damage + Random.Range(m_minDamagePerLevel, m_maxDamagePerLevel));
        data.SetSpeed(data.m_speed + Random.Range(m_minSpeedPerLevel, m_maxSpeedPerLevel));

        data.SetLevel(data.m_level + 1);


        return data;
    }

    public void SetDamage(int _damage)
    {
        this.m_damage = _damage;
    }
    public void SetSpeed(float _speed)
    {
        this.m_speed = _speed;
    }
    public void SetLevel(int _level)
    {
        this.m_level = _level;
    }

    public static string GetTags(Weapon type)
    {
        switch (type)
        {
            case Weapon.SWORD:
                return "One hand, Melee";
            case Weapon.SPEAR:
                return "Two hands, Melee";
            case Weapon.BOOMERANG:
                return "One hand, Ranged";
            case Weapon.SHIELD:
                return "One hand, Melee";
            case Weapon.CROSSBOW:
                return "One hand, Ranged";
            case Weapon.AXE:
                return "One hand, Melee";
            case Weapon.STAFF:
                return "One hand, Melee";
            case Weapon.GREATSWORD:
                return "Two hands, Melee";
            case Weapon.BOW:
                return "Two hands, Ranged";
            default:
            case Weapon.BRICK:
                return "";
		}
	}
	
    public string GetPassiveEffectDescription()
    {
        switch (itemEffect)
        {
            case ItemEffect.MOVE_SPEED:
                return "Higher movement speed.";
            case ItemEffect.ABILITY_CD:
                return "Lower ability cooldowns.";
            case ItemEffect.ATTACK_SPEED:
                return "Higher attack speed.";
            case ItemEffect.DAMAGE_RESISTANCE:
                return "Higher damage resistance.";
            case ItemEffect.MAX_HEALTH_INCREASE:
                return "Higher maximum health.";
            default:
                return null;
        }
    }

    public void Clone(WeaponData other)
    {
        this.weaponName = other.weaponName;
        this.weaponType = other.weaponType;
        this.weaponModelPrefab = other.weaponModelPrefab;

        this.isTwoHanded = other.isTwoHanded;
        this.weaponIcon = other.weaponIcon;
        this.hitCenterOffset = other.hitCenterOffset;
        this.hitSize = other.hitSize;
        this.abilityData = other.abilityData;
        this.itemEffect = other.itemEffect;

        this.overrideAnimation = other.overrideAnimation;

        this.m_level = other.m_level;
        this.m_damage = other.m_damage;
        this.m_speed = other.m_speed;
        this.m_knockback = other.m_knockback;
        this.m_projectileSpeed = other.m_projectileSpeed;
        this.m_attackMoveSpeed = other.m_attackMoveSpeed;
    }
}
