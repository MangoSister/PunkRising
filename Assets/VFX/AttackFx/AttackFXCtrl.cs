using UnityEngine;
using System.Collections;

public class AttackFXCtrl : MonoBehaviour
{
    public GameObject _bottom;
    public float _radius = 1f;
    public float _fxTime = 1f;
    public float bottomFadeOutStart = 0.8f;

    public ParticleSystem _particleSys;
    public float _particlePlayDelay;
    private bool _particlePlay;

    public void Execute()
    {
        StartCoroutine(ExecuteFXCoroutine());
    }

    private IEnumerator ExecuteFXCoroutine()
    {
        _bottom.transform.localScale = Vector3.one * _radius;
        _particleSys.startSize = _radius;
        _particleSys.startLifetime = _fxTime;
        Material bottomMat = _bottom.GetComponent<MeshRenderer>().material;
        bottomMat.SetFloat("_RingMaskScale", 0f);
        bottomMat.SetFloat("_FadeOut", 0f);
        _particlePlay = false;
        

        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < _fxTime)
        {
            currTime = Time.time;
            float currTimeRatio = Mathf.Clamp01((currTime - startTime) / _fxTime);

            bottomMat.SetFloat("_RingMaskScale", currTimeRatio);
            float bottomFadeOutWeight = Mathf.Clamp01((currTimeRatio - bottomFadeOutStart) / (1f - bottomFadeOutStart));
            bottomMat.SetFloat("_FadeOut", bottomFadeOutWeight);

            if (currTime - startTime > _particlePlayDelay && !_particlePlay)
            {
                _particleSys.Play();
                _particlePlay = true;
            }
            yield return null;
        }

        while (_particleSys.isPlaying)
            yield return null;
        Destroy(gameObject);
    }

}
