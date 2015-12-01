using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
public class PathManager : MonoBehaviour
{
    //cubic poly-bezier spline
    //CONSTRAINT: preserve the angle of the derivatives across sections
    //TO-DO: currently cannot move the top gameobject
    [SerializeField]
    private List<PathNode> _nodes;
    public List<PathNode> Nodes { get { return _nodes; } }
    public int NodeCount {  get { return _nodes.Count; } }

    public event EventHandler NodeAddNotify;
    public event EventHandler NodeResetNotify;
    public event EventHandler NodeModifyNotify;

    private PathRenderer pathRenderer
    { get { return GetComponent<PathRenderer>(); } }
    private PathSceneEditor pathEditor
    { get { return GetComponent<PathSceneEditor>(); } }
    private PathMeshGenerator pathMeshGenerator
    { get { return GetComponent<PathMeshGenerator>(); } }

    private void AddPathNodeInternal()
    {
        GameObject obj = new GameObject();

        obj.transform.parent = transform;
        obj.transform.localPosition = Vector3.zero;

        PathNode node = obj.AddComponent<PathNode>();
        if (_nodes.Count == 0)
            node.Reset(transform.position, transform.right);
        else
            node.Reset(_nodes[_nodes.Count - 1]._ctrlPts[PathNode.CtrlPtNum - 1],
                Bezier.GetTangent3d(_nodes[_nodes.Count - 1]._ctrlPts, 1f));

        _nodes.Add(node);
        obj.name = string.Format("PathNode[{0}]", NodeCount - 1);
    }

    public void AddPathNode()
    {
        AddPathNodeInternal();

        if (NodeAddNotify != null)
            NodeAddNotify(this, null);
    }

    public void ModifyPathNode(int nodeIdx, int ptIdx, Vector3 newPos)
    {
        Debug.Assert(nodeIdx >= 0 && nodeIdx < _nodes.Count);
        Debug.Assert(ptIdx >= 0 && ptIdx < PathNode.CtrlPtNum);
        Vector3 oldPos = _nodes[nodeIdx][ptIdx];
        _nodes[nodeIdx][ptIdx] = newPos;

        List<int> modifiedIndices = new List<int>();
        modifiedIndices.Add(nodeIdx);

        if (ptIdx == 0 && nodeIdx > 0)
        {
            _nodes[nodeIdx - 1][3] = newPos;
            //add delta to neighbor pts
            _nodes[nodeIdx - 1][2] += newPos - oldPos;
            _nodes[nodeIdx][1] += newPos - oldPos;
            modifiedIndices.Add(nodeIdx - 1);
        }
        else if (ptIdx == 1 && nodeIdx > 0)
        {
            //rotate
            //_nodes[nodeIdx - 1][2]
            Vector3 dir = (_nodes[nodeIdx][0] - _nodes[nodeIdx][1]).normalized;
            _nodes[nodeIdx - 1][2] = _nodes[nodeIdx - 1][3] + dir * (_nodes[nodeIdx - 1][3] - _nodes[nodeIdx - 1][2]).magnitude;
            modifiedIndices.Add(nodeIdx - 1);
        }
        else if (ptIdx == 2 && nodeIdx < NodeCount - 1)
        {
            //rotate
            //_nodes[nodeIdx + 1][1]
            Vector3 dir = (_nodes[nodeIdx][3] - _nodes[nodeIdx][2]).normalized;
            _nodes[nodeIdx + 1][1] = _nodes[nodeIdx + 1][0] + dir * (_nodes[nodeIdx + 1][1] - _nodes[nodeIdx + 1][0]).magnitude;
            modifiedIndices.Add(nodeIdx + 1);
        }
        else if (ptIdx == 3 && nodeIdx < NodeCount - 1)
        {
            _nodes[nodeIdx + 1][0] = newPos;
            //add delta to neighbors pts
            _nodes[nodeIdx][2] += newPos - oldPos;
            _nodes[nodeIdx + 1][1] += newPos - oldPos;
            modifiedIndices.Add(nodeIdx + 1);
        }
        

        //if (ptIdx == 0 && nodeIdx > 0)
        //{
        //    _nodes[nodeIdx - 1][3] = newPos;
        //    modifiedIndices.Add(nodeIdx - 1);
        //}
        //else if (ptIdx == 3 && nodeIdx < NodeCount - 1)
        //{
        //    _nodes[nodeIdx + 1][0] = newPos;
        //    modifiedIndices.Add(nodeIdx + 1);
        //}

        if (NodeModifyNotify != null)
        {
            NodeModifyNotify(this, new NodeModifyEventArg(modifiedIndices));
        }
    }

    public void ResetNodes()
    {
        if (_nodes != null)
        {
            foreach (var node in _nodes)
#if UNITY_EDITOR
                DestroyImmediate(node.gameObject);
#else
                Destroy(node.gameObject);
#endif
            _nodes.Clear();
        }
        _nodes = new List<PathNode>();
        AddPathNodeInternal();
        if (NodeResetNotify != null)
            NodeResetNotify(this, null);
    }

    private void Awake()
    {
        if (_nodes == null)
            ResetNodes();
        if (pathRenderer == null)
            (gameObject.AddComponent<PathRenderer>()).Init();
        if (pathEditor == null)
            (gameObject.AddComponent<PathSceneEditor>()).Init();
        if (pathMeshGenerator == null)
            gameObject.AddComponent<PathMeshGenerator>();
        pathRenderer.ReSubscribe();
        pathEditor.ReSubscribe();
    }

    public void EnableEdit(bool enable)
    {
        pathRenderer.GetComponent<LineRenderer>().enabled = enable;
        pathEditor.EnableEdit(enable);
    }
}

public class NodeModifyEventArg : EventArgs
{
    public List<int> _modifiedNodeIndices;
    public NodeModifyEventArg(List<int> indices)
    {
        _modifiedNodeIndices = indices;
    }
}