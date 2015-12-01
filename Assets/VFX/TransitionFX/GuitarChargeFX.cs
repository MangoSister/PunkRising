using UnityEngine;
using System.Collections;

public class GuitarChargeFX : MonoBehaviour
{
    public Renderer _guitarBodyRenderer;
    public Texture2D _guitarBodyTex;
    private const string _shaderName = "Custom/GuitarChargeShader";

    public float _bottomHeight;
    public float _topHeight;
    public float _chargeSpeed;
    private float _currHeight;

    public float _EmissionIntensity = 0.5f;

    private void Start()
    {
        Debug.Assert(_chargeSpeed > 0f);
        Material mat = new Material(Shader.Find(_shaderName));
        _guitarBodyRenderer.material = mat;
        _currHeight = _bottomHeight;
        mat.SetVector("_PlanePos", new Vector4(0, _bottomHeight, 0, 1));
        mat.SetVector("_PlaneNormal", new Vector4(0, 1, 0, 0));
        mat.SetFloat("_ThresDist", _currHeight);
        mat.SetTexture("_MainTex", _guitarBodyTex);
        mat.SetFloat("_EmissionSwitch", 0f);
        mat.SetFloat("_EmissionIntensity", _EmissionIntensity);
    }

    public void Charge(float percent)
    {
        percent = Mathf.Clamp01(percent);
        _currHeight = Mathf.Lerp(_bottomHeight, _topHeight, percent);
        _guitarBodyRenderer.material.SetFloat("_ThresDist", _currHeight);
        if (percent > 0.9f)
            _guitarBodyRenderer.material.SetFloat("_EmissionSwitch", 1f);
        else
            _guitarBodyRenderer.material.SetFloat("_EmissionSwitch", 0f);
    }

    public void ChargeTo(float percent)
    {
        StartCoroutine(ChargeToCoroutine(percent));
    }

    private IEnumerator ChargeToCoroutine(float percent)
    {
        percent = Mathf.Clamp01(percent);
        float targetHeight = Mathf.Lerp(_bottomHeight, _topHeight, percent);
        float startHeight = _currHeight;
        float chargeTime = (targetHeight - _currHeight) / _chargeSpeed;
        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < chargeTime)
        {
            _currHeight = Mathf.Lerp(startHeight, targetHeight, Mathf.Clamp01((currTime - startTime) / chargeTime));
            _guitarBodyRenderer.material.SetFloat("_ThresDist", _currHeight);
            if (Mathf.InverseLerp(_bottomHeight, _topHeight, _currHeight) > 0.9f)
                _guitarBodyRenderer.material.SetFloat("_EmissionInstensity", 0.2f);
            else
                _guitarBodyRenderer.material.SetFloat("_EmissionInstensity", 0f);

            currTime = Time.time;
            yield return null;
        }
    }

    //private void Update()
    //{
    //    Charge(Mathf.Abs(Input.GetAxis("Horizontal")));
    //}

}
