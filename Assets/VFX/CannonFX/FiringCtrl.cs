using UnityEngine;
using System.Collections;

public class FiringCtrl : MonoBehaviour
{
    public float _lastTime = 2f;
	// Use this for initialization
	private void Start ()
    {
        StartCoroutine(SelfDestoryCoroutine());
	}

    private IEnumerator SelfDestoryCoroutine()
    {
        yield return new WaitForSeconds(_lastTime);
        Destroy(gameObject);
    }
}
