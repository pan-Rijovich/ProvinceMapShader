using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BSpline
{
    public static List<Vector3> CalculateBSpline(List<Vector3> controlPoints, int numPoints)
    {
        List<Vector3> splinePoints = new List<Vector3>();

        controlPoints.Insert(0, controlPoints[0]);
        controlPoints.Insert(0, controlPoints[0]);
        controlPoints.Add(controlPoints.Last());
        controlPoints.Add(controlPoints.Last());

        int n = controlPoints.Count - 1;
        int degree = 5; // Кубический сплайн
        float[] knots = CreateKnotVector(n, degree);

        for (int i = 0; i <= numPoints; i++)
        {
            float t = (float)i / (float)numPoints;
            Vector3 point = DeBoor(degree, knots, controlPoints, t);
            splinePoints.Add(point);
        }

        return splinePoints;
    }

    private static Vector3 DeBoor(int degree, float[] knots, List<Vector3> controlPoints, float t)
    {
        int n = controlPoints.Count - 1;
        int k = FindSpan(knots, n, degree, t);
        Vector3[] d = new Vector3[degree + 1];

        // Инициализация
        for (int i = 0; i <= degree; i++)
            d[i] = controlPoints[k - degree + i];

        // Алгоритм Де Буара для вычисления точки сплайна
        for (int r = 1; r <= degree; r++)
        {
            for (int j = degree; j >= r; j--)
            {
                float alpha = (t - knots[k - degree + j]) / (knots[k + 1 + j - r] - knots[k - degree + j]);
                d[j] = (1 - alpha) * d[j - 1] + alpha * d[j];
            }
        }

        return d[degree];
    }

    private static int FindSpan(float[] knots, int n, int degree, float t)
    {
        if (t >= knots[n + 1]) return n;
        if (t <= knots[degree]) return degree;

        int low = degree;
        int high = n + 1;
        int mid = (low + high) / 2;

        while (t < knots[mid] || t >= knots[mid + 1])
        {
            if (t < knots[mid])
                high = mid;
            else
                low = mid;
            mid = (low + high) / 2;
        }

        return mid;
    }

    private static float[] CreateKnotVector(int n, int degree)
    {
        int m = n + degree + 1;
        float[] knots = new float[m + 1];

        for (int i = 0; i <= degree; i++)
            knots[i] = 0;

        for (int i = degree + 1; i <= n; i++)
            knots[i] = (float)(i - degree) / (float)(n - degree + 1);

        for (int i = n + 1; i <= m; i++)
            knots[i] = 1;

        return knots;
    }
}