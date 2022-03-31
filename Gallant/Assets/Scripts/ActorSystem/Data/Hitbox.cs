using UnityEngine;

namespace ActorSystem.Data
{
    [System.Serializable]
    public class Hitbox
    {
        public enum HitType { Box, Sphere, Capsule }
        public Vector3 start;
        public Vector3 center;
        public Vector3 end;
        public HitType type;
        public Vector3 size;
        public float radius;

        public Collider[] GetOverlappingObjects(Transform parent, int filterLayer)
        {
            switch (type)
            {
                case HitType.Box:
                    return Physics.OverlapBox(parent.position + parent.TransformVector(center), size / 2f, parent.rotation, filterLayer);
                default:
                case HitType.Sphere:
                    return Physics.OverlapSphere(parent.position + parent.TransformVector(start), radius, filterLayer);
                case HitType.Capsule:
                    return Physics.OverlapCapsule(parent.position + parent.TransformVector(start), parent.position + parent.TransformVector(end), radius, filterLayer);
            }
        }

        public Vector3 GetHitlocation(Transform parent)
        {
            switch (type)
            {
                default:
                case HitType.Box:
                case HitType.Sphere:
                    return parent.position + parent.TransformVector(center);
                case HitType.Capsule:
                    return Extentions.MidPoint(parent.position + parent.TransformVector(start), parent.position + parent.TransformVector(end));
            }
        }

        public void DrawGizmos(Transform parent)
        {
            Gizmos.matrix = parent.localToWorldMatrix;

            switch (type)
            {
                case HitType.Box:
                    Gizmos.DrawWireCube(center, size);
                    Gizmos.DrawSphere(center, 0.05f);
                    break;
                default:
                case HitType.Sphere:
                    Gizmos.DrawWireSphere(center, radius);
                    Gizmos.DrawSphere(center, 0.05f);
                    break;
                case HitType.Capsule:
                    Vector3 forward = (end - start).normalized;
                    float mag = (end - start).magnitude;
                    Gizmos.matrix *= Matrix4x4.TRS(start, Quaternion.LookRotation(forward, Vector3.up), new Vector3(1, 1, 1));
                    Gizmos.DrawSphere(Vector3.zero, 0.05f);
                    Gizmos.DrawLine(Vector3.zero, Vector3.forward * mag);
                    Gizmos.DrawSphere(Vector3.forward * mag, 0.05f);

                    Gizmos.DrawWireSphere(Vector3.zero, radius);
                    Gizmos.DrawWireSphere(Vector3.forward * mag, radius);

                    Gizmos.DrawLine(new Vector3(radius, 0, 0), Vector3.forward * mag + new Vector3(radius, 0, 0));
                    Gizmos.DrawLine(new Vector3(-radius, 0, 0), Vector3.forward * mag + new Vector3(-radius, 0, 0));
                    Gizmos.DrawLine(new Vector3(0, radius, 0), Vector3.forward * mag + new Vector3(0, radius, 0));
                    Gizmos.DrawLine(new Vector3(0, -radius, 0), Vector3.forward * mag + new Vector3(0, -radius, 0));
                    Gizmos.matrix *= Matrix4x4.TRS(start, Quaternion.LookRotation(forward, Vector3.up), new Vector3(1, 1, 1)).inverse;
                    break;
            }

            Gizmos.matrix = Matrix4x4.identity;
        }
    }
}
