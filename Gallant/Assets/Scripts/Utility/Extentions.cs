using System.Collections;
using UnityEngine;

 public static class Extentions
 { 
     public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
     {
         return (to - from).normalized;
     }

    public static Vector2 OnUnitSquare(Vector2 sphere)
    {
        if(sphere == Vector2.zero)
            sphere = Random.insideUnitCircle;

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
 }