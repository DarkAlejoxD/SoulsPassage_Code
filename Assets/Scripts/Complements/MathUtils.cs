using UnityEngine;

public class MathUtils
{
    public static int GetClosestPoint(Vector3 from, Vector3[] points)
    {
        if (points.Length <= 0) return -1;

        float smallestDistance = 9999.0f;
        int closestIndex = -1;
        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.Distance(from, points[i]);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    public static int GetClosestPoint(Vector3 from, Transform[] points)
    {
        if (points.Length <= 0) return -1;

        float smallestDistance = 9999.0f;
        int closestIndex = -1;
        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.Distance(from, points[i].position);
            if (distance < smallestDistance)
            {
                smallestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }
}
