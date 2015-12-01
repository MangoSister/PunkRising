using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(PathManager))]
public class PathRenderer : MonoBehaviour
{
    public float _sampleStep = 0.1f;
    private PathManager path { get { return GetComponent<PathManager>(); } }
    [SerializeField]
    private List<ListVec3Wrapper> _samples;
    public bool enableLineRenderer
    {
        get { return lineRenderer.enabled; }
        set { lineRenderer.enabled = value; }
    }

    private LineRenderer lineRenderer
    {
        get
        {
            LineRenderer lr = GetComponent<LineRenderer>();
            return lr != null ? lr : gameObject.AddComponent<LineRenderer>();
        }
    }

    private void OnNodeModify(object sender, EventArgs e)
    {
        NodeModifyEventArg arg = e as NodeModifyEventArg;
        Debug.Assert(arg != null);
        foreach (int i in arg._modifiedNodeIndices)
            _samples[i]._list = path.Nodes[i].ComputeSamples(_sampleStep);
        UpdateSamples();
    }

    private void OnNodeAdd(object sender, EventArgs e)
    {
        _samples.Add(new ListVec3Wrapper(path.Nodes[path.NodeCount - 1].ComputeSamples(_sampleStep)));
        UpdateSamples();
    }

    private void OnNodeReset(object sender, EventArgs e)
    {
        _samples.Clear();
        for (int i = 0; i < path.NodeCount; i++)
            _samples.Add(new ListVec3Wrapper(path.Nodes[i].ComputeSamples(_sampleStep)));
        UpdateSamples();
    }

    public void Init()
    { 
        if (_samples == null)
        {
            _samples = new List<ListVec3Wrapper>();
            for (int i = 0; i < path.NodeCount; i++)
                _samples.Add(new ListVec3Wrapper(path.Nodes[i].ComputeSamples(_sampleStep)));
            UpdateSamples();
        }
    }

    public void ReSubscribe()
    {
        path.NodeModifyNotify -= OnNodeModify;
        path.NodeModifyNotify += OnNodeModify;
        path.NodeAddNotify -= OnNodeAdd;
        path.NodeAddNotify += OnNodeAdd;
        path.NodeResetNotify -= OnNodeReset;
        path.NodeResetNotify += OnNodeReset;
    }

    private void UpdateSamples()
    {
        int vertexCount = 0;
        foreach (var nodeSample in _samples)
            vertexCount += nodeSample._list.Count;
        lineRenderer.SetVertexCount(vertexCount);
        int nodeIdx = 0, count = 0, offset = 0;
        while (count < vertexCount)
        {
            lineRenderer.SetPosition(count, _samples[nodeIdx][offset]);
            count++;
            offset++;
            if (offset >= _samples[nodeIdx]._list.Count)
            {
                nodeIdx++;
                offset = 0;
            }
        }
    }

    [Serializable]
    public class ListVec3Wrapper
    {
        public List<Vector3> _list;
        public Vector3 this[int idx]
        {
            get
            { return _list[idx]; }
            set
            { _list[idx] = value; }
        }
        public ListVec3Wrapper(List<Vector3> list)
        {
            _list = list;
        }
    }

}

