using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;


public class EndingCtrl : MonoBehaviour
{
    public float _musicfadeTime = 1f;
    public float _creditFadeTime = 2f;
    public Purification _pure;
    public List<Image> _creditImgs;

    // Use this for initialization
    private void Start()
    {
        StartCoroutine(EndingCoroutine());
    }

    private IEnumerator EndingCoroutine()
    {
        SoundManagerSingletonWrapper.Instance.
           GetComponent<SoundManager>().ShuffleMusic(_musicfadeTime, SoundManager.MusicType.End);
        foreach (var img in _creditImgs)
        {
            var oldCol = img.color;
            oldCol.a = 0f;
            img.color = oldCol;
        }


        _pure.Execute();
        yield return new WaitForSeconds(_pure._maxRange / _pure._spreadSpeed);

        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < _creditFadeTime)
        {
            foreach (var img in _creditImgs)
            {
                var oldCol = img.color;
                oldCol.a = Mathf.Lerp(0f, 1f, Mathf.Clamp01((currTime - startTime) / _creditFadeTime));
                img.color = oldCol;
            }
            currTime = Time.time;
            yield return null;
        }

        yield return null;
    }

}
