using ActorSystem.AI;
using System.Collections;
using UnityEngine;

namespace ActorSystem.Data
{

    [CreateAssetMenu(fileName = "InstantAttack_Data", menuName = "Game Data/Attack Data/Instant", order = 1)]
    public class InstantAttackData : AttackData
    {
        [Header("Instant Variables")]
        public GameObject projPrefab;
        public int attackCount;
        public float delay;

        public override bool InvokeAttack(Transform parent, GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
            int i = attackCount;
            Collider[] targets = GetDamageOverlap(parent, filter, id);

            foreach (var item in targets)
            {
                if (item.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    i--;
                    source.GetComponentInParent<Actor>().StartCoroutine(SpawnAfterDelay(item, delay, damageMod));
                    if (i <= 0) return true;
                }
            }
            return canContinueAfterMiss;
        }

        private IEnumerator SpawnAfterDelay(Collider target, float delay, float damageMod)
        {
            if (delay > 0)
                yield return new WaitForSeconds(delay);

            GameObject proj = GameObject.Instantiate(projPrefab, target.transform.position, Quaternion.identity);
            proj.GetComponent<AreaEffect>().m_data = this;
            proj.GetComponent<AreaEffect>().damage = baseDamage * damageMod;
            yield return null;
        }
    }
}
