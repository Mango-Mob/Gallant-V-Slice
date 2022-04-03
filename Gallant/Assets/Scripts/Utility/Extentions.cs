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
    public static Vector3 MidPoint(Vector3 A, Vector3 B)
    {
        return new Vector3( (A.x + B.x) / 2f, (A.y + B.y) / 2f, (A.z + B.z) / 2f);
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
    public static bool LineIntersection(Vector2 p1, Vector2 p2, Vector2 p3, Vector2 p4, ref Vector2 intersection)
    {
        float Ax, Bx, Cx, Ay, By, Cy, d, e, f, num;
        float x1lo, x1hi, y1lo, y1hi;

        Ax = p2.x - p1.x;
        Bx = p3.x - p4.x;

        // X bound box test/
        if (Ax < 0)
        {
            x1lo = p2.x; x1hi = p1.x;
        }
        else
        {
            x1hi = p2.x; x1lo = p1.x;
        }

        if (Bx > 0)
        {
            if (x1hi < p4.x || p3.x < x1lo) return false;
        }
        else
        {
            if (x1hi < p3.x || p4.x < x1lo) return false;
        }

        Ay = p2.y - p1.y;
        By = p3.y - p4.y;

        // Y bound box test//
        if (Ay < 0)
        {
            y1lo = p2.y; y1hi = p1.y;
        }
        else
        {
            y1hi = p2.y; y1lo = p1.y;
        }

        if (By > 0)
        {
            if (y1hi < p4.y || p3.y < y1lo) return false;
        }
        else
        {
            if (y1hi < p3.y || p4.y < y1lo) return false;
        }

        Cx = p1.x - p3.x;
        Cy = p1.y - p3.y;
        d = By * Cx - Bx * Cy;  // alpha numerator//
        f = Ay * Bx - Ax * By;  // both denominator//

        // alpha tests//
        if (f > 0)
        {
            if (d < 0 || d > f) return false;
        }
        else
        {
            if (d > 0 || d < f) return false;
        }

        e = Ax * Cy - Ay * Cx;  // beta numerator//

        // beta tests //
        if (f > 0)
        {
            if (e < 0 || e > f) return false;
        }
        else
        {
            if (e > 0 || e < f) return false;
        }

        // check if they are parallel
        if (f == 0) return false;

        // compute intersection coordinates //
        num = d * Ax; // numerator //
        intersection.x = p1.x + num / f;

        num = d * Ay;
        intersection.y = p1.y + num / f;

        return true;
    }

    private static bool same_sign(float a, float b)
    {
        return ((a * b) >= 0f);
    }
}