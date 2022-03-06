using System.Collections;
using UnityEngine;

public static class Extentions
{ 
    public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
    {
        return (to - from).normalized;
    }
     
    public static void SetEnabled(this MonoBehaviour comp, bool status)
    {
        comp.enabled = status;
    }

    public static Vector2 OnUnitSquare(Vector2 sphere)
    {
       if(sphere == Vector2.zero)
           sphere = Random.insideUnitCircle;

       sphere = sphere.normalized;

       if (1.0f - Mathf.Abs(sphere.x) < 1.0f - Mathf.Abs(sphere.y))
       {
           //x side
           sphere.x = (sphere.x < 0) ? -1f : 1f;
       }
       else
       {
           //y side
           sphere.y = (sphere.y < 0) ? -1f : 1f;
       }

       return sphere;
    }
    public static void GizmosDrawCircle(Vector3 point, float radius)
    {
        Gizmos.matrix = Matrix4x4.TRS(point, Quaternion.identity, new Vector3(1f, 0f, 1f));

        Gizmos.DrawWireSphere(Vector3.zero, radius);

        Gizmos.matrix = Matrix4x4.identity;
    }

    public static void GizmosDrawSquare(Vector3 point, Quaternion rotation, Vector2 size)
    {
        Gizmos.matrix = Matrix4x4.TRS(point, rotation, new Vector3(1f, 0f, 1f));

        Gizmos.DrawWireCube(Vector3.zero, new Vector3(size.x, 0, size.y));

        Gizmos.matrix = Matrix4x4.identity;
    }
}