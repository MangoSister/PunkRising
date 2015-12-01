using UnityEngine;
using System;
using System.Collections;

public class PathFollower : MoveController
{
    public int _currNodeIdx;
    public float _currTngOffset;
    private float _speed;
    public Vector3 _offset;
    public PathManager _path;

    public event EventHandler<PathFollowerEventArg> _FollowerMove;

    public float Speed
    {
        get { return _speed; }
        set
        { _speed = value; }
    }

    public PathFollower(PathManager path)
    {
        _path = path;
        _offset = Vector3.zero;
    }

    public void Init(out Vector3 initPos, out Quaternion initRot)
    {
        //_currNodeIdx = 0;
        //_currTngOffset = 0f;
        initPos = Bezier.GetPoint3d(_path.Nodes[_currNodeIdx]._ctrlPts, _currTngOffset);
        initRot = Bezier.GetOrientation3d(_path.Nodes[_currNodeIdx]._ctrlPts, _currTngOffset, _path.transform.up);
    }

    public bool Step(out Vector3 nextPos, out Quaternion nextRot)
    {
        if (_currNodeIdx < 0 || _currNodeIdx >= _path.NodeCount)
        {
            nextPos = Vector3.zero;
            nextRot = Quaternion.identity;
            return false;
        }
        // s: nodeOffset
        // ds / dt = speed / sqrt((dx/ds)^2 + (dy/ds)^2 + (dz/ds)^2)
        PathNode currNode = _path.Nodes[_currNodeIdx];
        //Vector3[] sCoef = currNode.ParamCoef;
        //Vector3[] dpdsCoef = new Vector3[3] { 3 * sCoef[0], 2 * sCoef[1], sCoef[2] };
        //Vector3 dpds = currNode.ComputeDerivativePosParam(agent._nodeOffset);
        Vector3 dpds = Bezier.GetFirstDerivative3d(currNode._ctrlPts, _currTngOffset);
        float dsdt = _speed / dpds.magnitude;
        _currTngOffset += dsdt * Time.deltaTime;
        float offsetBeforeClamp = _currTngOffset;
        _currTngOffset = Mathf.Clamp01(_currTngOffset);
        nextPos = Bezier.GetPoint3d(currNode._ctrlPts, _currTngOffset) + _offset;
        nextRot = Bezier.GetOrientation3d(currNode._ctrlPts, _currTngOffset, _path.transform.up);

        if (offsetBeforeClamp >= 1f)
        {
            _currNodeIdx++;
            _currTngOffset = 0f;
        }
        else if (offsetBeforeClamp <= 0f)
        {
            _currNodeIdx--;
            _currTngOffset = 1f;
        }

        if (_FollowerMove != null)
            _FollowerMove(this, new PathFollowerEventArg(_currNodeIdx, _currTngOffset));

        return true;
    }
}

public class PathFollowerEventArg : EventArgs
{
    public int _nodeIdx;
    public float _tngOffset;

    public PathFollowerEventArg(int nodeIdx, float tngOffset)
    {
        _nodeIdx = nodeIdx;
        _tngOffset = tngOffset;
    }
}