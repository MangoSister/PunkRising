using UnityEngine;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;

[CustomEditor(typeof(PathManager))]
public class PathManagerInspector : Editor
{

    public override void OnInspectorGUI()
    {
        PathManager manager = target as PathManager;
        base.OnInspectorGUI();
        if (GUILayout.Button("Add Node"))
        {
            manager.AddPathNode();
        }
        if (GUILayout.Button("Reset Nodes"))
        {
            manager.ResetNodes();
        }
    }
}
