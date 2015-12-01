using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;

[ExecuteInEditMode]
[RequireComponent(typeof(PathManager))]
public class PathMeshGenerator : MonoBehaviour
{
    public ExtrudeShape _shape;
    public float _scale = 1f;
    public Material _material;
    private PathManager path { get { return GetComponent<PathManager>(); } }
    public float _extrudeStep = 0.5f;
    private List<OrientedPoint> _samples;

    private const string meshObjName = "PathMesh";

    private GameObject meshObj
    {
        get { return transform.Find(meshObjName).gameObject; }
    }

    private void ReSample()
    {
        _samples = new List<OrientedPoint>();
        foreach (PathNode node in path.Nodes)
            _samples.AddRange(node.ComputeOrientedSamples(_extrudeStep));
        for (int i = 0; i < _samples.Count; i++)
        {
            var op = _samples[i];
            op._pos = transform.InverseTransformPoint(_samples[i]._pos);
            op._rot = Quaternion.Inverse(transform.rotation) * op._rot;
            _samples[i] = op;
        }
    }

    //NEED DEBUG
    public void ExtrudeMesh()
    {
        if (_shape == null)
            return;

        if (_samples == null)
            ReSample();

        int vertsInShape = _shape._vertices.Length;
        int segments = _samples.Count - 1;
        int edgeLoops = _samples.Count;
        int vertCount = vertsInShape * edgeLoops;
        int triCount = _shape._lines.Length * segments;
        int triIndexCount = triCount * 3;

        int[] triangleIndices = new int[triIndexCount];
        Vector3[] vertices = new Vector3[vertCount];
        Vector3[] normals = new Vector3[vertCount];
        Vector2[] uvs = new Vector2[vertCount];


        //Mesh Generation here
        for (int i = 0; i < _samples.Count; i++)
        {
            int offset = i * vertsInShape;
            for (int j = 0; j < vertsInShape; j++)
            {
                int id = offset + j;
                vertices[id] = _samples[i].LocalToWorld(_shape._vertices[j]._pos2d * _scale);
                normals[id] = _samples[i].LocalToWorldDirection(_shape._vertices[j]._normal);
                uvs[id] = new Vector2(_shape._vertices[j]._uCoord, i / ((float)edgeLoops));
                //uv stretching
            }
        }

        int ti = 0;
        for (int i = 0; i < segments; i++)
        {
            int offset = i * vertsInShape;
            for (int l = 0; l < _shape._lines.Length; l += 2)
            {
                int a = offset + _shape._lines[l] + vertsInShape;
                int b = offset + _shape._lines[l];
                int c = offset + _shape._lines[l + 1];
                int d = offset + _shape._lines[l + 1] + vertsInShape;
                triangleIndices[ti] = a; ti++;
                triangleIndices[ti] = b; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = c; ti++;
                triangleIndices[ti] = d; ti++;
                triangleIndices[ti] = a; ti++;
            }
        }

        if (meshObj == null)
        {
            GameObject newMeshObj = new GameObject();
            newMeshObj.transform.parent = transform;
            newMeshObj.transform.localPosition = Vector3.zero;
            newMeshObj.name = meshObjName;
            newMeshObj.AddComponent<MeshFilter>();
            newMeshObj.AddComponent<MeshRenderer>();
        }

        Mesh mesh = new Mesh();
        mesh.Clear();
        mesh.vertices = vertices;
        mesh.triangles = triangleIndices;
        mesh.normals = normals;
        mesh.uv = uvs;
        mesh.name = "pathMesh";
        meshObj.GetComponent<MeshFilter>().mesh = mesh;
        meshObj.GetComponent<MeshRenderer>().material = _material;
        //throw new NotImplementedException();
    }
}

[Serializable]
public class ExtrudeShape
{
    public ExtrudeShapeVertex[] _vertices;
    public int[] _lines;
};

[Serializable]
public class ExtrudeShapeVertex
{
    public Vector2 _pos2d;
    public Vector2 _normal;
    public float _uCoord;
}