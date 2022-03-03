using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;


public class Spawn : MonoBehaviour
{
    public float radius;
    public uint sections;
    public float sampleRadius;
    public float overlapRadius;
    public bool isCircle;

    public struct SpawnData
    {
        public Vector3 start;
        public Vector3 end;
        public Vector3 navPoint;
    }

    public List<SpawnData> m_data;

    public async void Start()
    {
        await CreateSpawns();
    }

    public Task CreateSpawns()
    {
        m_data = new List<SpawnData>();
        float step = Mathf.Deg2Rad * 360f / sections;
        float curr = 0;

        for (int i = 0; i < sections; i++)
        {
            Vector3 direct = Vector3.zero;

            if (isCircle)
            {
                direct = (Quaternion.Euler(0, Mathf.Rad2Deg * curr, 0) * transform.forward);
            }
            else
            {
                float m = Mathf.Max(Mathf.Abs(Mathf.Cos(curr)), Mathf.Abs(Mathf.Sin(curr)));
                direct.x = (1.0f / m) * Mathf.Cos(curr);    // x = radius/MAX(|cos(deg)|, |sin(deg)|) * cos(deg)
                direct.z = (1.0f / m) * Mathf.Sin(curr);    // z = radius/MAX(|cos(deg)|, |sin(deg)|) * sin(deg)
            }

            Vector3 point = transform.position + direct * radius;
            SpawnData data;
            if (EvaluatePoint(point, out data))
            {
                m_data.Add(data);
            }
            curr += step;
        }
        return Task.CompletedTask;
    }

    private bool EvaluatePoint(Vector3 point, out SpawnData data)
    {
        data = new SpawnData();
        RaycastHit hit;
        if (Physics.OverlapSphere(point, overlapRadius).Length == 0)
        {
            if (Physics.Raycast(point, Vector3.down, out hit, transform.position.y + 0.5f))
            {
                if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                {
                    if (sampleRadius > 0)
                    {
                        NavMeshHit navHit;
                        if (NavMesh.SamplePosition(point, out navHit, sampleRadius, NavMesh.AllAreas))
                        {
                            data.start = hit.point;
                            data.end = point;
                            data.navPoint = navHit.position;
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    private void OnCircle()
    {
        Gizmos.color = Color.white;
        DrawCircle(transform.position, radius);

        if (sections > 2)
        {
            float step = 360f / sections;
            float curr = 0;

            for (int i = 0; i < sections; i++)
            {
                Gizmos.color = Color.red;
                Vector3 direct = (Quaternion.Euler(0, curr, 0) * transform.forward);
                Vector3 point = transform.position + direct * radius;

                RaycastHit hit;
                if (Physics.OverlapSphere(point, overlapRadius).Length == 0)
                {
                    if (Physics.Raycast(point, Vector3.down, out hit, transform.position.y + 0.5f))
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                        {
                            Gizmos.color = Color.white;
                            DrawCircle(point, sampleRadius);
                            Gizmos.color = Color.yellow;

                            if (sampleRadius > 0)
                            {
                                NavMeshHit navHit;
                                if (NavMesh.SamplePosition(point, out navHit, sampleRadius, NavMesh.AllAreas))
                                {
                                    Gizmos.color = Color.green;
                                    Gizmos.DrawSphere(navHit.position, overlapRadius);
                                }
                            }
                        }
                        Gizmos.DrawSphere(hit.point, overlapRadius);
                    }
                }
                Gizmos.DrawSphere(point, overlapRadius);
                Gizmos.DrawLine(point, point + Vector3.down * transform.position.y);

                curr += step;
            }
        }
    }

    public void OnSquare()
    {
        Gizmos.color = Color.white;
        DrawSquare(transform.position, new Vector2(radius, radius) * 2);

        if(sections > 2)
        {
            float step = Mathf.Deg2Rad * (360f / sections);
            float curr = 0;

            for (int i = 0; i < sections; i++)
            {
                Gizmos.color = Color.red;

                Vector3 direct = GetPointOnSquare(curr);
                Vector3 point = direct * radius + transform.position; 

                RaycastHit hit;
                if (Physics.OverlapSphere(point, overlapRadius).Length == 0)
                {
                    if (Physics.Raycast(point, Vector3.down, out hit, transform.position.y + 0.5f))
                    {
                        if (hit.collider.gameObject.layer == LayerMask.NameToLayer("Water"))
                        {
                            Gizmos.color = Color.white;
                            DrawCircle(point, sampleRadius);
                            Gizmos.color = Color.yellow;
                
                            if (sampleRadius > 0)
                            {
                                NavMeshHit navHit;
                                if (NavMesh.SamplePosition(point, out navHit, sampleRadius, NavMesh.AllAreas))
                                {
                                    Gizmos.color = Color.green;
                                    Gizmos.DrawSphere(navHit.position, overlapRadius);
                                }
                            }
                        }
                        Gizmos.DrawSphere(hit.point, overlapRadius);
                    }
                }
                Gizmos.DrawSphere(point, overlapRadius);
                Gizmos.DrawLine(point, point + Vector3.down * transform.position.y);

                curr += step;
            }
        }
    }

    public Vector3 GetPointOnSquare(float angle)
    {
        float cos = Mathf.Cos(angle);
        float sin = Mathf.Sin(angle);
        float denom = Mathf.Max(Mathf.Abs(cos), Mathf.Abs(sin));

        Vector3 direct;
        direct.x = 1.0f/denom * cos;
        direct.y = 0f;
        direct.z = 1.0f/denom * sin;

        return direct;
    }

    private void OnDrawGizmos()
    {
        if (sections > 2)
        {
            float step = Mathf.Deg2Rad * 360f / sections;

        }
        if (!Application.isPlaying)
        {
            if (isCircle)
                OnCircle();
            else
                OnSquare();
        }
        else if (m_data != null)
        {
            foreach (var data in m_data)
            {
                Gizmos.DrawSphere(data.start, overlapRadius);
                Gizmos.DrawSphere(data.end, overlapRadius);
                Gizmos.DrawSphere(data.navPoint, overlapRadius);
            }
        }
    }

    private void DrawSquare(Vector3 point, Vector2 size)
    {
        Gizmos.matrix = Matrix4x4.TRS(point, transform.rotation, new Vector3(1f, 0f, 1f));

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, 0, size.y));

        Gizmos.matrix = Matrix4x4.identity;
    }

    private void DrawCircle(Vector3 point, float radius)
    {
        Gizmos.matrix = Matrix4x4.TRS(point, Quaternion.identity, new Vector3(1f, 0f, 1f));

        Gizmos.DrawWireSphere(Vector3.zero, radius);

        Gizmos.matrix = Matrix4x4.identity;
    }
}
