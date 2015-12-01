using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class EnergyRefill : MonoBehaviour {

	// Use this for initialization
	public float energyLevel;
	float curvePoint1;
    public float scaleFactor;// = 0.001f;
	//public Scrollbar energyBar; 
	public GameObject guitar;
	CrowdManager crowdManager;
    EmitterSoundManager soundManager;
	public bool tutorialFlag;
	void Start () {
		tutorialFlag = true;
		energyLevel = 0;
        soundManager = GameObject.Find("SoundManager").GetComponent<EmitterSoundManager>();
		crowdManager = GameObject.Find ("CrowdManager").GetComponent<CrowdManager>();
	}
	public void ChangeEnergylevel(int energyUsed)
	{
		if (energyLevel > energyUsed) {
			energyLevel -= energyUsed;
			GuitarChargeFX fx = guitar.GetComponent<GuitarChargeFX>();
			fx.Charge(energyLevel/100);
		}
	}
	// Update is called once per frame
	void Update () {
				//Debug.Log (Input.GetAxis ("Yrot"));
		float rotval = Input.GetAxis ("Yrot");
		Vector3 rot = guitar.transform.rotation.eulerAngles;
		rot.z = -270 - ((-rotval) * 90);
		//Debug.Log (rot);
		guitar.transform.rotation = Quaternion.Euler(rot);

		//Debug.Log (guitar.transform.rotation);
	if (energyLevel < 100) {
			//Debug.Log ("Energy: " + Input.GetAxis ("Yrot"));
			if ((Input.GetAxis ("Yrot") <= -0.97) && (Input.GetAxis ("Yrot") >= -1)) {
				//energyLevel += scaleFactor * Mathf.Abs (curvePoint1 - Input.GetAxis ("Yrot"));
				//energyBar.size = energyLevel/100f;
				//if(Mathf.Abs (curvePoint1 - Input.GetAxis ("Yrot"))>0.5)
				//   {
				//	soundManager.GetComponent<SoundManager>().Chargefx();
				//}
				//soundManager.Play (14,transform.position,0.7f);
				energyLevel += 100 * scaleFactor;
				//energyBar.size += scaleFactor;
				GuitarChargeFX fx = guitar.GetComponent<GuitarChargeFX> ();
				soundManager.gameObject.GetComponent<SoundManager> ().Chargefx ();
				fx.Charge (energyLevel / 100);
			} else {
				soundManager.gameObject.GetComponent<SoundManager> ().ChargePausefx ();
			}
		}
		if(tutorialFlag&&(energyLevel>=100))
		   {
			tutorialFlag=false;
			crowdManager.tutFlag = true;
		}
	}
}
