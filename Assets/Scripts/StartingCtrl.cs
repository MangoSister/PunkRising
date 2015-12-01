using UnityEngine;
using System.Collections;

public class StartingCtrl : MonoBehaviour
{
    public Animator _cameraAnim;
    public float _fadeTime;
	// Use this for initialization
	private void Start ()
    {
        StartCoroutine(StartingCoroutine());
    }
	
	// Update is called once per frame
	void Update () {
	
	}

    private IEnumerator StartingCoroutine()
    {
        //_cameraAnim.clip
        yield return new WaitForSeconds(0.1f);
        AnimationClip clip = (_cameraAnim.GetCurrentAnimatorClipInfo(0))[0].clip;
        yield return new WaitForSeconds(clip.length + 2f);
        SceneManager.Instance.GetComponent<ScreenFader>().fadeTime = _fadeTime;
        SoundManagerSingletonWrapper.Instance.
            GetComponent<SoundManager>().StopMusic(_fadeTime);
        SceneManager.Instance.TransitScene(SceneManager.SceneType.Main);
        yield return null;
    }
    
}
