using UnityEngine;
using System.Collections;

public class CitizenAnimHandler : MonoBehaviour
{
    public GameObject _modelObj;
    private Animator _animCtrl { get { return _modelObj.GetComponent<Animator>(); } }

    private Vector3 lastPos;

    private void Start()
    {
        lastPos = transform.position;
    }

	// Update is called once per frame
	private void Update ()
    {
        Vector3 currPos = transform.position;
        float speed = Vector3.Distance(currPos, lastPos) / Time.deltaTime;
        _animCtrl.SetFloat("MoveSpeed", speed);
        lastPos = currPos;
	}

    public void TriggerInspirationAnim()
    {
        _animCtrl.SetTrigger("Inspiration");
    }

    public void TriggerDeathAnim()
    {
        _animCtrl.SetTrigger("Die");
    }
}
