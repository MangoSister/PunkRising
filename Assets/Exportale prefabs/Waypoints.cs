using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class Waypoints : MonoBehaviour {

	public Transform[] waypoint;        // The amount of Waypoint you want
	public float patrolSpeed = 10f;       // The walking speed between Waypoints
	public bool  loop = true;       // Do you want to keep repeating the Waypoints
	public float dampingLook= 6.0f;          // How slowly to turn
	public float pauseDuration = 0;   // How long to pause at a Waypoint
	
	private float curTime;
	public int currentWaypoint = 0;
	public GameObject character;
	public Transform[,] crowd = new Transform[5,10];
	public Vector3 target;
	public bool flag;
	public int crowdX = 0;
	public int crowdY = 0;
	public int totalPeople;
	public Canvas UIGroup;

	void SpawnGrid()
	{
		float x = 1;
		float y  = 0.5f;
		float z = 1;
		for (int i = 0; i < 5; i++)
		{
			for (int j = 0; j < 10; j++)
			{
				GameObject obj = new GameObject();
				obj.transform.parent = transform;
				crowd[i,j] = obj.transform;
				crowd[i, j].position = new Vector3(x, y, z);
				Debug.Log(crowd[i,j].position);
				x -= 0.5f;
			}
			x=1;
			z-=0.5f;
		}
	}

	public void UpdateUI()
	{
		//UIGroup.transform.GetChild (1).GetComponent<Text> ().text = totalPeople.ToString();
	}

	//List<transform> crowdline = new List<transform>();
	
	void  Start (){
		//character = GetComponent<CharacterController>();
		curTime = 0;
		totalPeople = 0;
		SpawnGrid ();
	}
//	void InitialiseCrowdSpace()
//	{
//		crowd[0] = new Transform [5];
//		for(int i =0; i<5;i++)
//		{
//			crowd[0][i] = i;
//		}
//	}
	void  Update (){		
//		if(currentWaypoint < waypoint.Length){
//			patrol();
//		}else{    
//			if(loop){
//				currentWaypoint=0;
//			} 
//		}

	}
	
	public void  patrol (GameObject tempCitizen,int currentpos){

		if(currentpos<8){
			if((currentpos == 7)&& flag)
			{
				flag = false;
				waypoint[7].position = crowd[crowdY,crowdX].position;
				//target = crowd[crowdX,crowdY].position;
				Debug.Log("pos: " + crowdX + crowdY );
				if(crowdX < 5){
					crowdX++;
					Debug.Log("increment");
				}
				else if(crowdX == 5)
				{
					crowdX = 0;
					crowdY++;
				}

			}
			else
			{
				target = waypoint [currentpos].position;
			}
			target.y = transform.position.y; // Keep waypoint at character's height
			Vector3 moveDirection = target - tempCitizen.gameObject.transform.position;
//			if(currentpos==6){
//				//tempCitizen.transform.position = Vector3.MoveTowards (tempCitizen.transform.position, crowd[2,2].position, patrolSpeed * Time.deltaTime);
//				target = crowd[2,2].position;
//				Debug.Log(target);
//			}
//			if (moveDirection.magnitude < 0.5f) {
//				currentpos++;
//				if (curTime == 0)
//					curTime = Time.time; // Pause over the Waypoint
//				if ((Time.time - curTime) >= pauseDuration) {
//					currentpos++;
//					curTime = 0;
//				}
//			} else {        
				var rotation = Quaternion.LookRotation (target - transform.position);
				transform.rotation = Quaternion.Slerp (transform.rotation, rotation, Time.deltaTime * dampingLook);
				tempCitizen.transform.position = Vector3.MoveTowards (tempCitizen.transform.position, target, patrolSpeed * Time.deltaTime);
				if (moveDirection.magnitude <0.4f) {
					//Debug.Log("times");
				}
//			}
			//currentWaypoint++;
			//Debug.Log ("pos" + currentpos);

		} 
	}

	void AssignCrowd()
	{

	}
}