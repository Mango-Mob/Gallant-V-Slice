using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace ActorSystem.AI.Bosses
{
    public class Boss_Castle : Boss_Actor
    {
        protected override void Awake()
        {
            base.Awake();
            base.SetTarget(GameManager.Instance.m_player);
        }

        protected override void Start()
        {
            base.Start();
        }

        protected override void Update()
        {
            base.Update();
        }

        public override void DealImpactDamage(float amount, float piercingVal, Vector3 direction, CombatSystem.DamageType _type)
        {

        }
        public override bool DealDamage(float _damage, CombatSystem.DamageType _type, float piercingVal = 0, Vector3? _damageLoc = null)
        {
            return false;
        }
        public override bool DealDamageSilent(float _damage, CombatSystem.DamageType _type)
        {
            return false;
        }
    }
}
