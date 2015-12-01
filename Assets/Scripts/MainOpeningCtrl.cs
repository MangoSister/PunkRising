using UnityEngine;
using System.Collections;

public class MainOpeningCtrl : MonoBehaviour
{
    public float _fadeTime = 1f;
    // Use this for initialization
    private void Start ()
    {
        SoundManagerSingletonWrapper.Instance.
                GetComponent<SoundManager>().ShuffleMusic(_fadeTime, SoundManager.MusicType.Main);
    }
	
}
