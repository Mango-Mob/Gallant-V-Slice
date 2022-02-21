using System.Collections;
using UnityEngine;

 public static class Extentions
 { 
     public static Vector3 DirectionTo(this Vector3 from, Vector3 to)
     {
         return (to - from).normalized;
     }
 }