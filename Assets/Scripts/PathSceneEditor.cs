using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(PathManager))]
public class PathSceneEditor : MonoBehaviour
{
    private PathManager path { get { return GetComponent<PathManager>(); } }
    private static readonly float moveDetectPrecision = 10e-3f;

    [SerializeField]
    private List<GameObject> _ctrlPtHandler;
    public float _handleSize = 0.2f;

    private Vector3 HandleIdxToPos(int idx)
    {
        int padIdx = idx + Mathf.Max((idx - 1), 0) / 3;
        return path.Nodes[padIdx / 4][padIdx % 4];
    }

    private void DecoupleHandleIdx(int idx, out int nodeIdx, out int ctrlPtIdx)
    {
        int padIdx = idx + Mathf.Max((idx - 1), 0) / 3;
        nodeIdx = padIdx / 4;
        ctrlPtIdx = padIdx % 4;
    }

    public void Init()
    {
        RecreateHandles();

    }

    public void ReSubscribe()
    {
        path.NodeAddNotify -= OnNodeCountChange;
        path.NodeAddNotify += OnNodeCountChange;
        path.NodeResetNotify -= OnNodeCountChange;
        path.NodeResetNotify += OnNodeCountChange;
        path.NodeModifyNotify -= OnNodeModify;
        path.NodeModifyNotify += OnNodeModify;
    }

    private void Update()
    {
        for (int i = 0; i < _ctrlPtHandler.Count; i++)
        {
            if ((_ctrlPtHandler[i].transform.position - HandleIdxToPos(i)).sqrMagnitude > moveDetectPrecision)
            {
                int nodeIdx, ctrlPtIdx;
                DecoupleHandleIdx(i, out nodeIdx, out ctrlPtIdx);
                path.ModifyPathNode(nodeIdx, ctrlPtIdx, _ctrlPtHandler[i].transform.position);
            }
        }
    }

    private void RecreateHandles()
    {
        if (_ctrlPtHandler != null)
        {
            foreach (var handle in _ctrlPtHandler)
#if UNITY_EDITOR
                DestroyImmediate(handle);
#else
                Destroy(handle);
#endif

            _ctrlPtHandler.Clear();
        }

        _ctrlPtHandler = new List<GameObject>();
        for (int i = 0; i < path.NodeCount * 3 + 1; i++)
        {
            var handle = GameObject.CreatePrimitive(PrimitiveType.Sphere);
            int nodeIdx, ctrlPtIdx;
            DecoupleHandleIdx(i, out nodeIdx, out ctrlPtIdx);
            handle.name = string.Format("handle[{0}][{1}]", nodeIdx, ctrlPtIdx);
            handle.transform.position = HandleIdxToPos(i);
            handle.transform.parent = path.gameObject.transform;
            handle.transform.localScale = Vector3.one * _handleSize;
            _ctrlPtHandler.Add(handle);
        }

    }

    private void OnNodeCountChange(object sender, EventArgs e)
    {
        RecreateHandles();
    }

    private void OnNodeModify(object sender, EventArgs e)
    {
        //very ugly hack
        for (int i = 0; i < _ctrlPtHandler.Count; i++)
        {
            _ctrlPtHandler[i].transform.position = HandleIdxToPos(i);
        }
    }

    public void EnableEdit(bool enable)
    {
        foreach (var handle in _ctrlPtHandler)
        {
            handle.SetActive(enable);
        }
    }
}
