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
        protected float MaxHP { get { return DataOnLoad.HP; } }

        //Current speed and default speed
        public float Speed;
        protected float DefaultSpeed { get { return DataOnLoad.Speed; } }

        //Current Stamina and maximum stamina
        public float Stamina;
        protected float MaxStamina { get { return DataOnLoad.Stamina; } }

        //Current Defence and maximum defence
        public float Defence;
        protected float DefaultDefence { get { return DataOnLoad.Defence; } }

        //Current Ward and maximum ward
        public float Ward;
        protected float DefaultWard { get { return DataOnLoad.Ward; } }

        public bool IsDead { get { return HP <= 0; } }

        public enum DamageType
        {
            Physical,
            Ability,
            True
        }

        public struct Damage
        {

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
        public abstract bool DealDamageToEntity(Damage _damage, GameObject _source = null, bool _playHurtSound = false);

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
