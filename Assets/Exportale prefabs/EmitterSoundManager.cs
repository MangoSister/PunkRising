using UnityEngine;
using System.Collections;
using UnityEngine.Audio;

public class EmitterSoundManager : MonoBehaviour {

    public AudioMixer _masterMixer;
    public AudioMixerGroup _inputSFXMixer;
    public AudioMixerGroup _gameSFXMixer;
    public AudioMixerGroup _slowMotionSFXMixer;

    SoundManager soundManager;

	void Start()
	{
		soundManager = gameObject.GetComponent<SoundManager> ();
	}
	public AudioSource Play(int clip, Transform emitter, AudioType type)
	{
		return Play(clip, emitter, 1f, 1f, type);
	}
	
	public AudioSource Play(int clip, Transform emitter, float volume, AudioType type)
	{
		return Play(clip, emitter, volume, 1f, type);
	}

	public AudioSource Play(int clip, Transform emitter, float volume, float pitch, AudioType type)
	{
		//Create an empty game object
		GameObject go = new GameObject ("Audio: " +  soundManager.clips[clip].name);
		go.transform.position = emitter.position;
		go.transform.parent = emitter;
		
		//Create the source
		AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.outputAudioMixerGroup = GetMixer(type);
		source.clip = soundManager.clips[clip];
		source.volume = volume;
		source.pitch = pitch;
		source.Play ();
		Destroy (go, soundManager.clips[clip].length);
		return source;
	}
	
	public AudioSource Play(int clip, Vector3 point, AudioType type)
	{
		return Play(clip, point, 1f, 1f, type);
	}
	
	public AudioSource Play(int clip, Vector3 point, float volume, AudioType type)
	{
		return Play(clip, point, volume, 1f, type);
	}

	public AudioSource Play(int clip, Vector3 point, float volume, float pitch, AudioType type)
	{
		//Create an empty game object
		GameObject go = new GameObject("Audio: " + soundManager.clips[clip].name);
		go.transform.position = point;
        go.transform.parent = transform;
		//Create the source
		AudioSource source = go.AddComponent<AudioSource>();
        source.spatialBlend = 1f;
        source.outputAudioMixerGroup = GetMixer(type);
        source.clip = soundManager.clips[clip];
		source.volume = volume;
		source.pitch = pitch;
		source.Play();
		Destroy(go, soundManager.clips[clip].length);
		return source;
	}
	public AudioSource Play(AudioClip clip, Vector3 point, float volume, float pitch, AudioType type)
	{
		//Create an empty game object
		GameObject go = new GameObject("Audio: " + clip.name);
		go.transform.position = point;
        go.transform.parent = transform;
		//Create the source
		AudioSource source = go.AddComponent<AudioSource>();
		source.clip = clip;
        source.outputAudioMixerGroup = GetMixer(type);
        source.volume = volume;
		source.pitch = pitch;
		source.Play();
		Destroy(go, clip.length);
		return source;
	}

    private AudioMixerGroup GetMixer(AudioType type)
    {
        switch (type)
        {
            case AudioType.GameSFX: return _gameSFXMixer;
            case AudioType.InputSFX: return _inputSFXMixer;
            case AudioType.SlowMotionSFX: return _slowMotionSFXMixer;
            default: throw new UnityException("unexpected type");
        }
    }

    public void StartSlowMotionSFX()
    {
        _masterMixer.SetFloat("GameSFXVol", -80f);
        _masterMixer.SetFloat("InputSFXVol", -80f);
        _masterMixer.SetFloat("BgmVol", -20f);
        _masterMixer.SetFloat("SlowMotionSFXVol", 20f);
    }

    public void EndSlowMotionSFX()
    {
        _masterMixer.SetFloat("GameSFXVol", 0f);
        _masterMixer.SetFloat("InputSFXVol", 0f);
        _masterMixer.SetFloat("BgmVol", 0f);
        _masterMixer.SetFloat("SlowMotionSFXVol", -80f);
    }
}

public enum AudioType
{
    GameSFX, InputSFX, SlowMotionSFX,
};
