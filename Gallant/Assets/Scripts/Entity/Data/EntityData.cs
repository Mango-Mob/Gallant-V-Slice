using UnityEngine;

namespace EntitySystem.Data
{
    public class EntityData : ScriptableObject
    {
        [Header("Base Entity Stats")]
        public float Name;
        public float HP;
        public float Speed;
        public float Stamina;
        public float Defence;
        public float Ward;
    }
}
