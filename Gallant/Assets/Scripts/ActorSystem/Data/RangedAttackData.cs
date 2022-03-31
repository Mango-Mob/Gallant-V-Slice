using UnityEngine;

namespace ActorSystem.Data
{
    [CreateAssetMenu(fileName = "RangedAttack_Data", menuName = "Game Data/Attack Data/Ranged", order = 1)]
    public class RangedAttackData : AttackData
    {
        [Header("Ranged Variables")]
        public GameObject projPrefab;
        public GameObject spawnVfxPrefab;
        public int projCount = 1;
        public float projLifeTime = 5;
        public float projSpeed;
        public float requiredAngle = 180;

        public override bool InvokeAttack(Transform parent, GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
            int i = projCount;
            Collider[] targets = GetDamageOverlap(parent, filter, id);

            foreach (var item in targets)
            {
                if (item.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Quaternion lookAt = Quaternion.LookRotation((item.transform.position - source.transform.position).normalized, Vector3.up);

                    if (Mathf.Abs(Quaternion.Angle(source.transform.rotation, lookAt)) <= requiredAngle)
                    {
                        i--;

                        //Create projectile
                        GameObject prefabInWorld = GameObject.Instantiate(projPrefab, source.transform.position, Quaternion.LookRotation(source.transform.forward, Vector3.up));
                        ProjectileObject projInWorld = prefabInWorld.GetComponent<ProjectileObject>();
                        projInWorld.m_damageDetails = this;
                        projInWorld.m_velocity = ((item.transform.position + Vector3.up * 0.5f) - source.transform.position).normalized * projSpeed;
                        projInWorld.m_damage = damageMod * baseDamage;
                        projInWorld.m_duration = projLifeTime;

                        if (spawnVfxPrefab != null)
                        {
                            GameObject vfx = GameObject.Instantiate(spawnVfxPrefab, source.transform.position, Quaternion.LookRotation(source.transform.forward, Vector3.up));
                        }
                        if (i <= 0) return true;
                    }
                }
            }
            return canContinueAfterMiss;
        }
    }
}
