using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

//cubic bezier curve path node
public class PathNode : MonoBehaviour
{
    public static readonly int CtrlPtNum = 4;
    public List<Vector3> _ctrlPts;
    public Vector3 this[int index]
    {
        get
        {
            Debug.Assert(index >= 0 && index <= 3);
            return _ctrlPts[index];
        }
        set
        {
            Debug.Assert(index >= 0 && index <= 3);
            _ctrlPts[index] = value;
        }
    }

    public float _eventTriggerPos;
    public event EventHandler<PathNodeEventArg> _nodeEvent;

    //F(s) = (1-s)^3 * P_0 + 3s(1-s)^2 * P_1 + 3s^2(1-s) * P_2 + s^3 * P_3
    //F(s) = (-P_0 + 3*P_1 - 3*P_2 + P_3) s^3 +
    //      (3*P_0 - 6*P_1 + 3*P_2) s^2 +
    //      (-3 * P_0 + 3 * P_1) s +
    //      P_0
    public Vector3[] ParamCoef
    {
        get
        {
            Vector3[] output = new Vector3[4];
            output[0] = -_ctrlPts[0] + 3f * _ctrlPts[1] - 3f * _ctrlPts[2] + _ctrlPts[3];
            output[1] = 3f * _ctrlPts[0] - 6f * _ctrlPts[1] + 3f * _ctrlPts[2];
            output[2] = -3f * _ctrlPts[0] + 3f * _ctrlPts[1];
            output[3] = _ctrlPts[0];
            return output;
        }
    }

    public void Reset(Vector3 start, Vector3 dir)
    {
        _ctrlPts = new List<Vector3>();
        dir.Normalize();
        for (int i = 0; i < CtrlPtNum; i++)
        {
            _ctrlPts.Add(start + dir * i * 2f);
        }
    }

    //public Vector3 ComputeDerivativePosParam(float s)
    //{
    //    Vector3[] sCoef = ParamCoef;
    //    Vector3[] dpdsCoef = new Vector3[3] { 3 * sCoef[0], 2 * sCoef[1], sCoef[2] };
    //    Vector3 dpds = new Vector3(
    //        dpdsCoef[0].x * s * s +
    //        dpdsCoef[1].x * s +
    //        dpdsCoef[2].x,

    //        dpdsCoef[0].y * s * s +
    //        dpdsCoef[1].y * s +
    //        dpdsCoef[2].y,

    //        dpdsCoef[0].z * s * s +
    //        dpdsCoef[1].z * s +
    //        dpdsCoef[2].z);
    //    return dpds;
    //}

    public List<Vector3> ComputeSamples(float step)
    {
        List<Vector3> samples = new List<Vector3>();
        float s = 0f;
        while (s < 1f)
        {
            Vector3 dpds = Bezier.GetFirstDerivative3d(_ctrlPts, s);
            float ds = step / dpds.magnitude;
            s += ds;
            s = Mathf.Clamp01(s);
            samples.Add(Bezier.GetPoint3d(_ctrlPts, s));
        }
        return samples;
    }

    public List<OrientedPoint> ComputeOrientedSamples(float step)
    {
        List<OrientedPoint> samples = new List<OrientedPoint>();
        float s = 0f;
        while (s < 1f)
        {
            Vector3 dpds = Bezier.GetFirstDerivative3d(_ctrlPts, s);
            float ds = step / dpds.magnitude;
            s += ds;
            s = Mathf.Clamp01(s);
            samples.Add(new OrientedPoint
                (Bezier.GetPoint3d(_ctrlPts, s), Bezier.GetOrientation3d(_ctrlPts, s, transform.up)));
        }
        return samples;
    }
}

public class PathNodeEventArg : EventArgs
{
    // public float 
}

public struct OrientedPoint
{
    public Vector3 _pos;
    public Quaternion _rot;

    public OrientedPoint(Vector3 pos, Quaternion rot)
    {
        _pos = pos;
        _rot = rot;
    }

    public Vector3 LocalToWorld(Vector3 point)
    {
        return _pos + _rot * point;
    }

    public Vector3 WorldToLocal(Vector3 point)
    {
        return Quaternion.Inverse(_rot) * (point - _pos);
    }

    public Vector3 LocalToWorldDirection(Vector3 dir)
    {
        return _rot * dir;
    }
};