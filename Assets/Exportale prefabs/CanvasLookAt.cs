using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class CanvasLookAt : MonoBehaviour {

	// Use this for initialization

	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {

		foreach (Transform child in transform) {
			child.transform.LookAt(GameObject.FindGameObjectWithTag("Player").transform.position);
		}
	
	}

}
