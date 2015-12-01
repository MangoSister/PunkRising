using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class InputManager : MonoBehaviour {

	public int[] combo;
	public LayerMask citizen;
	int[] attack;
	int[] inspire;
	public bool flag;
	public float radius;
	public LayerMask cop;
	float initial;
	float curvePoint1;
	public float scaleFactor;
	public EnergyRefill RefillScript;
	public InsipirationFXCtrl InspirationFX;
    public AttackFXCtrl AttackFX;
    EmitterSoundManager soundManager;
	public bool tankFlag;
	public float threshold;
	public float inspirationPower = 100;
    public float Inspirethreshold = 5 ;
	public float numOfStrokes;
	public Image[] buttons;
	public Sprite[] buttonSprites;
	public Image stringButton;
	public GameObject tankIndicator;
	float stringYPos;

	public float reflectionTotalTime = 2f;

    // Use this for initialization
    void Start () {
		tankIndicator.SetActive(false);
		stringYPos = stringButton.transform.position.y;
		stringButton.gameObject.SetActive(false);
		RefillScript = GameObject.FindGameObjectWithTag ("Player").GetComponent<EnergyRefill> ();
        //UIGroup.transform.GetChild(0).GetComponent<Scrollbar>().size = 0;// energyLevel/100f;
		soundManager = GameObject.Find ("SoundManager").GetComponent<EmitterSoundManager>();
		//InspirationFX = GameObject.Find ("InspirationFX");
		//scaleFactor = 2;
		attack = new int[3];
		inspire = new int[3];
		int j = 3;
		radius = 10;
		for (int i =0;i<3; i++) {
			inspire[i] = i+1;
			attack[i] = j;
			j--;
			flag = true;
			tankFlag = false;
		}
	}
	void Push(int a)
	{
		int length = combo.Length;
		for (int i =0; i<length-1; i++) { 
			buttons[i].GetComponent<Image>().sprite = buttons[i+1].GetComponent<Image>().sprite;
			combo[i] = combo[i+1];
			if(buttons[i].GetComponent<Image>().sprite!=null)
			{
				var col = buttons[i].GetComponent<Image>().color;
				col.a = 255;
				buttons[i].GetComponent<Image>().color = col;
			}
		}
		buttons[length-1].GetComponent<Image>().sprite = buttonSprites[a-1]; 
		var col1 = buttons[length-1].GetComponent<Image>().color;
		col1.a = 255;
		buttons[length-1].GetComponent<Image>().color = col1;
		combo [length - 1] = a;
		flag = true;
	}
	// Update is called once per frame
	void Update () {
        if (threshold <= 0.01 * 250)
        {
            Debug.Log("TankDestroyed");
            tankFlag = false;
            tankIndicator.SetActive(false);
        }
        if ((Input.GetAxis ("Yrot") <= -0.97) && (Input.GetAxis ("Yrot") >= -1)) {
		}
		else{
		if(tankFlag)
		{
				tankIndicator.SetActive(true);
            if (threshold < 250)
            {
                threshold += 0.5f;
            }
		}
		if (Input.GetAxis ("DPadY") == 1) {
			stringButton.gameObject.SetActive(true);
                //			var rot = stringButton.transform.rotation;
                //			rot = Quaternion.Euler(0,0,225);
                //			stringButton.transform.rotation = rot;
            var rot = stringButton.transform.position;
			rot.y = stringYPos + 0.1f;
			stringButton.transform.position = rot;
			//Debug.Log(stringButton.transform.rotation);
		}
		if (Input.GetAxis ("DPadY") == -1) {
			stringButton.gameObject.SetActive(true);
                //			var rot = stringButton.transform.rotation;
                //			rot = Quaternion.Euler(0,0,135);
                //			stringButton.transform.rotation = rot;
                var rot = stringButton.transform.position;
			rot.y = stringYPos - 0.1f;
			stringButton.transform.position = rot;
		}
		if (Input.GetAxis ("DPadY") == 0) {
			stringButton.gameObject.SetActive(true);
			var rot = stringButton.transform.position;
			rot.y = stringYPos;
			stringButton.transform.position = rot;
		}
		if ((Input.GetKeyDown(KeyCode.Joystick1Button0))|| (Input.GetKeyDown(KeyCode.A))) {
			soundManager.gameObject.GetComponent<ButtonSoundManager>().Playbutton1();
			Inspirethreshold = numOfStrokes;
			Debug.Log ("1");
			Push(1);
		}
		else if ((Input.GetKeyDown(KeyCode.Joystick1Button1))|| (Input.GetKeyDown(KeyCode.S))) {
			soundManager.gameObject.GetComponent<ButtonSoundManager>().Playbutton2();
			Inspirethreshold = numOfStrokes;
			Debug.Log ("2");
			Push (2);
		}
		else if ((Input.GetKeyDown(KeyCode.Joystick1Button3)) || (Input.GetKeyDown(KeyCode.D))){
			soundManager.gameObject.GetComponent<ButtonSoundManager>().Playbutton3();
			Inspirethreshold = numOfStrokes;
			Debug.Log ("3");
			Push (3);
		}
		if(((((Input.GetAxis ("DPadY") == 1) || (Input.GetAxis ("DPadY") == -1 )))&& (!tankFlag))|| (Input.GetKeyDown(KeyCode.Space))){ 
			int count = 0;
			if(RefillScript.energyLevel>=10)
            	Inspirethreshold--;
			//flag =false;
			for(int i =0;i<combo.Length;i++)
			{
				if(combo[i] == inspire[i])
				{
					count ++;
				}
			}
			if((((count == 3)&&(RefillScript.energyLevel>=40))&& Inspirethreshold<=0)&& flag)
			{
				Inspirethreshold = numOfStrokes;
				flag = false;
				soundManager.Play(5,transform.position,0.35f, AudioType.GameSFX);
				Debug.Log("inspire");
				InspireSphere();
			}
			if((count!=3)&&flag)
			{
				Inspirethreshold = numOfStrokes;
			}
			count = 0;
			for(int i =0;i<combo.Length;i++)
			{
				if(combo[i].Equals(attack[i]))
				{
					count ++;
				}
			}
			if(((count == 3)&&(RefillScript.energyLevel>=20))&& flag)
			{
                flag = false;
				soundManager.Play(6,transform.position,0.15f, AudioType.GameSFX);
				Debug.Log("attack");
				KillSphere();
			}
			count = 0;

		}
		if (((Input.GetAxis ("DPadY") == 1)||(Input.GetAxis ("DPadY") == -1)) && tankFlag) {
			flag = true;
			threshold--;
		}
		}
	}
	void InspireSphere()
	{
		stringButton.gameObject.SetActive(false);
		int length = combo.Length;
		for (int j =0; j<length; j++) {
				buttons[j].GetComponent<Image>().sprite = null;
				var col = buttons[j].GetComponent<Image>().color;
				col.a = 0;
				buttons[j].GetComponent<Image>().color = col;
		}
		RefillScript.ChangeEnergylevel (40);
		Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius,citizen);
		InsipirationFXCtrl ctrl = Instantiate (InspirationFX, transform.position + transform.up * (0.05f + Random.Range(0, 0.02f)), Quaternion.identity) as InsipirationFXCtrl;
		ctrl.gameObject.transform.parent = transform;
		ctrl._radius = 10;
		ctrl.Execute ();
		int i = 0;
		float reduceFactor = inspirationPower / hitColliders.Length;
		while (i < hitColliders.Length) {
			//hitColliders[i].gameObject.GetComponent<Renderer>().material.SetColor("_Color",Color.black);
			hitColliders[i].gameObject.GetComponent<HealthManager>().health+=reduceFactor;
			if(hitColliders[i].gameObject.GetComponent<HealthManager>().health>100)
			{
				hitColliders[i].gameObject.GetComponent<HealthManager>().health = 100;
			}
				//<Material>().SetColor
			Debug.Log(hitColliders[i].gameObject.name);
			i++;
		}
	}
	void KillSphere()
	{
		stringButton.gameObject.SetActive(false);
		RefillScript.ChangeEnergylevel (20);
//        AttackFXCtrl ctrl = Instantiate(AttackFX, transform.position + transform.up * (0.05f + Random.Range(0, 0.02f)), Quaternion.identity) as AttackFXCtrl;
//        ctrl.gameObject.transform.parent = transform;
//        ctrl._radius = 10;
//        ctrl.Execute();
        //RefillScript.energyLevel -= 10;
		StartCoroutine (EmitSphere());
//        Collider[] hitColliders = Physics.OverlapSphere(transform.position, radius,cop);
//		int i = 0;
//		while (i < hitColliders.Length) {
//			hitColliders[i].gameObject.GetComponent<BulletFire>().Reflect();
//			i++;
//		}
	}
	IEnumerator EmitSphere()
	{
		AttackFXCtrl ctrl = Instantiate(AttackFX, transform.position + transform.up * (0.05f + Random.Range(0, 0.02f)), Quaternion.identity) as AttackFXCtrl;
		ctrl.gameObject.transform.parent = transform;
		ctrl._radius = 5f;
		ctrl._fxTime = reflectionTotalTime;
		
		Collider[] hitColliders;
		ctrl.Execute();
		
		float startTime = Time.time;
		float currTime = startTime;
		while (currTime - startTime < reflectionTotalTime) 
		{
			float currRadius = Mathf.Lerp(0f, 5f, Mathf.Clamp01( (currTime - startTime) / reflectionTotalTime ));
			hitColliders = Physics.OverlapSphere(transform.position, currRadius,cop);
			foreach(Collider col in hitColliders )
			{
				if(!col.gameObject.GetComponent<BulletFire>().Reflected)
					col.gameObject.GetComponent<BulletFire>().Reflect();
			}
			currTime = Time.time;
			yield return null;
		}
	}
}
