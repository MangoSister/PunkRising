using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

#region Bezier Math Class
internal static class Bezier
{
    //de Casteljau's algorithm
    internal static Vector3 GetPoint3d(List<Vector3> ctrlPts, float t)
    {
        t = Mathf.Clamp01(t);
        int iterNum = ctrlPts.Count;
        Vector3[] iterArrayOld, iterArrayNew;
        iterArrayOld = ctrlPts.ToArray();

        Vector3 pos;
        for (int level = 1; level < iterNum; level++)
        {
            iterArrayNew = new Vector3[iterNum - level];
            for (int idx = 0; idx < iterNum - level; idx++)
                iterArrayNew[idx] = (1f - t) * iterArrayOld[idx] + t * iterArrayOld[idx + 1];
            iterArrayOld = iterArrayNew;
        }
        pos = iterArrayOld[0];

        //			if (world)
        //				pos = transform.TransformPoint(pos);

        return pos;
    }

    internal static Vector2 GetPoint2d(List<Vector2> ctrlPts, float t)
    {
        t = Mathf.Clamp01(t);
        int iterNum = ctrlPts.Count;
        Vector2[] iterArrayOld, iterArrayNew;
        iterArrayOld = ctrlPts.ToArray();

        Vector2 pos;
        for (int level = 1; level < iterNum; level++)
        {
            iterArrayNew = new Vector2[iterNum - level];
            for (int idx = 0; idx < iterNum - level; idx++)
                iterArrayNew[idx] = (1f - t) * iterArrayOld[idx] + t * iterArrayOld[idx + 1];
            iterArrayOld = iterArrayNew;
        }
        pos = iterArrayOld[0];

        //			if (world)
        //				pos = transform.TransformPoint(pos);

        return pos;
    }

    internal static Vector3 GetFirstDerivative3d(List<Vector3> ctrlPts, float t)
    {
        t = Mathf.Clamp01(t);
        int iterNum = ctrlPts.Count;
        Vector3[] iterArrayOld, iterArrayNew;
        iterArrayOld = ctrlPts.ToArray();

        Vector3 derivative;
        //initialize derivative
        if (iterNum >= 2)
            derivative = ctrlPts[iterNum - 1] - ctrlPts[0];
        else derivative = Vector3.up; // meaningless

        for (int level = 1; level < iterNum - 1; level++)
        {
            iterArrayNew = new Vector3[iterNum - level];
            for (int idx = 0; idx < iterNum - level; idx++)
                iterArrayNew[idx] = (1f - t) * iterArrayOld[idx] + t * iterArrayOld[idx + 1];
            iterArrayOld = iterArrayNew;
        }

        derivative = (iterNum - 1) * (iterArrayOld[1] - iterArrayOld[0]);
        //			if (world)
        //				derivative = (transform.TransformPoint(derivative) - transform.position);

        return derivative;
    }

    internal static Vector2 GetFirstDerivative2d(List<Vector2> ctrlPts, float t)
    {
        t = Mathf.Clamp01(t);
        int iterNum = ctrlPts.Count;
        Vector2[] iterArrayOld, iterArrayNew;
        iterArrayOld = ctrlPts.ToArray();

        Vector2 derivative;
        //initialize derivative
        if (iterNum >= 2)
            derivative = ctrlPts[iterNum - 1] - ctrlPts[0];
        else derivative = Vector2.up; // meaningless

        for (int level = 1; level < iterNum - 1; level++)
        {
            iterArrayNew = new Vector2[iterNum - level];
            for (int idx = 0; idx < iterNum - level; idx++)
                iterArrayNew[idx] = (1f - t) * iterArrayOld[idx] + t * iterArrayOld[idx + 1];
            iterArrayOld = iterArrayNew;
        }

        derivative = (iterNum - 1) * (iterArrayOld[1] - iterArrayOld[0]);
        //			if (world)
        //				derivative = (transform.TransformPoint(derivative) - transform.position);

        return derivative;
    }

    internal static Vector3 GetTangent3d(List<Vector3> ctrlPts, float t)
    {
        return GetFirstDerivative3d(ctrlPts, t).normalized;
    }

    internal static Vector2 GetTangent2d(List<Vector2> ctrlPts, float t)
    {
        return GetFirstDerivative2d(ctrlPts, t).normalized;
    }

    internal static Vector3 GetNormal3d(List<Vector3> ctrlPts, float t, Vector3 up)
    {
        Vector3 tangent = GetTangent3d(ctrlPts, t);
        Vector3 binormal = Vector3.Cross(up, tangent).normalized;
        return Vector3.Cross(tangent, binormal);
    }

    internal static Vector3 GetBinormal3d(List<Vector3> ctrlPts, float t, Vector3 up)
    {
        Vector3 tangent = GetTangent3d(ctrlPts, t);
        Vector3 binormal = Vector3.Cross(up, tangent).normalized;
        return binormal;
    }

    internal static void GetTangentSpaceAxes3d(List<Vector3> ctrlPts, float t, Vector3 up,
        out Vector3 tangent, out Vector3 binormal, out Vector3 normal)
    {
        tangent = GetTangent3d(ctrlPts, t);
        binormal = Vector3.Cross(up, tangent).normalized;
        normal = Vector3.Cross(tangent, binormal);
    }

    internal static Vector2 GetNormal2d(List<Vector2> ctrlPts, float t)
    {
        Vector2 tangent = GetTangent2d(ctrlPts, t);
        return new Vector2(-tangent.y, tangent.x);
    }

    internal static Quaternion GetOrientation3d(List<Vector3> ctrlPts, float t, Vector3 up)
    {
        Vector3 tangent, binormal, normal;
        GetTangentSpaceAxes3d(ctrlPts, t, up, out tangent, out binormal, out normal);
        return Quaternion.LookRotation(tangent, normal);
    }

    internal static Quaternion GetOrientation2d(List<Vector2> ctrlPts, float t)
    {
        Vector3 tangent = GetTangent2d(ctrlPts, t);
        Vector3 normal = GetNormal2d(ctrlPts, t);
        return Quaternion.LookRotation(tangent, normal);
    }

    //http://www.cs.mtu.edu/~shene/COURSES/cs3621/NOTES/spline/Bezier/bezier-der.html
    //C(t)' = n * [C1(t) - C2(t)]
    internal static Vector3 GetSecondDerivative3d(List<Vector3> ctrlPts, float t)
    {
        t = Mathf.Clamp01(t);
        List<Vector3> c1 = new List<Vector3>();
        List<Vector3> c2 = new List<Vector3>();
        for (int i = 0; i < ctrlPts.Count - 1; i++)
        {
            c1.Add(ctrlPts[i]);
            c2.Add(ctrlPts[i + 1]);
        }
        return (ctrlPts.Count - 1) * (GetFirstDerivative3d(c1, t) - GetFirstDerivative3d(c2, t));
    }

    internal static Vector2 GetSecondDerivative2d(List<Vector2> ctrlPts, float t)
    {
        t = Mathf.Clamp01(t);
        List<Vector2> c1 = new List<Vector2>();
        List<Vector2> c2 = new List<Vector2>();
        for (int i = 0; i < ctrlPts.Count - 1; i++)
        {
            c1.Add(ctrlPts[i]);
            c2.Add(ctrlPts[i + 1]);
        }
        return (ctrlPts.Count - 1) * (GetFirstDerivative2d(c2, t) - GetFirstDerivative2d(c1, t)); //sign
    }
}
#endregion
