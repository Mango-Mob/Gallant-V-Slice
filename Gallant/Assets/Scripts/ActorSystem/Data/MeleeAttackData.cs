using UnityEngine;

namespace ActorSystem.Data
{
    [CreateAssetMenu(fileName = "MeleeAttack_Data", menuName = "Game Data/Attack Data/Melee", order = 1)]
    public class MeleeAttackData : AttackData
    {
        [Header("Melee Variables")]
        public int hitsCount = 1;
        public bool ignoreIFrames = false;

        public override bool InvokeAttack(Transform parent, GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
            int i = hitsCount;
            Collider[] targets = GetDamageOverlap(parent, filter, id);
            foreach (var item in targets)
            {
                if (item.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    i--;
                    Player_Controller player = item.GetComponentInParent<Player_Controller>();
                    player.DamagePlayer(baseDamage * damageMod, damageType, source, ignoreIFrames);
                    ApplyEffect(player, parent, onHitEffect, effectPower);
                    if (i <= 0) return true;
                }
                else if (item.gameObject.layer == LayerMask.NameToLayer("Destructible"))
                {
                    i--;
                    if (i <= 0) return true;
                }
            }
            return canContinueAfterMiss;
        }
    }
}
