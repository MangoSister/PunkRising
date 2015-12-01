using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ButtonSoundManager : MonoBehaviour {


	public float Timer;
	public float volume;
	bool flag;
	// Use this for initialization
	private List<int> sequence = new List<int>(16)
	{
		2,1,4,2,3,5,1,2,2,1,4,2,3,1,5,2
	};
	public List <AudioClip>button1 = new List<AudioClip>(5);
	public List <AudioClip>button2 = new List<AudioClip>(5);
	public List <AudioClip>button3 = new List<AudioClip>(5);
	int count;
	void Start () {
		Timer = 0;
		Debug.Log ("count"+sequence.Count);
		count = -1;
		StartSoundControl ();
		flag = false;
		StartTimer ();
		//Debug.Log (sequence[0]);
	}
	public void StartSoundControl()
	{
		InvokeRepeating ("SwitchSound",0,2.2f);
	}
	// Update is called once per frame
	public void StartTimer()
	{
		flag = true;
	}
	void Update () {
		if (flag) {
			Timer += Time.deltaTime;
			if (Timer < 17.7) {
				volume = 0.15f;
			} else if ((Timer > 17.7) && (Timer < 35.5)) {
				volume = 0.3f;
			} else if ((Timer > 35.5) && (Timer < 76)) {
				volume = 0.4f;
			} else if ((Timer > 76) && (Timer < 160)) {
				volume = 0.5f;
			} else if ((Timer > 160) && (Timer < 195)) {
				volume = 0.2f;
			}
		}
	}
	void SwitchSound()
	{
		if (count < sequence.Count-1) {
			count++;
		} else {
			count = 0;
		}
		Debug.Log (sequence[count]);
	}
	public void Playbutton1 ()
	{
		GetComponent<EmitterSoundManager> ().Play (button1[sequence[count]-1],transform.position,volume,1f, AudioType.InputSFX);
	}
	public void Playbutton2 ()
	{
		GetComponent<EmitterSoundManager> ().Play (button2[sequence[count]-1],transform.position,volume,1f, AudioType.InputSFX);
	}
	public void Playbutton3 ()
	{
		GetComponent<EmitterSoundManager> ().Play (button3[sequence[count]-1],transform.position,volume,1f, AudioType.InputSFX);
	}
}
