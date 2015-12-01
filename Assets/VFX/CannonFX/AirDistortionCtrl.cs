using UnityEngine;
using System.Collections;

public class AirDistortionCtrl : MonoBehaviour
{
    public GameObject _distortionPlane;
    public float _rotationSpeed = 300f;
    public float _spreadAnimLength = 5f;
    public float _size;

    private void Start()
    {
        _distortionPlane.transform.localScale = Vector3.one * _size;
        StartCoroutine(SpreadCoroutine());
        StartCoroutine(RotationCoroutine());
    }

    private IEnumerator SpreadCoroutine()
    {
        var mat = _distortionPlane.GetComponent<MeshRenderer>().material;
        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < _spreadAnimLength)
        {
            mat.SetFloat("_BumpAmt", Mathf.Lerp(0f, 128f, 
                Mathf.Clamp01((currTime - startTime) / _spreadAnimLength)));
            currTime = Time.time;
            yield return null;
        }
    }

    private IEnumerator RotationCoroutine()
    {
        float angle = 0f;
        while (true)
        {
            angle += _rotationSpeed * Time.deltaTime;
            transform.localRotation = Quaternion.AngleAxis(angle, transform.worldToLocalMatrix.MultiplyVector(transform.forward));
            yield return null;
        }
        
    }

}
