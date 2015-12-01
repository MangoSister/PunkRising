using UnityEngine;
using System.Collections;

public class CitizenMovementManager : MonoBehaviour {
	
	public float speed;
	public GameObject player;
	GameObject target;
	GameObject crowdManager;
	public bool moveFlag;
	void Start () {
		crowdManager = GameObject.Find ("CrowdManager");
	}
	// Update is called once per frame
	void Update () {
		
		if(moveFlag){
			moveFlag = false;
				target = gameObject.GetComponent<CitizenPositionManager>().findClosestObject();
				Debug.Log("target" + target);
				this.gameObject.layer = 0;
				StartCoroutine("MovetoTarget");
				
				
			}
		}
	IEnumerator MovetoTarget()
	{
		crowdManager.GetComponent<CrowdManager> ().activeCitizens.Add (gameObject);
		while (Vector3.Distance (target.transform.position, transform.position) > 0.2) {
			float step = speed * Time.deltaTime;
			transform.position = Vector3.MoveTowards (transform.position, target.transform.position, step);
			transform.LookAt(target.transform.position);
            yield return null;//new WaitForFixedUpdate();
			//Debug.Log("distance" + Vector3.Distance (target.transform.position, transform.position));
		}
		gameObject.transform.forward = (GameObject.FindGameObjectWithTag("Player").transform.forward);
		transform.SetParent (crowdManager.transform);
        this.gameObject.GetComponent<CitizenAnimHandler>().TriggerInspirationAnim();
    }
}
