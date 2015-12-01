using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class CrowdManager : MonoBehaviour {

	// Use this for initialization
	public List <GameObject>activeCitizens = new List<GameObject>();
	int count;
	public GameObject placeHolder;
	public float xOffset = -1;
	public int numHits;
    EmitterSoundManager soundManager; 
	public Text peopleIndicator;
	public bool tutFlag;
	void Start () {
		count = 0;
        //xOffset = -1;
		numHits = 2;
		tutFlag = false;
        soundManager = GameObject.Find("SoundManager").GetComponent<EmitterSoundManager>();
	}
	
	// Update is called once per frame
	void Update () {
		peopleIndicator.GetComponent<Text> ().text = activeCitizens.Count.ToString();
		if (tutFlag && (activeCitizens.Count == 3)) {
			tutFlag = false;
		}
		if(Input.GetKeyDown(KeyCode.O))
		{
//			Debug.Log("Number of Citizens : " + activeCitizens.Count);
//			KillCitizen();
			SpawnCitizen(transform.forward);
		}
		if(Input.GetKeyDown(KeyCode.I))
		{
			Debug.Log("Number of Citizens : " + activeCitizens.Count);
			KillCitizen();
		}
	
	}
	IEnumerator GameOver()
	{
        SceneManager.Instance.GetComponent<ScreenFader>().fadeColor = Color.black;
        SceneManager.Instance.TransitScene(SceneManager.SceneType.Start);
        yield return null;
	}
	public void OnTriggerEnter(Collider other)
	{
        if (other.gameObject.GetComponent<BulletFire>() != null)
        {
            if (activeCitizens.Count > 0)
                KillCitizen();
            else
            {
                Debug.Log("Game Over!! You Suck!!!");
                StartCoroutine("GameOver");
            }
        }
        else if (other.gameObject.GetComponent<Cannon>() != null)
        {
            Debug.Log("Game Over!! You Suck!!!");
            StartCoroutine("GameOver");
        }
	}
	public void KillCitizen()
	{
		//int pos = (int)Random.Range(0,activeCitizens.Count-1);
		//Debug.Log("pos: " + pos);
		activeCitizens[0].GetComponent<HealthManager>().health-=100/numHits;
		if (activeCitizens [0].GetComponent<HealthManager> ().health <= 0) {
			activeCitizens[0].GetComponent<CitizenPositionManager>().closestObject.tag = "Open";
			soundManager.GetComponent<EmitterSoundManager>().Play ((int)Random.Range(0,3.9f),transform.position, AudioType.GameSFX);
			StartCoroutine(DeathAnimationTrigger());

		}
	}
	IEnumerator DeathAnimationTrigger()
	{
		if (activeCitizens [0].gameObject != null) {
			activeCitizens [0].transform.SetParent (null);
			activeCitizens [0].GetComponent<CitizenAnimHandler> ().TriggerDeathAnim ();
			yield return new WaitForSeconds (1.5f);
			Destroy (activeCitizens [0].gameObject);
			activeCitizens.RemoveAt (0);
		} else {
			StartCoroutine(GameOver());
		}
	}
	public void SpawnCitizen(Vector3 direction)
	{
			//Debug.Log ("forward : " + transform.forward);
		var pos = direction;
		pos.x -= xOffset;
        pos.z += 1.2f;
        for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				if(((j<4))&&(i==2)){
//					var clone = Instantiate (placeHolder,pos,Quaternion.identity) as GameObject;
//					Debug.Log("position" + pos);
//					clone.transform.parent = transform;
//					pos.x -= 1f ;
				}
				else{
				var clone = Instantiate (placeHolder,pos,Quaternion.identity) as GameObject;
				Debug.Log("position" + pos);
				clone.transform.parent = transform;
					pos.x += 1f+ Random.Range(-0.2f,0.2f);
				}
			}
			//Destroy();
			pos.x=-xOffset;
			pos.z-=0.5f;
		}
	}
}
