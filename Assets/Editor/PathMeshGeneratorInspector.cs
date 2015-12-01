using UnityEngine;
using UnityEditor;
using System.Collections;

[CustomEditor(typeof(PathMeshGenerator))]
public class PathMeshGeneratorInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        PathMeshGenerator meshGen = target as PathMeshGenerator;
        if (GUILayout.Button("Re-Extrude Mesh"))
        {
            meshGen.ExtrudeMesh();
        }
    }
}
