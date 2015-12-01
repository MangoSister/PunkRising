using UnityEngine;
using System.Collections;

public class HealthManager : MonoBehaviour {
	public float health;
	public GameObject explosion;
	public GameObject barrage;
	int currentPos;
	GameObject Player;
	SkinnedMeshRenderer rend;
	bool flag;

	// Use this for initialization
	void Start () {
		if (gameObject.layer == 8) {
			rend = transform.GetChild (0).transform.GetChild (1).GetComponent<SkinnedMeshRenderer> ();
		}
		flag = true;
	}
	
	// Update is called once per frame
	void Update () {
        //rend.material.SetColor ("_Color",new Color(1,(health*2.55f)/255,(health*2.55f)/255,1));
        GetComponent<CitizenChargeFX>().ChargeTo(health /100f);
		var pos = this.transform.position;
		if (((health >= 100)&&(gameObject.layer == 8))&& flag) {
			flag = false;
//			GameObject clone = Instantiate(explosion,pos,Quaternion.identity) as GameObject;
//			Destroy(this.gameObject);
//			Destroy(clone,3);
			StartCoroutine(InspirationAnimationTrigger());
		}
		if ((health < 0)&&(gameObject.layer == 9)) {
						GameObject clone = Instantiate(explosion,pos,Quaternion.identity) as GameObject;
						Destroy(this.gameObject);
						Destroy(clone,3);
		}
//		if ((health < 0)&&(gameObject.layer == 8)) {
//			crowdManager..GetComponent<CrowdManager>().
//			Destroy(this.gameObject);
//		}

	}
	IEnumerator InspirationAnimationTrigger()
	{
		GetComponent<CitizenAnimHandler> ().TriggerInspirationAnim ();
		yield return new WaitForSeconds (0.5f);
		this.gameObject.GetComponent<CitizenMovementManager>().moveFlag = true;
	}
	void Shoot()
	{
		if (gameObject.layer == 9) {
			Debug.Log ("Imma Shoot");
			gameObject.transform.forward = (GameObject.FindGameObjectWithTag("Player").transform.forward);
			GameObject clone = Instantiate (barrage, transform.position, Quaternion.identity) as GameObject;
			clone.GetComponent<Rigidbody>().AddForce((-(transform.position+Player.transform.position))*5);
		}
	}
}
