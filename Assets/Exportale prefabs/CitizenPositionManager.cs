using UnityEngine;
using System.Collections;

public class CitizenPositionManager : MonoBehaviour {
	

	public GameObject closestObject;
	
	//other variables here for other functionality in this script
	
	void Start()
	{
		
		
	}
	public GameObject findClosestObject()
	{
		GameObject[]objectArray;
		objectArray = GameObject.FindGameObjectsWithTag("Open");
		
		float distance = Mathf.Infinity;
		
		Vector3 position = transform.position;
		
		foreach(GameObject currentObject in objectArray)
		{
			float distanceCheck;    
			distanceCheck = (currentObject.transform.position - position).magnitude;

			float currentDistance = distanceCheck;
			if ((currentDistance < distance)&&(distanceCheck>0))
			{
				closestObject = currentObject;
				
				distance = currentDistance;
				
			}
		}
		
		//Debug.Log (closestObject);
		closestObject.tag = "Close";
		return closestObject;
	}


}
