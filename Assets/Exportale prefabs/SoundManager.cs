using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class SoundManager : MonoBehaviour
{

    public AudioSource audioPlayer;
    public AudioSource bgmPlayer;
    public List<AudioClip> clips = new List<AudioClip>();

    public AudioClip _startBgm;
    public AudioClip _mainBgm;
    public AudioClip _endBgm;
    public float _bgmFadeTime = 1f;
    // Use this for initialization
    void Start()
    {
        clips.Capacity = 20;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void Inspirefx()
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.clip = clips[0];
            audioPlayer.Play();
            //audioPlayer.clip = null;
        }
    }
    public void Chargefx()
    {
        if (!audioPlayer.isPlaying)
        {
            audioPlayer.clip = clips[14];
            audioPlayer.Play();
        }
        //audioPlayer.clip = null;

    }
    public void ChargePausefx()
    {
        audioPlayer.Pause();
        //audioPlayer.clip = null;
    }

    public void StopMusic(float fadeTime)
    {
        StartCoroutine(FadeMusicCoroutine(bgmPlayer.volume, 0f, fadeTime));
    }

    public void ShuffleMusic(float fadeTime, MusicType type)
    {
        switch (type)
        {
            case MusicType.Start: bgmPlayer.clip = _startBgm; break;
            case MusicType.Main: bgmPlayer.clip = _mainBgm; break;
            case MusicType.End: bgmPlayer.clip = _endBgm; break;
        }
        bgmPlayer.Play();
        StartCoroutine(FadeMusicCoroutine(0f, 1f, fadeTime));

    }

    public enum MusicType
    {
        Start, Main, End,
    }

    private IEnumerator FadeMusicCoroutine(float startVol, float endVol, float fadeTime)
    {
        float startTime = Time.time;
        float currTime = startTime;
        while (currTime - startTime < fadeTime)
        {
            bgmPlayer.volume = Mathf.Lerp(startVol, endVol, Mathf.Clamp01((currTime - startTime) / fadeTime));
            currTime = Time.time;
            yield return null;
        }
    }
}
