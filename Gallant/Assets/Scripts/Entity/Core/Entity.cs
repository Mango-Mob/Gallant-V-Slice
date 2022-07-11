using EntitySystem.Data;
using UnityEngine;

namespace EntitySystem.Core
{
    /// <summary>
    /// Base entity class that is the parent to all moving gameobjects.
    /// </summary>
    public abstract class Entity : MonoBehaviour
    {
        public EntityData DataOnLoad;

        [Header("Base Entity Stats")]
        //Current Health and maximum health
        public float HP;
        public float MaxHP { get { return DataOnLoad.HP; } }

        //Current speed and default speed
        public float Speed;
        public float DefaultSpeed { get { return DataOnLoad.Speed; } }

        //Current Stamina and maximum stamina
        public float Stamina;
        public float MaxStamina { get { return DataOnLoad.Stamina; } }

        //Current Defence and maximum defence
        public float Defence;
        public float DefaultDefence { get { return DataOnLoad.Defence; } }

        //Current Ward and maximum ward
        public float Ward;
        public float DefaultWard { get { return DataOnLoad.Ward; } }

        public bool IsDead { get { return HP <= 0; } }

        public enum DamageType
        {
            Physical,
            Ability,
            True
        }

        /// <summary>
        /// Struct that contains damage instance data
        /// </summary>
        public struct DamageInstance
        {
            public DamageInstance(float _value, // Damage value applied to target
            float _pen, // Penetration value of damage
            DamageType _type, // Type of instance damage
            GameObject _source = null, // Source of instance damage
            bool __bypassInvincibility = false // Whether the attack bypasses invincibility frames
            ){
                value =_value;
                pen =_pen;
                type = _type;
                source = _source;
                bypassInvincibility = __bypassInvincibility;
            }

            public float value; // Damage value applied to target
            public float pen; // Penetration value of damage
            public DamageType type; // Type of instance damage
            public GameObject source; // Source of instance damage
            public bool bypassInvincibility;
        }

        protected virtual void Awake()
        {
            if(DataOnLoad)
            {
                RefreshEntity();
            }
            else
            {
                Debug.LogError($"Entity {this.name} was loaded without data. Entity was destroyed.");
                Destroy(this);
            }
        }

        /// <summary>
        /// Damages the entity directly, and calculates the damage dealt depending on the penitration and type of damage.
        /// </summary>
        /// <param name="_damage">Total damage dealt to the entity</param>
        /// <param name="_pen">Total penitration of the attack</param>
        /// <param name="_type">Type of damage of the attack</param>
        /// <param name="_source">Source of the damage</param>
        /// <param name="_playHurtSound">Weither the attack should play hurt sounds.</param>
        /// <returns>Status of the entity, if it has been killed by this attack.</returns>
        public abstract bool DealDamageToEntity(DamageInstance _damage, bool _playHurtSound = false);

        /// <summary>
        /// Reset's this entity back to spawning stats
        /// </summary>
        protected virtual void RefreshEntity()
        {
            this.HP = this.MaxHP;
            this.Speed = this.DefaultSpeed;
            this.Stamina = this.MaxStamina;
            this.Defence = this.DefaultDefence;
            this.Ward = this.DefaultWard;
        }

        protected virtual float GetResistanceValue(DamageType _type)
        {
            switch (_type)
            {
                case DamageType.Physical:
                    return Defence;
                case DamageType.Ability:
                    return Ward;
                default:
                case DamageType.True:
                    return 0.0f;
            }
        }

        /// <summary>
        /// Calculates the modifier to be multiplied to the damage.
        /// </summary>
        /// <param name="_type">Type of the attack</param>
        /// <param name="_resist">Amount of this entites' resistance</param>
        /// <param name="_atkPen">Total Penitration of the attack</param>
        /// <returns>The value to multiply the damage with.</returns>
        public static float CalculateDamageNegated(DamageType _type, float _resist, float _atkPen)
        {
            _resist *= (100f / (100f + _atkPen));

            switch (_type)
            {
                case DamageType.Physical:
                case DamageType.Ability:
                    return (100f / (100f + _resist));
                default:
                case DamageType.True:
                    return 1f;
            }
        }
    }
}
