using ActorSystem.AI;
using ActorSystem.AI.Components;
using UnityEngine;
using UnityEngine.AI;

namespace ActorSystem.Data
{
    [CreateAssetMenu(fileName = "UtilityAttack_Data", menuName = "Game Data/Attack Data/Utility", order = 1)]
    public class UtilityAttackData : AttackData
    {
        public enum UtilityType { Teleport}
        public UtilityType m_type;
        public float m_intensity = 5;

        private Vector3 targetPoint;

        public override bool InvokeAttack(Transform parent, GameObject source, int filter, uint id = 0, float damageMod = 1)
        {
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
    }
}
