using UnityEngine;
using System.Collections;

public class CitizenChargeFX : MonoBehaviour
{
    public Renderer _citizenBodyRenderer;
    public Texture2D _beforeTex;
    public Texture2D _afterTex;
    private const string _shaderName = "Custom/TexLocalTransitionPlane";

    public float _planeHeight;
    public float _bottomThres;
    public float _topThres;
    public float _chargeSpeed;
    private float _currThres;
    private void Start()
    {
        Debug.Assert(_chargeSpeed > 0f);
        Material mat = new Material(Shader.Find(_shaderName));
        _citizenBodyRenderer.material = mat;
        _currThres = _bottomThres;
        mat.SetVector("_PlanePos", new Vector4(_planeHeight, 0, 0, 1));
        mat.SetVector("_PlaneNormal", new Vector4(-1, 0, 0, 0));
        mat.SetFloat("_ThresDist", _currThres);
        mat.SetTexture("_BeforeTex", VFXManager.Instance.RandomRiotTex());
        mat.SetTexture("_AfterTex", _afterTex);
    }

    public void Charge(float percent)
    {
        percent = Mathf.Clamp01(percent);
        _currThres = Mathf.Lerp(_bottomThres, _topThres, percent);
        _citizenBodyRenderer.material.SetFloat("_ThresDist", _currThres);
    }

    public void ChargeTo(float percent)
    {
        StartCoroutine(ChargeToCoroutine(percent));
    }

    private IEnumerator ChargeToCoroutine(float percent)
    {
        percent = Mathf.Clamp01(percent);
        float targetThres = Mathf.Lerp(_bottomThres, _topThres, percent);
        float startThres = _currThres;
        float chargeTime = (targetThres - _currThres) / _chargeSpeed;
        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < chargeTime)
        {
            _currThres = Mathf.Lerp(startThres, targetThres, Mathf.Clamp01((currTime - startTime) / chargeTime));
            _citizenBodyRenderer.material.SetFloat("_ThresDist", _currThres);
            currTime = Time.time;
            yield return null;
        }
    }

    //private void Update()
    //{
    //    //Charge(Mathf.Abs(Input.GetAxis("Horizontal")));
    //    if (Input.GetKeyDown(KeyCode.P))
    //    {
    //        ChargeTo(.5f);
    //    }
    //}
}
