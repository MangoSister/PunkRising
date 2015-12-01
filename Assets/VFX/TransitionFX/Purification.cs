using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Purification : MonoBehaviour
{
    public GameObject _cityObj;
    public GameObject _center;
    private const string _shaderName = "Custom/TexTransitionPoint";
    
    public float _maxRange;
    public float _spreadSpeed;
    // Use this for initialization
    private List<MeshRenderer> _meshRenderers;

    public void Execute ()
    {
        _meshRenderers = new List<MeshRenderer>();
        GetAllRendererRecursively(_cityObj, ref _meshRenderers);

        foreach (MeshRenderer ren in _meshRenderers)
        {
            ren.material = new Material(Shader.Find(_shaderName));
            ren.material.SetVector("_SourcePos", 
                new Vector4(_center.transform.position.x, 
                            _center.transform.position.y, 
                            _center.transform.position.z, 0f));

            ren.material.SetTexture("_InsideTex", VFXManager.Instance.RandomRiotTex());
            ren.material.SetFloat("_ThresDist", 0f);
            ren.material.SetFloat("_SoftEdgeWidth", 1f);
        }

        StartCoroutine(PurifyCoroutine());
	}

    private void GetAllRendererRecursively(GameObject go, ref List<MeshRenderer> output)
    {
        if (go.GetComponent<MeshRenderer>() != null)
            output.Add(go.GetComponent<MeshRenderer>());
        foreach (Transform t in go.transform)
            GetAllRendererRecursively(t.gameObject, ref output);
    }

    private IEnumerator PurifyCoroutine()
    {
        //VFXManager.Instance._inspirationFXPrefab
        float purifyTime = _maxRange / _spreadSpeed;

        InsipirationFXCtrl wave = Instantiate(VFXManager.Instance._inspirationFXPrefab, _center.transform.position, Quaternion.identity) as InsipirationFXCtrl;
        wave._radius = _maxRange;
        wave._fxTime = purifyTime;
        wave.Execute();

        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < purifyTime)
        {
            float currRange = Mathf.Lerp(0f, _maxRange, Mathf.Clamp01((currTime - startTime) / purifyTime));
            foreach (MeshRenderer ren in _meshRenderers)
            {
                ren.material.SetFloat("_ThresDist", currRange);
            }
            currTime = Time.time;
            yield return null;
        }
        
    }
}
