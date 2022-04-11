using UnityEngine;
using System;

public class MathParabola
{

    public static Vector3 Parabola(Vector3 start, Vector3 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector3.Lerp(start, end, t);

        return new Vector3(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t), mid.z);
    }

    public static Vector2 Parabola(Vector2 start, Vector2 end, float height, float t)
    {
        Func<float, float> f = x => -4 * height * x * x + 4 * height * x;

        var mid = Vector2.Lerp(start, end, t);

        return new Vector2(mid.x, f(t) + Mathf.Lerp(start.y, end.y, t));
    }
    public static float ParabolaDistance(Vector2 start, Vector2 end, float height, uint steps = 2)
    {
        if (steps < 2)
            steps = 2;

        float distance = 0;
        Vector3 prevPoint = start;
        for (int i = 1; i < steps; i++)
        {
            Vector3 nextPoint = Parabola(start, end, height, i * (1.0f / (steps - 1)));
            distance += Vector3.Distance(prevPoint, nextPoint);
            prevPoint = nextPoint;
        }

        return distance;
    }
}