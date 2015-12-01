using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

public abstract class PathEvent : MonoBehaviour
{
    public bool _start;
    public bool _end;
    public int _triggerNodeIdx;
    public float _triggerTngOffset;

    public int _startNodeIdx;
    public float _startTngOffset;
    public float _startBinrmOffset;

    public abstract void StartEvent();
    public abstract void EndEvent();

    protected void Start()
    {
        _start = false;
        _end = false;
    }

    private void OnDrawGizmos()
    {
        PathManager path = LevelController.Instance._path;
        if (path == null)
            return;

        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(Bezier.GetPoint3d(path.Nodes[_triggerNodeIdx]._ctrlPts, _triggerTngOffset), 0.3f);
        Gizmos.color = Color.magenta;
        Gizmos.DrawWireSphere(transform.position, 0.3f);
    }
}
