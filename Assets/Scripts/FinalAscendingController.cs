using UnityEngine;
using System;
using System.Collections;

public class FinalAscendingController : MoveController
{
    //hyperbolic curve
    public float _param;
    public Vector3 _forwardAxis;
    public Vector3 _up;

    public Vector3 RightAxis
    { get { return (Vector3.Cross(_up, _forwardAxis).normalized); } }

    public Vector3 UpwardAxis
    { get { return Vector3.Cross(_forwardAxis, RightAxis); } }

    public Vector3 _initPos;
    public float _startDist;
    private float _currDist;
    public float _endDist;

    private float _speed;
    public Vector3 _offset;

    public float Speed
    {
        get { return _speed; }
        set
        { _speed = value; }
    }

    public void Init(out Vector3 initPos, out Quaternion initRot)
    {
        initPos = _initPos;
        initRot = Quaternion.LookRotation(_forwardAxis, UpwardAxis);
        _currDist = _startDist;
    }

    public bool Step(out Vector3 nextPos, out Quaternion nextRot)
    {
        float originOffset = (_param / _startDist);
        _currDist -= _speed / Mathf.Sqrt(1f + _param * _param / (_currDist * _currDist * _currDist * _currDist)) * Time.deltaTime;
        nextPos = _initPos + _forwardAxis * (_startDist - _currDist) + UpwardAxis * (_param / _currDist - originOffset);
        float slope = -_param / (_currDist * _currDist);
        float rotAngle = - Mathf.Atan(slope);
        Vector3 fore = Mathf.Cos(rotAngle) * _forwardAxis + Mathf.Sin(rotAngle) * UpwardAxis;
        Vector3 up = Vector3.Cross(fore, RightAxis);
        nextRot = Quaternion.LookRotation(fore, up);

        return _currDist > _endDist;
    }

}
