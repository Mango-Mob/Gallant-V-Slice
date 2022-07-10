using UnityEngine;

namespace EntitySystem.Data
{
    [CreateAssetMenu(fileName = "entityData", menuName = "Game Data/Entity Data", order = 1)]
    public class EntityData : ScriptableObject
    {
        [Header("Base Entity Stats")]
        public string Name;
        public float HP;
        public float Speed;
        public float Stamina;
        public float Defence;
        public float Ward;
    }
}
