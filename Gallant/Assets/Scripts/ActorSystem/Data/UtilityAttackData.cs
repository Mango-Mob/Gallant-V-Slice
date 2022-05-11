using ActorSystem.AI;
using ActorSystem.AI.Components;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.Data
{
    [CreateAssetMenu(fileName = "UtilityAttack_Data", menuName = "Game Data/Attack Data/Utility", order = 1)]
    public class UtilityAttackData : AttackData
    {
        public enum UtilityType { Teleport, Tracker}
        public UtilityType m_type;
        public float m_intensity = 5;
        public float m_speed = 5f;
        public GameObject m_projPrefab;

        public AnimationCurve m_growthEquation;
        private float m_startIntensity;
        private Vector3 targetPoint;
        private GameObject m_proj; 
        public override bool InvokeAttack(Transform parent, GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
            switch (m_type)
            {
                case UtilityType.Teleport:
                    return true;
                case UtilityType.Tracker:
                    Instantiate(base.postVFXPrefab, m_proj.transform.position, Quaternion.identity);
                    ApplyEffect(damageMod);
                    m_intensity = m_startIntensity;
                    Destroy(m_proj);
                    break;
                default:
                    break;
            }
            return true;
        }

        public override void BeginActor(Actor user)
        {
            switch (m_type)
            {
                case UtilityType.Teleport:
                    (user.m_myBrain.m_legs as Actor_Leap).m_speedModifier = 5f;
                    targetPoint = GetRandomPoint(user);
                    user.SetTargetLocation(targetPoint, canTrackTarget);
                    break;
                case UtilityType.Tracker:
                    m_proj = Instantiate(m_projPrefab, user.transform.position, Quaternion.identity);
                    m_proj.transform.localScale = Vector3.one * 0.25f;
                    m_startIntensity = m_intensity;
                    break;
                default:
                    break;
            }
        }

        public override void UpdateActor(Actor user)
        {
            switch (m_type)
            {
                case UtilityType.Teleport:
                    user.SetTargetLocation(targetPoint, canTrackTarget);
                    if (Vector3.Distance(user.transform.position, targetPoint) < 0.5f)
                    {
                        user.m_myBrain.EndAttack();
                    }
                    break;
                case UtilityType.Tracker:
                    if(m_proj != null)
                    {
                        m_intensity -= Time.deltaTime;
                        m_proj.transform.localScale = Vector3.one * m_growthEquation.Evaluate(1.0f - m_intensity / m_startIntensity);

                        if (m_proj.transform.localScale.x != base.damageColliders[0].radius)
                        {
                            Vector3 direct = (GameManager.Instance.m_player.transform.position - m_proj.transform.position);
                            float dist = Mathf.Min(m_speed, direct.magnitude) * Time.deltaTime;
                            m_proj.transform.position += direct.normalized * dist;
                        }
                    }
                    break;
                default:
                    break;
            }
        }

        public override void EndActor(Actor user)
        {
            switch (m_type)
            {
                case UtilityType.Teleport:
                    base.EndActor(user);
                    (user.m_myBrain.m_legs as Actor_Leap).m_speedModifier = 1f;
                    break;
                default:
                    break;
            } 
        }

        private Vector3 GetRandomPoint(Actor user)
        {
            Vector3 center = user.m_target.transform.position;
            float dist = Vector3.Distance(user.transform.position, user.m_target.transform.position);
            Vector3 bestPoint = user.transform.position;
            for (int i = 0; i < 10; i++)
            {
                Vector2 direct2 = Random.insideUnitCircle * m_intensity;
                NavMeshHit samplePoint;
                if(NavMesh.SamplePosition(center + new Vector3(direct2.x, 0, direct2.y), out samplePoint, m_intensity/2f, ~0))
                {
                    float currDist = Vector3.Distance(user.transform.position, samplePoint.position);
                    if (currDist > dist)
                    {
                        dist = currDist;
                        bestPoint = samplePoint.position;
                    }
                }
            }
            return bestPoint;
        }

        public override void PostInvoke(Transform user, uint id)
        {

        }

        private void ApplyEffect(float damMod = 1.0f)
        {
            Collider[] hits = Physics.OverlapSphere(m_proj.transform.position, base.damageColliders[0].radius);

            foreach (var hit in hits)
            {
                if (hit.gameObject.layer == LayerMask.NameToLayer("Player"))
                {
                    Player_Controller player = hit.GetComponent<Player_Controller>();
                    if (player != null)
                    {
                        player.DamagePlayer(damMod * base.baseDamage, CombatSystem.DamageType.Ability);
                        AttackData.ApplyEffect(player, m_proj.transform, base.onHitEffect, base.effectPower);
                    }
                }
            }
        }
    }
}
