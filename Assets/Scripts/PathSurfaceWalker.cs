using UnityEngine;
using System;
using System.Collections;

public class PathSurfaceWalker : MonoBehaviour
{
    public float _truncateDist = 0.1f;
    public float _speed;
    private PathManager _path;

    private int _currNodeIdx;
    private float _currTngOffset;
    private float _currNrmOffset;

    public int CurrNodeIdx {
        get { return _currNodeIdx; }
    }

    public float CurrTngOffset
    {
        get { return _currTngOffset; }
    }

    public float CurrNrmOffset
    {
        get
        { return _currNrmOffset; }}

    public event EventHandler WalkFinish;

    public static PathSurfaceWalker AttachPathSurfaceWalker(GameObject obj, PathManager path, int nodeIdx,
        float tngOffset, float binrmOffset, float nrmOffset, float speed)
    {
        if (obj.GetComponent<PathSurfaceWalker>() != null)
            throw new UnityException("object already attached");
        var walker = obj.AddComponent<PathSurfaceWalker>();
        walker._path = path;
        walker._currNodeIdx = nodeIdx;
        walker._currTngOffset = tngOffset;
        walker._currNrmOffset = nrmOffset;
        walker.transform.position = Bezier.GetPoint3d(path.Nodes[nodeIdx]._ctrlPts, tngOffset) +
                                    Bezier.GetBinormal3d(path.Nodes[nodeIdx]._ctrlPts, tngOffset, path.transform.up) * binrmOffset +
                                    Bezier.GetNormal3d(path.Nodes[nodeIdx]._ctrlPts, tngOffset, path.transform.up) * nrmOffset;

        walker.transform.rotation = Bezier.GetOrientation3d(path.Nodes[nodeIdx]._ctrlPts, tngOffset, path.transform.up);
        walker._speed = speed;
        return walker;
    }

    public void MoveOnPathSurface(int targetNodeIdx,
        float targetTngOffset, float targetBinrmOffset)
    {
        StartCoroutine(MoveOnPathSurfaceCoroutine(targetNodeIdx, targetTngOffset, targetBinrmOffset));
    }

    private IEnumerator MoveOnPathSurfaceCoroutine(int targetNodeIdx,
        float targetTngOffset, float targetBinrmOffset)
    {
        Vector3 target =    Bezier.GetPoint3d(_path.Nodes[targetNodeIdx]._ctrlPts, targetTngOffset) +
                            Bezier.GetBinormal3d(_path.Nodes[targetNodeIdx]._ctrlPts, targetTngOffset, _path.transform.up) * targetBinrmOffset +
                            Bezier.GetNormal3d(_path.Nodes[targetNodeIdx]._ctrlPts, targetTngOffset, _path.transform.up) * _currNrmOffset;
        Vector3 dir = (target - transform.position).normalized;
        float lastDist = Vector3.Distance(transform.position, target);
        while (lastDist > _truncateDist)
        {
            PathNode currNode = _path.Nodes[_currNodeIdx];
            Vector3 currDerivative = Bezier.GetFirstDerivative3d(currNode._ctrlPts, _currTngOffset);
            Vector3 currTng, currBinrm, currNrm;
            Bezier.GetTangentSpaceAxes3d(currNode._ctrlPts, _currTngOffset, _path.transform.up,
                out currTng, out currBinrm, out currNrm);

            //Debug.Log(currBinrm);

            float tngComp = Vector3.Dot(dir, currTng);
            float binrmComp = Vector3.Dot(dir, currBinrm);
            float compMag = Mathf.Sqrt(tngComp * tngComp + binrmComp * binrmComp);
            tngComp /= compMag;
            binrmComp /= compMag;

            _currTngOffset += _speed * Time.deltaTime / currDerivative.magnitude;//ERROR
            transform.position += currTng * tngComp * _speed * Time.deltaTime;
            transform.position += currBinrm * binrmComp * _speed * Time.deltaTime;
            transform.rotation = Quaternion.LookRotation(currTng * tngComp + currBinrm * binrmComp, currNrm);

            //Debug.Log(_currTngOffset);
            if (_currTngOffset >= 1f && _currNodeIdx < _path.NodeCount - 1)
            {
                _currNodeIdx++;
                _currTngOffset = 0f;
            }
            else if (_currTngOffset >= 1f && _currNodeIdx >= _path.NodeCount - 1)
            {
                _currTngOffset = 1f;
                //Debug.Log("111");
                break;
            }
            else if (_currTngOffset <= 0f && _currNodeIdx > 0)
            {
                _currNodeIdx--;
                _currTngOffset = 1f;
            }
            else if (_currTngOffset <= 0f && _currNodeIdx <= 0)
            {
                _currTngOffset = 0f;
                //Debug.Log("222");
                break;
            }

            float currDist = Vector3.Distance(transform.position, target);
            if (lastDist < currDist)
            {
                break;
            }
            lastDist = currDist;

            yield return null;
        }

        //Debug.Log("!!!");
        //quick fix
        Vector3 unfixedPos = transform.position;
        float fixPeriod = Vector3.Distance(transform.position, target) / _speed;
        float startTime = Time.time;
        float currTime = Time.time;
        while(currTime - startTime < fixPeriod)
        {
            transform.position = Vector3.Lerp(unfixedPos, target, Mathf.Clamp01((currTime - startTime) / fixPeriod));
            currTime = Time.time;
            yield return null;
        }

        _currNodeIdx = targetNodeIdx;
        _currTngOffset = targetTngOffset;
        transform.position = target;
        if (WalkFinish != null)
            WalkFinish(gameObject, null);
        Destroy(this);
    }
}
