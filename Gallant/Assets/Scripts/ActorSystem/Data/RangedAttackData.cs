using ActorSystem.AI;
using UnityEngine;

namespace ActorSystem.Data
{
    [CreateAssetMenu(fileName = "RangedAttack_Data", menuName = "Game Data/Attack Data/Ranged", order = 1)]
    public class RangedAttackData : AttackData
    {
        [Header("Ranged Variables")]
        public GameObject projPrefab;
        public GameObject spawnVfxPrefab;
        public float scale = 1.0f;
        public int projCount = 1;
        public float projLifeTime = 5;
        public float projSpeed;
        public float requiredAngle = 180;
        public LayerMask filter; 

        public override bool CanAttack(Transform parent, GameObject target)
        {
            Vector3 forward = (target.transform.position - parent.transform.position).normalized;
            Quaternion lookAt = Quaternion.LookRotation(forward, Vector3.up);
            bool angleReq = Mathf.Abs(Quaternion.Angle(parent.transform.rotation, lookAt)) <= requiredAngle;
            Collider hit = Extentions.GetClosestCollider(parent.gameObject, Physics.SphereCastAll(parent.transform.position, 2, forward, 100, filter));

            return angleReq && hit.gameObject == target;
        }

        public override bool InvokeAttack(Transform parent, ref GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
            int i = projCount;
            Collider[] targets = GetDamageOverlap(parent, filter, id);

            foreach (var item in targets)
            {
                if (item.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Quaternion lookAt = Quaternion.LookRotation((item.transform.position - parent.transform.position).normalized, Vector3.up);

                    if (Mathf.Abs(Quaternion.Angle(parent.transform.rotation, lookAt)) <= requiredAngle)
                    {
                        i--;

                        //Create projectile
                        GameObject prefabInWorld = GameObject.Instantiate(projPrefab, source.transform.position, Quaternion.LookRotation(source.transform.forward, Vector3.up));
                        prefabInWorld.transform.localScale = Vector3.one * scale;
                        ProjectileObject projInWorld = prefabInWorld.GetComponent<ProjectileObject>();
                        projInWorld.m_damageDetails = this;
                        projInWorld.m_velocity = ((item.transform.position + Vector3.up * 1.0f) - source.transform.position).normalized * projSpeed;
                        projInWorld.m_damage = damageMod * baseDamage;
                        projInWorld.m_duration = projLifeTime;

                        if (spawnVfxPrefab != null)
                        {
                            GameObject vfx = GameObject.Instantiate(spawnVfxPrefab, source.transform.position, Quaternion.LookRotation(source.transform.forward, Vector3.up));
                            vfx.transform.localScale = Vector3.one * scale;
                        }
                        if (i <= 0) return true;
                    }
                }
            }
            return canContinueAfterMiss;
        }
    }
}
